#if UNITY_EDITOR
using DMTimeArea;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using RenCSharp.Combat.Player;
using RenCSharp.Combat.Interfaces;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp.Combat.Enemies.Editor
{
    public class EnemyAttackTimeLineEditor : SimpleTimeArea
    {
        [SerializeField] private EnemyAttack targetToEdit;
        [SerializeField] private EnemyAttackTimelineData timelineData;

        private SerializedObject activeTimeline, targetToEditObject;
        private SerializedProperty targetToEditProperty, timelineToEditProperty;
        /// <summary>
        /// One of the properties of an EnemyAttack SO asset. See EnemyAttack.cs for more details.
        /// </summary>
        private SerializedProperty arenaDimensionsProperty, controlTypeProperty, attackDurationProperty,
            secondsPerSpawnProperty, projectilesPerSpawnProperty, projectilesThatSpawnProperty, spawnPointsProperty,
            initialDirectionsProperty, indexesProperty, projectileSpawnPositionMethodProperty, projectileIndexMethodProperty,
            loopsProperty;

        private ProjectileKnob curKnob;
        private Vector2 curKnobTimeAndOffset;
        //private SerializedProperty curKnobProjectileProperty;

        private Rect rectTotalArea;
        private Rect rectContent;
        private Rect rectTimeRuler;

        private Rect rectTopBar;
        private Rect rectLeft;
        public Rect rectLeftTopToolBar;
        private GenericMenu timeAreaCTXMenu, previewSpawnProj;

        public AnimationCurve m_AnimationCurve;

        private float _lastUpdateTime = 0f;
        /// <summary>
        /// How much to scale up values by (ie. multiply the expected 1920x1080 screen by X to get the rect size).
        /// Best if it's below 1 probby.
        /// </summary>
        private float previewRectScale = 0.25f;

        /// <summary>
        /// how much pre-scaled space is placed between guidelines in the projectile preview window
        /// </summary>
        private int previewGuidelineDistance = 100;
        /// <summary>
        /// preview pixel snapping for placing new projectiles inside the preview
        /// </summary>
        private int previewProjectilePlacingGridSnap = 20;
        private Rect VisualPreviewHolderRect
        {   
            get
            {
                //by default, we use a 1920x1080 full-screen. You can change this based on how you're scaling your canvases.
                float w = 1920f * previewRectScale;
                float h = 1080f * previewRectScale;
                return new Rect(rectTotalArea.width - w * 0.5f, rectTotalArea.height - h, w, h);
            }
            set
            {
                visualPreviewHolderRect = value;
            }
        }
        private Rect visualPreviewHolderRect = Rect.zero, visualPreviewArenaRect;
        private readonly float knobVerticalOffsetMult = 50f;
        private static readonly Vector2 minWindowSize = new Vector2(400f, 200f);
        /// <summary>
        /// gets multiplied by preview rect scale
        /// </summary>
        private static readonly Vector2 projectilePreviewSize = new Vector2(100, 100);

        private static Texture saveIcon, placeKnobIcon, arenaPreviewTexture, projectilePreviewTexture, singlePixel;
        private readonly static string assetPathToEditorIcons = "Assets/ddlc/Visuals/Editor/";

        private static readonly Color spawnC = new Color(1, 0.3f, 0, 1);
        private static readonly Color afterSpawnC = new Color(0.86f, 0.86f, 0.86f, 1);

        private static GUIContent PlaceKnobContent;
        private static GUIContent SaveContent;

        private static string PreferredSaveFolder = Application.dataPath;

        private static Event cur;
        #region Used
        private double runningTime = 10.0f;
        protected override double RunningTime
        {
            get { return runningTime; }
            set
            {
                runningTime = value;
            }
        }

        private static double cutOffTime = 15.0f;
        protected override double CutOffTime
        {
            get { return cutOffTime; }
            set
            {
                cutOffTime = value;
            }
        }

        private float LEFTWIDTH = 250f;

        public bool IsPlaying
        {
            get;
            set;
        }

        protected override bool IsLockedMoveFrame
        {
            get { return (IsPlaying || Application.isPlaying); }
        }

        protected override bool IsLockDragHeaderArrow
        {
            get { return IsPlaying; }
        }

        public override Rect _rectTimeAreaTotal
        {
            get { return rectTotalArea; }
        }

        public override Rect _rectTimeAreaContent
        {
            get { return rectContent; }
        }

        public override Rect _rectTimeAreaRuler
        {
            get { return rectTimeRuler; }
        }

        protected override float sequencerHeaderWidth
        {
            get { return LEFTWIDTH; }
        }

        #endregion
        #region EnemyAttackStuff
        private void PlaceANewKnob()
        {
            ProjectileKnob toAdd = new(Vector3.zero, Vector3.zero, null);
            float timeItSpawnsAt = (float)Math.Round(RunningTime, 1, MidpointRounding.AwayFromZero);
            float largestY = 0;

            foreach(KeyValuePair<Vector2, ProjectileKnob> kvp in projectiles)
            {
                if(kvp.Key.x == timeItSpawnsAt)
                {
                    largestY = kvp.Key.y + knobVerticalOffsetMult;
                }
            }

            Vector2 timeAndOffset = new Vector2(timeItSpawnsAt, largestY);
            projectiles.Add(timeAndOffset, toAdd);
        }
        /// <summary>
        /// Places a new knob, that takes in a spawn position. Probably being used by the arena preview window.
        /// </summary>
        /// <param name="pos">The spawn position of the new projectile.</param>
        /// <param name="toSpawn">The projectile being spawned.</param>
        private void PlaceANewKnob(Vector2 pos, Base_Projectile toSpawn = null)
        {
            ProjectileKnob toAdd = new(pos, Vector3.zero, toSpawn);
            float timeItSpawnsAt = (float)Math.Round(RunningTime, 1, MidpointRounding.AwayFromZero);
            float largestY = 0;
            foreach(KeyValuePair<Vector2, ProjectileKnob> kvp in projectiles)
            {
                if(kvp.Key.x == timeItSpawnsAt)
                {
                    largestY = kvp.Key.y + knobVerticalOffsetMult;
                }
            }

            Vector2 timeAndOffset = new Vector2(timeItSpawnsAt, largestY);
            projectiles.Add(timeAndOffset, toAdd);
            GrabAKnob(projectiles[timeAndOffset], timeAndOffset);
        }

        public void RemoveAKnob(Vector2 timeAndOffset)
        {
            projectiles.Remove(timeAndOffset);
            GrabAKnob(null, Vector2.zero);
        }

        private void GrabAKnob(ProjectileKnob newcur, Vector2 timeAndOffset)
        {
            curKnob = newcur;
            //Debug.Log("Grabbing a new knob: " + curKnob.ToString());
            curKnobTimeAndOffset = timeAndOffset;
        }
        private void ClearMarkers()
        {
            //get rid of all existing markers from previous enemy attack
            projectiles = new();
            curKnob = null;
        }
        /// <summary>
        /// Generate ProjectileKnob Markers from a given EnemyAttack object.
        /// </summary>
        /// <param name="ea">The EnemyAttack asset in question.</param>
        private void GrabMarkers(EnemyAttack ea)
        {
            ClearMarkers();

            timelineData = CreateInstance(typeof(EnemyAttackTimelineData)) as EnemyAttackTimelineData;
            timelineData.name = "tempfile"; //to make sure when saving, we can check if the name means it's a tempfile or a file
            //that already exists.
            targetToEditObject = new SerializedObject(ea);
            SerializedObject timelineDataSO = new SerializedObject(timelineData);

            arenaDimensionsProperty = targetToEditObject.FindProperty("arenaDimensions");
            controlTypeProperty = targetToEditObject.FindProperty("controlType");
            attackDurationProperty = targetToEditObject.FindProperty("attackDuration");
            secondsPerSpawnProperty = targetToEditObject.FindProperty("secondsPerProjectileSpawn");
            projectilesPerSpawnProperty = targetToEditObject.FindProperty("projectilesPerSpawn");
            projectilesThatSpawnProperty = targetToEditObject.FindProperty("projectilesThatSpawn");
            spawnPointsProperty = targetToEditObject.FindProperty("spawnPoints");
            initialDirectionsProperty = targetToEditObject.FindProperty("initialDirections");
            indexesProperty = targetToEditObject.FindProperty("indexes");
            projectileSpawnPositionMethodProperty = targetToEditObject.FindProperty("projectileSpawnPositionMethod");
            projectileIndexMethodProperty = targetToEditObject.FindProperty("projectileIndexMethod");
            loopsProperty = timelineDataSO.FindProperty("loops");

            List<Base_Projectile> storedProjectiles = new();

            if (_simpleTimeArea == null) InitTimeArea(false, false, true, true);

            _simpleTimeArea.hRangeMax = targetToEdit.AttackDuration;

            for (int i = 0; i < targetToEdit.SpawnPoints.Length; i++)
            {
                Base_Projectile stinker = targetToEdit.ProjectilesThatSpawn[targetToEdit.Indexes[i]];
                if (!storedProjectiles.Contains(stinker)) { storedProjectiles.Add(stinker); }
                ProjectileKnob toAdd = new(targetToEdit.SpawnPoints[i], targetToEdit.InitialDirections[i], stinker);
                float timeItSpawnsAt = targetToEdit.SecondsPerProjectileSpawn * Mathf.Floor((float)i / targetToEdit.ProjectilesPerSpawn);
                
                Vector2 timeAndOffset = new Vector2(timeItSpawnsAt, (i % targetToEdit.ProjectilesPerSpawn) * knobVerticalOffsetMult);
                projectiles.Add(timeAndOffset, toAdd);
            }

            timelineData.SetProjectilesThatSpawn = storedProjectiles.ToArray();
        }

        /// <summary>
        /// Generates ProjectileKnob Markers from a given EnemyAttackTimelineData object.
        /// </summary>
        /// <param name="eatd">The timeline in question.</param>
        private void GrabMarkers(EnemyAttackTimelineData eatd)
        {
            ClearMarkers();

            timelineData = eatd;
            targetToEditObject = new SerializedObject(eatd); //we also use the targettoeditobject for the new timeline attacks

            arenaDimensionsProperty = targetToEditObject.FindProperty("arenaDimensions");
            controlTypeProperty = targetToEditObject.FindProperty("controlType");
            attackDurationProperty = targetToEditObject.FindProperty("attackDuration");
            projectileSpawnPositionMethodProperty = targetToEditObject.FindProperty("projectileSpawnPositionMethod");
            projectileIndexMethodProperty = targetToEditObject.FindProperty("projectileIndexMethod");
            loopsProperty = targetToEditObject.FindProperty("loops");

            if (_simpleTimeArea == null) InitTimeArea(false, false, true, true);
            _simpleTimeArea.hRangeMax = eatd.AttackDuration;

            List<Base_Projectile> storedProjectiles = eatd.ProjectilesThatSpawn.ToList();
            SortedDictionary<int, ProjectileFrameData> timeline = eatd.GetTimelineInformation;

            foreach(KeyValuePair<int, ProjectileFrameData> frameData in timeline)
            {
                ProjectileSnub[] snubs = frameData.Value.ProjectilesSpawnedAtFrame;
                for(int i = 0; i < snubs.Length; i++)
                {
                    ProjectileKnob toAdd = new(snubs[i].SpawnPosition, snubs[i].InitialDirection, storedProjectiles[snubs[i].ProjectileIndex]);
                    float timeItSpawnsAt = (float)frameData.Key / _frameRate;
                    Vector2 timeAndOffset = new Vector2(timeItSpawnsAt, i * knobVerticalOffsetMult);
                    projectiles.Add(timeAndOffset, toAdd);
                }
            }
        }
        /// <summary>
        /// Draws all the Knobs for each projectile in the timeline data.
        /// </summary>
        private void DrawMarkers()
        {
            GUILayout.BeginArea(_rectTimeAreaTotal, string.Empty);
            foreach(KeyValuePair<Vector2, ProjectileKnob> knob in projectiles)
            {
                knob.Value.DisplayKnob(this, knob.Key, knob.Value == curKnob);
            }
            GUILayout.EndArea();
        }
        /// <summary>
        /// Saves the current timeline to an EnemyAttackTimelineData asset.
        /// </summary>
        private void SaveTimelineDataToFile()
        {
            List<Base_Projectile> projectilesToSave = new();
            Dictionary<int, ProjectileFrameData> frameDatasToSave = new();
            Vector2 prevPos = projectiles.First().Key;
            ProjectileFrameData pfd = new();
            List<ProjectileSnub> snubs = new(); 
            //int is frame?
            foreach (KeyValuePair<Vector2, ProjectileKnob> knob in projectiles)
            {
                ProjectileKnob pk = knob.Value;
                if (!projectilesToSave.Contains(pk.ProjectileToSpawn))
                {
                    projectilesToSave.Add(pk.ProjectileToSpawn);
                }

                ProjectileSnub newSnub = new();
                newSnub.ProjectileIndex = projectilesToSave.IndexOf(pk.ProjectileToSpawn);
                newSnub.SpawnPosition = pk.SpawnPosition;
                newSnub.InitialDirection = pk.InitialDirection;

                if (prevPos.x == knob.Key.x && !knob.Equals(projectiles.Last())) { snubs.Add(newSnub); prevPos = knob.Key; continue; }

                if (prevPos.x != knob.Key.x)
                {
                    pfd.ProjectilesSpawnedAtFrame = snubs.ToArray();

                    frameDatasToSave.Add((int)(prevPos.x * _frameRate), pfd); //?
                    snubs.Clear();
                    snubs.Add(newSnub);
                    pfd = new();
                }

                if (knob.Equals(projectiles.Last()))
                {
                    //snubs.Add(newSnub);
                    pfd.ProjectilesSpawnedAtFrame = snubs.ToArray();
                    frameDatasToSave.Add((int)(knob.Key.x * _frameRate), pfd);
                }

                prevPos = knob.Key;
            }

            timelineData.SetProjectilesThatSpawn = projectilesToSave.ToArray();
            Debug.Log("How many frame datas we have: " + frameDatasToSave.Count);
            timelineData.SetTimelineInformation = frameDatasToSave;
            timelineData.SetControlType = controlTypeProperty.managedReferenceValue as ControlType;
            timelineData.SetArenaDimensions = arenaDimensionsProperty.vector2Value;
            timelineData.SetAttackDuration = attackDurationProperty.floatValue;
            timelineData.SetProjectileSpawnPositionMethod = (AttackSpawnSelectionMethod)projectileSpawnPositionMethodProperty.boxedValue;
            timelineData.SetProjectileIndexMethod = (AttackSpawnSelectionMethod)projectileIndexMethodProperty.boxedValue;

            if (timelineData.name != "tempfile")
            {
                EditorUtility.SetDirty(timelineData);
                targetToEditObject.ApplyModifiedProperties();
                Repaint();
            }
            else
            {
                string filePath = EditorUtility.SaveFilePanel("Save Attack Timeline", PreferredSaveFolder, "New Timeline", "asset");
                if (!string.IsNullOrEmpty(filePath))
                {
                    //split up the taken filePath so we can skip over folders that aren't within the project, starting at Assets.
                    string[] split = filePath.Split('/');
                    string coolerPath = "";
                    int startIndex = 0;

                    for (int i = 0; i < split.Length; i++)
                    {
                        if (split[i] == "Assets")
                        { 
                            startIndex = i;
                            break;
                        }
                    }

                    for(int i = startIndex; i < split.Length - 1; i++)
                    {
                        coolerPath += (split[i] + "/");
                    }
                    coolerPath += split[split.Length - 1];

                    AssetDatabase.CreateAsset(timelineData, coolerPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("User gave a stinky filepath string (somehow?!?)");
                }
            }
        }
        //ideally, show a small area that shows where projectiles spawn, or even are at- at current time in the timeline!
        //change shape based off of the arena dimensions, too?
        private void DisplayAttackAreaPreview()
        {
            VisualPreviewHolderRect = GUI.Window(10, VisualPreviewHolderRect, DisplayAttackAreaWindow, "Preview Arena Display");
            GUI.BringWindowToFront(10);
        }

        private void DisplayAttackAreaWindow(int windowID)
        {
            //rember to flip y. somehow?!?!?!?!?!?!?!
            Color cToDraw = CoolColors.slightTransWhiteGUI;
            Color ogGUIc = GUI.color;
            Matrix4x4 ogMatrix = GUI.matrix;
            Vector2 arenaDimensions = arenaDimensionsProperty.vector2Value * previewRectScale;
            Vector2 arenaPos = new Vector2((visualPreviewHolderRect.width * 0.5f) - (arenaDimensions.x * 0.5f), (visualPreviewHolderRect.height * 0.5f) - (arenaDimensions.y * 0.5f));
            
            visualPreviewArenaRect = new Rect(arenaPos.x, arenaPos.y, arenaDimensions.x, arenaDimensions.y);

            GUI.DrawTexture(visualPreviewArenaRect, arenaPreviewTexture);
            Vector2 trueOrigin = visualPreviewArenaRect.center;

            Rect guideLineRect;

            //draw x guidelines
            for (float x = trueOrigin.x; x < visualPreviewHolderRect.width; x+=(previewGuidelineDistance * previewRectScale))
            {
                guideLineRect = new Rect(x, 0, 1, visualPreviewHolderRect.height);
                GUI.DrawTexture(guideLineRect, singlePixel, ScaleMode.StretchToFill, true, 0, cToDraw, 0, 0);
            }
            for (float x = trueOrigin.x - previewGuidelineDistance * previewRectScale; x > 0; x-=(previewGuidelineDistance * previewRectScale))
            {
                guideLineRect = new Rect(x, 0, 1, visualPreviewHolderRect.height);
                GUI.DrawTexture(guideLineRect, singlePixel, ScaleMode.StretchToFill, true, 0, cToDraw, 0, 0);
            }

            //draw y guidelines
            for(float y = trueOrigin.y; y < visualPreviewHolderRect.height; y+=(previewGuidelineDistance * previewRectScale))
            {
                guideLineRect = new Rect(0, y, visualPreviewHolderRect.width, 1);
                GUI.DrawTexture(guideLineRect, singlePixel, ScaleMode.StretchToFill, true, 0, cToDraw, 0, 0);
            }
            for(float y = trueOrigin.y - previewGuidelineDistance * previewRectScale; y > 0; y-=(previewGuidelineDistance * previewRectScale))
            {
                guideLineRect = new Rect(0, y, visualPreviewHolderRect.width, 1);
                GUI.DrawTexture(guideLineRect, singlePixel, ScaleMode.StretchToFill, true, 0, cToDraw, 0, 0);
            }

            //declare things to be re-used during foreach loop. less gc?
            Vector2 drawPos, flipYPos, offsetPos, rotatePoint, projOrigin, dirAtTime;
            Vector3 flipYDir;
            Rect drawProjectile;

            foreach (KeyValuePair<Vector2, ProjectileKnob> kvp in projectiles) 
            {
                ProjectileKnob pk = kvp.Value;
                //t is the current time in the projectile's life after being spawned.
                float t = (float)runningTime - kvp.Key.x;
                //don't render if before proj spawn!
                if (pk.ProjectileToSpawn == null) { GUI.matrix = ogMatrix; continue; }
                if (t > pk.ProjectileToSpawn.Lifetime || t < 0) { GUI.matrix = ogMatrix; continue; }

                projOrigin = new Vector2((visualPreviewHolderRect.width * 0.5f) - (pk.ProjectileToSpawn.SizeDelta.x * previewRectScale * 0.5f), (visualPreviewHolderRect.height * 0.5f) - (pk.ProjectileToSpawn.SizeDelta.y * previewRectScale * 0.5f));
                drawPos = new Vector2(projOrigin.x + pk.SpawnPosition.x * previewRectScale, projOrigin.y + pk.SpawnPosition.y * previewRectScale);

                flipYPos = new Vector3(pk.SpawnPosition.x, pk.SpawnPosition.y * -1);
                flipYDir = new Vector3(pk.InitialDirection.x, pk.InitialDirection.y * -1);

                drawPos = new Vector2(projOrigin.x + flipYPos.x * previewRectScale, projOrigin.y + flipYPos.y * previewRectScale);
                drawProjectile = new Rect(drawPos.x, drawPos.y, pk.ProjectileToSpawn.SizeDelta.x * previewRectScale, pk.ProjectileToSpawn.SizeDelta.y * previewRectScale);

                dirAtTime = flipYDir;

                if (kvp.Key.x == runningTime)
                {
                    //if the key equals running time, that means the projectile just "spawned"
                    cToDraw = spawnC;
                }
                else
                {
                    cToDraw = afterSpawnC;
                    //get the expected offset that comes from being a moving projectile at t seconds of its lifespan.
                    offsetPos = pk.ProjectileToSpawn.GetMovementType.GetPositionAtTime(t, pk.InitialDirection, pk.SpawnPosition, out dirAtTime, true) * previewRectScale;
                    drawPos = new Vector2(projOrigin.x + offsetPos.x, projOrigin.y + offsetPos.y);
                    drawProjectile = new Rect(drawPos.x, drawPos.y, pk.ProjectileToSpawn.SizeDelta.x * previewRectScale, pk.ProjectileToSpawn.SizeDelta.y * previewRectScale);
                }
                if (pk == curKnob) cToDraw = CoolColors.selectedOliveColor;
                //make sure the representation is rotated to match the initial direction (updating the rotation as it would in game probably isn't *that* important)
                rotatePoint = drawPos + new Vector2(pk.ProjectileToSpawn.SizeDelta.x * 0.5f * previewRectScale, pk.ProjectileToSpawn.SizeDelta.y * 0.5f * previewRectScale);
                GUIUtility.RotateAroundPivot(TrigHelper.GetDegreeFromVector(dirAtTime, 270), rotatePoint);
                //finally draw our projectile representation now that we have everything
                GUI.color = cToDraw;
                //draw texture w/ cords no work??????? even tho it should??!?!?!?
                GUI.DrawTextureWithTexCoords(drawProjectile, pk.ProjectileToSpawn.DisplayTexture, pk.ProjectileToSpawn.DisplayTextureRect, true);
                //GUI.DrawTexture(drawProjectile, projectilePreviewTexture);
                Debug.Log($"Drawing mf at rect: {drawProjectile}, texture: {pk.ProjectileToSpawn.DisplayTexture}, texturerect: {pk.ProjectileToSpawn.DisplayTextureRect}");
                GUI.matrix = ogMatrix;
                GUI.color = ogGUIc;

                if(drawProjectile.Contains(cur.mousePosition) && cur.button == 0 && cur.isMouse)
                {
                    GrabAKnob(pk, kvp.Key);
                    cur.Use();
                }
            }

            if(cur.button == 1 && cur.isMouse)
            {
                Vector2 wouldBeSpawnPosition = (cur.mousePosition - trueOrigin) / previewRectScale; //absolute minus origin = relative?
                wouldBeSpawnPosition = new Vector2(wouldBeSpawnPosition.x, wouldBeSpawnPosition.y * -1);
                float xMod = wouldBeSpawnPosition.x % previewProjectilePlacingGridSnap;
                float yMod = wouldBeSpawnPosition.y % previewProjectilePlacingGridSnap;

                wouldBeSpawnPosition = new Vector2(wouldBeSpawnPosition.x - xMod,wouldBeSpawnPosition.y - yMod); //"normalize" the super decimal garbage that the mouse pos will be
                //to instead increment in size by the grid spacing snap integer. (to make the numbers clean and usable!)

                previewSpawnProj = new();
                foreach (Base_Projectile bp in timelineData.ProjectilesThatSpawn)
                {
                    if (bp == null) continue;
                    previewSpawnProj.AddItem(new GUIContent($"Add Projectile ({bp.gameObject.name}) at: ({wouldBeSpawnPosition.x}, {wouldBeSpawnPosition.y}) - {runningTime}s"), false, delegate
                    {
                        PlaceANewKnob(wouldBeSpawnPosition, bp);
                    });
                }
                previewSpawnProj.AddSeparator("");
                previewSpawnProj.AddItem(new GUIContent($"Add Empty Projectile at: ({wouldBeSpawnPosition.x}, {wouldBeSpawnPosition.y}) - {runningTime}s"), false, delegate
                {
                    PlaceANewKnob(wouldBeSpawnPosition);
                });
                previewSpawnProj.ShowAsContext();
                cur.Use();
            }
        }

        #endregion
        #region WindowSpawning
        [MenuItem("Window/EnemyAttack Timeline Window", false, 2002)]
        public static void DoWindow()
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.Show();
        }
        /// <summary>
        /// enemy attack so
        /// </summary>
        /// <param name="given"></param>
        public static void DoWindow(EnemyAttack given)
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.SetTargetToEdit = given;
            window.Show();
        }
        /// <summary>
        /// enemy attack timeline so
        /// </summary>
        /// <param name="given"></param>
        public static void DoWindow(EnemyAttackTimelineData given)
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.SetTimelineData = given;
            window.Show();
        }
        #endregion
        private void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(OnEditorUpdate));
            _lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            _frameRate = 60f; //default to 60fps always to remain consistent with math
            //load textures used in GUI
            saveIcon = EditorGUIUtility.Load(assetPathToEditorIcons + "saveicon.png") as Texture;
            SaveContent = new GUIContent(saveIcon, "Save the contents of the current timeline to an Enemy Attack Timeline Data asset.");
            placeKnobIcon = EditorGUIUtility.Load(assetPathToEditorIcons + "placeknobicon.png") as Texture;
            PlaceKnobContent = new GUIContent(placeKnobIcon, "Place a new Projectile in the Timeline.");
            arenaPreviewTexture = EditorGUIUtility.Load(assetPathToEditorIcons + "arenapreview.png") as Texture;
            singlePixel = EditorGUIUtility.Load(assetPathToEditorIcons + "singlepixel.png") as Texture;
            projectilePreviewTexture = EditorGUIUtility.Load(assetPathToEditorIcons + "projectilepreview.png") as Texture;

            //set up timeline stuffs
            activeTimeline = new SerializedObject(this);
            targetToEditProperty = activeTimeline.FindProperty("targetToEdit");
            ProjectileKnob.SelectKnob += GrabAKnob;

            //Get Prefs
            PreferredSaveFolder = EditorPrefs.GetString("attacksavefolder", Application.dataPath);
            previewGuidelineDistance = EditorPrefs.GetInt("previewguideline", 100);
            previewProjectilePlacingGridSnap = EditorPrefs.GetInt("previewprojectileplacinggrid", 20);
            previewRectScale = EditorPrefs.GetFloat("previewrectscale", 0.25f);

            //Grab Markers if relevant
            if(timelineData != null) { GrabMarkers(timelineData); return; }
            if (targetToEdit != null) { targetToEditProperty.boxedValue = targetToEdit; GrabMarkers(targetToEdit); return; }
        }

        private void OnDisable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(OnEditorUpdate));
            activeTimeline = null;
            ProjectileKnob.SelectKnob = null;
        }

        private void OnEditorUpdate()
        {
            // float delta = (float)(EditorApplication.timeSinceStartup - _lastUpdateTime);
            if (!Application.isPlaying && this.IsPlaying)
            {
                double fTime = (float)EditorApplication.timeSinceStartup - _lastUpdateTime;
                this.RunningTime += Math.Abs(fTime) * 1.0f;
                if (this.RunningTime >= this.CutOffTime)
                {
                    this.PausePreView();
                }
            }
            //if (_simpleSample)
            //    BlendCenter.BlendAnimation((float)this.RunningTime, aa01, aa02, _targetObject);

            _lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            Repaint();
        }

        private void OnGUI()
        {
            cur = Event.current;
            Rect rectMainBodyArea = new Rect(0, toolbarHeight, base.position.width, this.position.height - toolbarHeight);
            rectTopBar = new Rect(0, 0, this.position.width, toolbarHeight);
            rectLeft = new Rect(rectMainBodyArea.x, rectMainBodyArea.y + timeRulerHeight, LEFTWIDTH, rectMainBodyArea.height);
            rectLeftTopToolBar = new Rect(rectMainBodyArea.x, rectMainBodyArea.y, LEFTWIDTH, timeRulerHeight);

            rectTotalArea = new Rect(rectMainBodyArea.x + LEFTWIDTH, rectMainBodyArea.y, base.position.width - LEFTWIDTH, rectMainBodyArea.height);
            rectTimeRuler = new Rect(rectMainBodyArea.x + LEFTWIDTH, rectMainBodyArea.y, base.position.width - LEFTWIDTH, timeRulerHeight);
            rectContent = new Rect(rectMainBodyArea.x + LEFTWIDTH, rectMainBodyArea.y + timeRulerHeight, base.position.width - LEFTWIDTH, rectMainBodyArea.height - timeRulerHeight);

            InitTimeArea(false, false, true, true);
            DrawTimeAreaBackGround();
            OnTimeRulerCursorAndCutOffCursorInput();
            DrawTimeRulerArea();

            // Draw your top bar
            DrawTopToolBar();
            // Draw left content
            DrawLeftContent();
            // Draw your left tool bar
            DrawLeftTopToolBar();
            if (projectiles.Count > 0) DrawMarkers();
            BeginWindows();
            DisplayAttackAreaPreview();
            EndWindows();

            GUILayout.BeginArea(rectContent);
            GUILayout.EndArea();

            //bail if outside rect, not a mouse click, or if in the preview rect.
            if (!rectContent.Contains(cur.mousePosition) || cur.type != EventType.MouseDown || visualPreviewHolderRect.Contains(cur.mousePosition)) return;
            if (cur.button == 1 && curKnob == null)
            {
                float timeX = PixelToTime(cur.mousePosition.x);
                timeAreaCTXMenu = new GenericMenu();
                foreach (Base_Projectile bp in timelineData.ProjectilesThatSpawn)
                {
                    if(bp == null) continue;
                    timeAreaCTXMenu.AddItem(new GUIContent($"Place Knob ({bp.gameObject.name}) At Frame: {Mathf.Floor(timeX * _frameRate)}"), false, delegate
                    {
                        runningTime = (double)timeX;
                        PlaceANewKnob(Vector2.zero,bp);
                    });
                }
                timeAreaCTXMenu.AddSeparator("");
                timeAreaCTXMenu.AddItem(new GUIContent($"Place Knob At Frame: {Mathf.Floor(timeX * _frameRate)}"), false, delegate
                {
                    runningTime = (double)timeX;
                    PlaceANewKnob();
                });
                timeAreaCTXMenu.ShowAsContext();
            }
            else if(cur.button == 0 && curKnob != null)
            {
                GrabAKnob(null, Vector2.zero);
            }
            cur.Use();
        }

        protected override void DrawVerticalTickLine()
        {
            Color preColor = Handles.color;
            Color color = Color.white;
            color.a = 0.3f;
            Handles.color = color;
            // draw vertical ticks
            float step = 10;
            float preStep = GetTimeArea.drawRect.height / 20f;
            // step = GetTimeArea.drawRect.y;
            step = 0f;
            while (step <= GetTimeArea.drawRect.height + GetTimeArea.drawRect.y)
            {
                Vector2 pos = new Vector2(rectContent.x, step + GetTimeArea.drawRect.y);
                Vector2 endPos = new Vector2(position.width, step + GetTimeArea.drawRect.y);
                step += preStep;
                float height = PixelToY(step);
                Rect rect = new Rect(rectContent.x + 5f, step - 10f + GetTimeArea.drawRect.y, 100f, 20f);
                GUI.Label(rect, height.ToString("0"));
                Handles.DrawLine(pos, endPos);
            }
            Handles.color = preColor;
        }
        /// <summary>
        /// Should be drawing the relevant properties: controlType, arenaDimensions, loops, attackDuration, and the spawningMethod enums.
        /// Also draws the current knob's information, if there is one.
        /// </summary>
        protected virtual void DrawLeftContent()
        {
            if (timelineData == null) return;
            GUILayout.BeginArea(rectLeft);
            if(targetToEdit != null || timelineData.name != "tempfile") //if we have actual timelinedata, it shouldn't matter if targettoedit is null
            {
                if (controlTypeProperty != null) EditorGUILayout.PropertyField(controlTypeProperty);

                if (arenaDimensionsProperty != null) EditorGUILayout.PropertyField(arenaDimensionsProperty);

                if(attackDurationProperty != null) EditorGUILayout.PropertyField(attackDurationProperty);

                if (loopsProperty != null) EditorGUILayout.PropertyField(loopsProperty);

                if(projectileSpawnPositionMethodProperty != null) EditorGUILayout.PropertyField(projectileSpawnPositionMethodProperty);

                if(projectileIndexMethodProperty != null) EditorGUILayout.PropertyField(projectileIndexMethodProperty);
            }

            if(curKnob != null)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Projectile Knob Data");
                curKnob.SpawnPosition = EditorGUILayout.Vector3Field("Spawn Position", curKnob.SpawnPosition);
                curKnob.InitialDirection = EditorGUILayout.Vector3Field("Initial Direction", curKnob.InitialDirection);
                curKnob.ProjectileToSpawn = EditorGUILayout.ObjectField(curKnob.ProjectileToSpawn, typeof(Base_Projectile), false) as Base_Projectile;
                //EditorGUILayout.PropertyField(curKnobProjectileProperty);
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the old enemy attack field for selection, as well as the settings menu to change framerate and desired file paths.
        /// </summary>
        protected virtual void DrawTopToolBar()
        {
            GUILayout.BeginArea(rectTopBar);
            Rect settingsDropDownRect = new Rect(rectTopBar.width - 32, rectTopBar.y, 30, 30);
            Rect enemyAttackSORect = new Rect(0, rectTopBar.y, 300, 18);
            Rect previewGuidelineRect = new(300, rectTopBar.y, 225, 18);
            Rect previewProjectilePlacingRect = new(525, rectTopBar.y, 225, 18);
            Rect previewRectScaleRect = new(750, rectTopBar.y, 225, 18);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(enemyAttackSORect,targetToEditProperty);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("We swapping out the stupid ah attack poperty!");
                activeTimeline.ApplyModifiedProperties();
                GrabMarkers(targetToEditProperty.boxedValue as EnemyAttack);
            }

            previewGuidelineDistance = EditorGUI.IntSlider(previewGuidelineRect, "Preview Guideline Distance", previewGuidelineDistance, 1, 99999);
            previewProjectilePlacingGridSnap = EditorGUI.IntSlider(previewProjectilePlacingRect, "Preview Snap Grid Spacing", previewProjectilePlacingGridSnap, 1, 99999);
            previewRectScale = EditorGUI.Slider(previewRectScaleRect, "Preview Rect Scale", previewRectScale, 0, 1);
            EditorPrefs.SetInt("previewguideline", previewGuidelineDistance);
            EditorPrefs.SetInt("previewprojectileplacinggrid", previewProjectilePlacingGridSnap);
            EditorPrefs.SetFloat("previewrectscale", previewRectScale);

            if (!Application.isPlaying && GUI.Button(settingsDropDownRect, ResManager.SettingIcon, EditorStyles.toolbarDropDown))
            {
                OnClickSettingButton();
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// Draws the controls: back-a-frame, play/pause, forward-a-frame, place-a-knob, and save-to-file.
        /// </summary>
        private void DrawLeftTopToolBar()
        {
            // left top tool bar
            GUILayout.BeginArea(rectLeftTopToolBar, string.Empty, EditorStyles.toolbarButton);
            GUILayout.BeginHorizontal();
            //go back a frame
            if (GUILayout.Button(ResManager.prevKeyContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                PreviousTimeFrame();
            }
            //toggle between playing and paused
            bool playing = IsPlaying;
            playing = GUILayout.Toggle(playing, ResManager.playContent, EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (!Application.isPlaying)
            {
                if (IsPlaying != playing)
                {
                    IsPlaying = playing;
                    if (IsPlaying)
                        PlayPreview();
                    else
                        PausePreView();
                }
            }
            //go forward a frame
            if (GUILayout.Button(ResManager.nextKeyContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                NextTimeFrame();
            }
            //stop and go back to beninging?
            if (GUILayout.Button(ResManager.StopIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))
                && !Application.isPlaying)
            {
                PausePreView();
                this.RunningTime = 0.0f;
            }
            //place a new knob
            if (GUILayout.Button(PlaceKnobContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                PlaceANewKnob();
            }
            //save the current timeline to an EnemyAttackTimelineData file
            if (GUILayout.Button(SaveContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                playing = false;
                PausePreView();
                SaveTimelineDataToFile();
            }

            GUILayout.FlexibleSpace();
            string timeStr = TimeAsString((double)this.RunningTime, "F2");
            GUILayout.Label(timeStr);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void PlayPreview()
        {
            IsPlaying = true;
        }

        private void PausePreView()
        {
            IsPlaying = false;
        }
        //do the thing where the user can select a desired folder where they store/save attacks
        protected override void OnCreateSettingContent(GenericMenu menu)
        {
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Set Preferred Default Save Folder"), false, delegate
            {
                PreferredSaveFolder = EditorUtility.SaveFolderPanel("Set Preferred Save Folder", PreferredSaveFolder, "");
                EditorPrefs.SetString("attacksavefolder", PreferredSaveFolder);
            });
        }

        public int m_segmentResolution = 20;
        private Dictionary<Vector2, ProjectileKnob> projectiles = new();

        public EnemyAttack SetTargetToEdit { set { targetToEdit = value; GrabMarkers(value); } }

        public EnemyAttackTimelineData SetTimelineData { set 
            { 
                timelineData = value;
                GrabMarkers(value);
            } 
        }
    }
    /// <summary>
    /// Exists to serve as an individual projectile spawn in an enemy attack timeline. Basically a layer of user readability before
    /// being converted into a file that is more memory-effective for the Fight_Manager to run through. (I HOPE?!?!)
    /// </summary>
    [Serializable]
    public class ProjectileKnob
    {
        [SerializeField] public Vector3 SpawnPosition = Vector3.zero;
        [SerializeField] public Vector3 InitialDirection = Vector3.zero;
        [SerializeField] public Base_Projectile ProjectileToSpawn = null;
        public static Action<ProjectileKnob, Vector2> SelectKnob;
        private readonly float knobSize = 30;
        
        private static Texture knobImage;
        private static GenericMenu DestroyMeMenu = null;

        public ProjectileKnob(Vector3 pos, Vector3 dir, Base_Projectile proj)
        {
            SpawnPosition = pos;
            InitialDirection = dir;
            ProjectileToSpawn = proj;
        }

        public void DisplayKnob(EnemyAttackTimeLineEditor timeArea, Vector2 timeAndOffset, bool selected = false)
        {
            if (knobImage == null) knobImage = EditorGUIUtility.Load("Assets/ddlc/Visuals/Editor/editordiamond.png") as Texture;
            Vector2 pos = new Vector2(timeArea.TimeToPixel(timeAndOffset.x), timeAndOffset.y + 100f);
            Rect drawRect = new Rect(pos.x - (knobSize * 0.5f) - timeArea._rectTimeAreaRuler.x, pos.y - knobSize * 0.5f, knobSize, knobSize);
            GUI.DrawTexture(drawRect, knobImage, ScaleMode.ScaleToFit, true, 0, selected ? CoolColors.selectedOliveColor : Color.white, 0, 0);
            Event cur = Event.current;
            if (!drawRect.Contains(cur.mousePosition) || cur.type != EventType.MouseUp) return; //bail if it's a bad event type

            if(cur.button == 0) SelectKnob?.Invoke(this, timeAndOffset);
            else if(cur.button == 1)
            { 
                DestroyMeMenu = new GenericMenu();
                DestroyMeMenu.AddItem(new GUIContent($"Delete Knob at {timeAndOffset.x} seconds."), false, delegate
                {
                    timeArea.RemoveAKnob(timeAndOffset);
                });
                DestroyMeMenu.ShowAsContext();
            }
            cur.Use();
        }

        public override string ToString()
        {
            return $"Spawn Pos: {SpawnPosition}, Initial Dir: {InitialDirection}, ProjectileSpawned: {ProjectileToSpawn?.name}";
        }
    }
}
#endif