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
        private string[] spriteAssetGUIDs;
        private float secondsPerFrame = 0.1f, t;
        private int curI;

        public Image Image => image;
        public Sprite[] AnimationFrames => animationFrames;
        public string[] SpriteAssetGUIDs => spriteAssetGUIDs;
        public float SecondsPerFrame => secondsPerFrame;

        void OnEnable()
        {
            t = 0;
            curI = 0;
            image = GetComponent<Image>();
            animationFrames = new Sprite[0];
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

        public void ReceiveAnimationInformation(string[] spriteAssetGUID, float SPF)
        {
            curI = 0;
            t = 0;
            spriteAssetGUIDs = spriteAssetGUID;
            animationFrames = new Sprite[spriteAssetGUID.Length];

            for(int i = 0; i < spriteAssetGUID.Length; i++) //run thru each
            {
                AsyncOperationHandle spriteHandle = Addressables.LoadAssetAsync<Sprite>(spriteAssetGUID[i]);
                spriteHandle.WaitForCompletion();

                if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    animationFrames[i] = spriteHandle.Result as Sprite;
                }
                else
                {
                    Debug.LogError("Fucked up loading the: " + i + "'th sprite!");
                }

                spriteHandle.Release();
            }

            secondsPerFrame = SPF;
            if(animationFrames.Length > 0) image.sprite = animationFrames[0];
        }
    }
}
