#if UNITY_EDITOR
using DMTimeArea;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
            initialDirectionsProperty, indexesProperty, projectileSpawnPositionMethodProperty, projectileIndexMethodProperty;

        private ProjectileKnob curKnob;

        private Rect rectTotalArea;
        private Rect rectContent;
        private Rect rectTimeRuler;

        private Rect rectTopBar;
        private Rect rectLeft;
        public Rect rectLeftTopToolBar;

        public AnimationCurve m_AnimationCurve;

        private float _lastUpdateTime = 0f;
        private readonly float knobVerticalOffsetMult = 10f;
        private static readonly Vector2 minWindowSize = new Vector2(400f, 200f);
        private static Texture SaveIcon;
        private static string PreferredSaveFolder = Application.dataPath;
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
        private void GrabAKnob(ProjectileKnob newcur)
        {
            curKnob = newcur;
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

            if (_simpleTimeArea == null) InitTimeArea(false, false, true, true);

            _simpleTimeArea.hRangeMax = targetToEdit.AttackDuration;

            for (int i = 0; i < targetToEdit.SpawnPoints.Length; i++)
            {
                ProjectileKnob toAdd = new(targetToEdit.SpawnPoints[i], targetToEdit.InitialDirections[i], targetToEdit.ProjectilesThatSpawn[targetToEdit.Indexes[i]]);
                float timeItSpawnsAt = targetToEdit.SecondsPerProjectileSpawn * Mathf.Floor((float)i / targetToEdit.ProjectilesPerSpawn);
                
                Vector2 timeAndOffset = new Vector2(timeItSpawnsAt, (i % targetToEdit.ProjectilesPerSpawn) * knobVerticalOffsetMult);
                projectiles.Add(timeAndOffset, toAdd);
            }
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
            controlTypeProperty = targetToEditObject.FindProperty("controlTypeDimensions");
            attackDurationProperty = targetToEditObject.FindProperty("attackDuration");
            projectileSpawnPositionMethodProperty = targetToEditObject.FindProperty("projectileSpawnPositionMethod");
            projectileIndexMethodProperty = targetToEditObject.FindProperty("projectileIndexMethod");

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

        private void DrawMarkers()
        {
            GUILayout.BeginArea(_rectTimeAreaTotal, string.Empty);
            foreach(KeyValuePair<Vector2, ProjectileKnob> knob in projectiles)
            {
                knob.Value.DisplayKnob(this, knob.Key);
            }
            GUILayout.EndArea();
        }

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
                    snubs.Add(newSnub);
                    pfd.ProjectilesSpawnedAtFrame = snubs.ToArray();
                    frameDatasToSave.Add((int)(knob.Key.x * _frameRate), pfd);
                }

                prevPos = knob.Key;
            }

            timelineData.SetProjectilesThatSpawn = projectilesToSave;
            Debug.Log("How many frame datas we have: " + frameDatasToSave.Count);
            timelineData.SetTimelineInformation = frameDatasToSave;

            if (timelineData.name != "tempfile")
            {
                EditorUtility.SetDirty(timelineData);
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

        #endregion

        [MenuItem("Window/EnemyAttack Timeline Window", false, 2002)]
        public static void DoWindow()
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.Show();
        }

        public static void DoWindow(EnemyAttack given)
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.SetTargetToEdit = given;
            window.Show();
        }

        public static void DoWindow(EnemyAttackTimelineData given)
        {
            EnemyAttackTimeLineEditor window = GetWindow<EnemyAttackTimeLineEditor>(false, "Enemy Attack Timeline Editor");
            window.minSize = minWindowSize;
            window.SetTimelineData = given;
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction)System.Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(OnEditorUpdate));
            _lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            _frameRate = 60f;
            if (SaveIcon == null) SaveIcon = Resources.Load("EditorIcons/saveicon") as Texture;
            activeTimeline = new SerializedObject(this);
            targetToEditProperty = activeTimeline.FindProperty("targetToEdit");
            ProjectileKnob.SelectKnob += GrabAKnob;
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

            GUILayout.BeginArea(rectContent);
            //DrawCurveLine(rectTotalArea.x);

            GUILayout.EndArea();
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

        protected virtual void DrawLeftContent()
        {
            if (timelineData == null) return;
            GUILayout.BeginArea(rectLeft);
            if(targetToEdit != null || timelineData.name != "tempfile") //if we have actual timelinedata, it shouldn't matter if targettoedit is null
            {
                EditorGUILayout.PropertyField(arenaDimensionsProperty);
                EditorGUILayout.PropertyField(controlTypeProperty);
                EditorGUILayout.PropertyField(attackDurationProperty);
                EditorGUILayout.PropertyField(projectileSpawnPositionMethodProperty);
                EditorGUILayout.PropertyField(projectileIndexMethodProperty);
            }

            if(curKnob != null)
            {
                curKnob.SpawnPosition = EditorGUILayout.Vector3Field("Spawn Position", curKnob.SpawnPosition);
                curKnob.InitialDirection = EditorGUILayout.Vector3Field("Initial Direction", curKnob.SpawnPosition);
                curKnob.ProjectileToSpawn = EditorGUILayout.ObjectField(curKnob.ProjectileToSpawn, typeof(GameObject)) as Base_Projectile;
            }
            GUILayout.EndArea();
        }

        protected virtual void DrawTopToolBar()
        {
            GUILayout.BeginArea(rectTopBar);
            Rect settingsDropDownRect = new Rect(rectTopBar.width - 32, rectTopBar.y, 30, 30);
            Rect enemyAttackSORect = new Rect(0, rectTopBar.y, 300, 18);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(enemyAttackSORect,targetToEditProperty);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("We swapping out the stupid ah attack poperty!");
                activeTimeline.ApplyModifiedProperties();
                GrabMarkers(targetToEditProperty.boxedValue as EnemyAttack);
            }

            if (!Application.isPlaying && GUI.Button(settingsDropDownRect, ResManager.SettingIcon, EditorStyles.toolbarDropDown))
            {
                OnClickSettingButton();
            }
            GUILayout.EndArea();
        }

        private void DrawLeftTopToolBar()
        {
            // left top tool bar
            GUILayout.BeginArea(rectLeftTopToolBar, string.Empty, EditorStyles.toolbarButton);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(ResManager.prevKeyContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                PreviousTimeFrame();
            }

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

            if (GUILayout.Button(ResManager.nextKeyContent, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                NextTimeFrame();
            }

            if (GUILayout.Button(ResManager.StopIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))
                && !Application.isPlaying)
            {
                PausePreView();
                this.RunningTime = 0.0f;
            }

            if(GUILayout.Button(SaveIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
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

        protected override void OnCreateSettingContent(GenericMenu menu)
        {
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Set Preferred Default Save Folder"), false, delegate
            {
                PreferredSaveFolder = EditorUtility.SaveFolderPanel("Set Preferred Save Folder", PreferredSaveFolder, "");
            });
        }

        public int m_segmentResolution = 20;
        private Dictionary<Vector2, ProjectileKnob> projectiles = new();

        public EnemyAttack SetTargetToEdit { set { targetToEdit = value; } }

        public EnemyAttackTimelineData SetTimelineData { set { timelineData = value; } }
    }
    /// <summary>
    /// Exists to serve as an individual projectile spawn in an attack. Basically a layer of user readability before
    /// being converted into a file that is more memory-effective for the Fight_Manager to run through.
    /// </summary>
    [Serializable]
    public class ProjectileKnob
    {
        //when clicked, set the active knob to this bastard

        [SerializeField] public Vector3 SpawnPosition;
        [SerializeField] public Vector3 InitialDirection;
        [SerializeField] public Base_Projectile ProjectileToSpawn;
        public static Action<ProjectileKnob> SelectKnob;
        private readonly float knobSize = 30;
        private static Texture knobImage;

        public ProjectileKnob(Vector3 pos, Vector3 dir, Base_Projectile proj)
        {
            SpawnPosition = pos;
            InitialDirection = dir;
            ProjectileToSpawn = proj;
        }

        public void DisplayKnob(SimpleTimeArea timeArea, Vector2 timeAndOffset)
        {
            if (knobImage == null) knobImage = Resources.Load("EditorIcons/editordiamond") as Texture;
            Vector3 pos = new Vector3(timeArea.TimeToPixel(timeAndOffset.x), timeAndOffset.y + 100f, 0);
            Rect drawRect = new Rect(pos.x - (knobSize * 0.5f) - timeArea._rectTimeAreaRuler.x, pos.y - knobSize * 0.5f, knobSize, knobSize);
            if(GUI.Button(drawRect, knobImage))
            {
                SelectKnob?.Invoke(this);
            }
        }
    }
}
#endif