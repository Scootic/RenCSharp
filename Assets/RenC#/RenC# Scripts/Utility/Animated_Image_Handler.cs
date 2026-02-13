using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace RenCSharp
{
    [RequireComponent(typeof(Image))]
    public class Animated_Image_Handler : MonoBehaviour
    {
        private Image image;
        private Sprite[] animationFrames;
        private string[] spriteAssetGUIDs, subObjectGUIDs;
        private float secondsPerFrame = 0.1f, t;
        private int curI;

        public Image Image => image;
        public Sprite[] AnimationFrames => animationFrames;
        public string[] SpriteAssetGUIDs => spriteAssetGUIDs;
        public string[] SubObjectGUIDs => subObjectGUIDs;
        public float SecondsPerFrame => secondsPerFrame;

        void OnEnable() //?
        {
            t = 0;
            curI = 0;
            if(image == null) image = GetComponent<Image>();
            if(animationFrames == null) animationFrames = new Sprite[0];
            if(spriteAssetGUIDs == null) spriteAssetGUIDs = new string[0];
            if(subObjectGUIDs == null) subObjectGUIDs = new string[0];
        }

        void Update()
        {
            if (animationFrames.Length == 0) return;
            t += Time.deltaTime;
            if(t >= secondsPerFrame)
            {
                t = 0;
                curI++;
                if (curI >= animationFrames.Length) curI = 0;
                image.sprite = animationFrames[curI];
            }
        }
        /// <summary>
        /// Only for unsaved stuff. Just takes in sprites, no assetreferences.
        /// </summary>
        /// <param name="visuals">The sprites being run thru</param>
        /// <param name="SPF">Seconds per frame</param>
        public void ReceiveAnimationInformation(Sprite[] visuals, float SPF)
        {
            curI = 0;
            t = 0;
            animationFrames = visuals;
            secondsPerFrame = SPF;
            if (animationFrames.Length > 0 && animationFrames[0] != null) image.sprite = animationFrames[0];
        }

        /// <summary>
        /// Used for save load safety. AssetReference moment.
        /// </summary>
        /// <param name="spriteAssetGUID">Addressable AssetReference GUIDs</param>
        /// <param name="subObjectGUIDS">Addressable AssetReference.SubObjectNames</param>
        /// <param name="SPF">Seconds per Frame</param>
        public void ReceiveAnimationInformation(string[] spriteAssetGUID, string[] subObjectGUIDS, float SPF)
        {
            curI = 0;
            t = 0;
            spriteAssetGUIDs = spriteAssetGUID;
            subObjectGUIDs = subObjectGUIDS;

            foreach(Sprite s in animationFrames)
            {
                Addressables.Release(s);
            }

            animationFrames = new Sprite[spriteAssetGUID.Length];

            for (int i = 0; i < spriteAssetGUID.Length; i++) //run thru each
            {
                string key = $"{spriteAssetGUIDs[i]}";
                if (subObjectGUIDs[i] != "") { key += $"[{subObjectGUIDs[i]}]"; } //Debug.Log("We gotta sub-sprite!"); 
                AsyncOperationHandle spriteHandle = Addressables.LoadAssetAsync<Sprite>(key);
                spriteHandle.WaitForCompletion();

                if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    animationFrames[i] = spriteHandle.Result as Sprite;
                }
                else
                {
                    Debug.LogError("Fucked up loading the: " + i + "'th sprite!");
                    spriteHandle.Release();
                }
            }

            secondsPerFrame = SPF;
            if (animationFrames.Length > 0) image.sprite = animationFrames[0];
        }
    }
}
