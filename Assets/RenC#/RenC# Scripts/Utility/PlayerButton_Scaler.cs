using TMPro;
using UnityEngine;
namespace RenCSharp
{
    public class PlayerButton_Scaler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private RectTransform me;

        [SerializeField, Min(1), Tooltip("If the TMP char count is less than this, just use minSizeDelta.")] private int minCharCount = 14;
        [SerializeField, Min(2), Tooltip("If the TMP char count is bigger than this, use maxSizeDelta and set text to auto-scale.")] private int maxCharCount = 60;

        [SerializeField] private Vector2 minSizeDelta;
        [SerializeField] private Vector2 maxSizeDelta;

        private void OnEnable()
        {
            ScaleButton();
        }

        public void ScaleButton()
        {
            int foundcharcount = textComponent.textInfo.characterCount; //might include hidden chars? maybe not too bad a problem.

            if (foundcharcount < minCharCount)
            {
                me.sizeDelta = minSizeDelta;
            }
            else if (foundcharcount > maxCharCount)
            {
                me.sizeDelta = maxSizeDelta;
                textComponent.autoSizeTextContainer = true;
            }
            else //if the char count is inbetween, scale dat shizz. go vertical?
            {
                float perc = (float)foundcharcount / (float)maxCharCount;
                Vector2 desSizeDelta = Vector2.Lerp(minSizeDelta, maxSizeDelta, perc);
                me.sizeDelta = desSizeDelta;
            }
        }
    }
}
