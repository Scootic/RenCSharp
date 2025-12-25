using UnityEngine;
using UnityEngine.UI;
namespace RenCSharp
{
    [RequireComponent(typeof(Image))]
    public class Animated_Image_Handler : MonoBehaviour
    {
        private Image image;
        private Sprite[] animationFrames;
        private float secondsPerFrame = 0.1f, t;
        private int curI;

        public Image Image => image;
        public Sprite[] AnimationFrames => animationFrames;
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

        public void ReceiveAnimationInformation(Sprite[] frames, float SPF)
        {
            curI = 0;
            t = 0;
            animationFrames = frames;
            secondsPerFrame = SPF;
            if(frames.Length > 0) image.sprite = frames[0];
        }
    }
}
