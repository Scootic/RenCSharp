using TMPro;
using UnityEngine;
namespace RenCSharp
{
    public class TextHolder_Scaler : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;
        [SerializeField] private RectTransform me, textRect;

        [Header("Scaling Settings")]
        [SerializeField,Tooltip("Decides if text should be autoscaled if charcount is between min and max.")] private bool autoSizeTextIfPartialScaling = true;
        [SerializeField, Min(1), Tooltip("If the TMP char count is less than this, just use minSizeDelta.")] private int minCharCount = 14;
        [SerializeField, Min(2), Tooltip("If the TMP char count is bigger than this, use maxSizeDelta and set text to auto-scale.")] private int maxCharCount = 60;

        [Header("Size Deltas")]
        [SerializeField] private Vector2 minSizeDelta;
        [SerializeField] private Vector2 maxSizeDelta;
        [SerializeField] private Vector2 localTextRectDeltaOffset;

        [Header("Lerp Position?")]
        [SerializeField, Tooltip("Decides if the textholder should move when scaling.")] private bool lerpPosition = false;
        [SerializeField, Tooltip("Please be local")] private Vector3 minPosition;
        [SerializeField, Tooltip("Please be local")] private Vector3 maxPosition;

        public void ScaleTextHolder()
        {
            int foundcharcount = textComponent.text.Length; //might include hidden chars? maybe not too bad a problem.
            Debug.Log($"Found charcount for textholder {gameObject.name}: {foundcharcount}");
            if (foundcharcount < minCharCount)
            {
                me.sizeDelta = minSizeDelta;
                if(lerpPosition) me.localPosition = minPosition;
            }
            else if (foundcharcount > maxCharCount)
            {
                me.sizeDelta = maxSizeDelta;
                textComponent.enableAutoSizing = true;
                if (lerpPosition) me.localPosition = maxPosition;
            }
            else //if the char count is inbetween, scale dat shizz. go vertical?
            {
                float perc = (float)(foundcharcount - minCharCount) / (float)(maxCharCount - minCharCount);
                Vector2 desSizeDelta = Vector2.Lerp(minSizeDelta, maxSizeDelta, perc);
                me.sizeDelta = desSizeDelta;
                textComponent.enableAutoSizing = autoSizeTextIfPartialScaling;
                if (lerpPosition) me.localPosition = Vector3.Lerp(minPosition, maxPosition, perc);
            }

            textRect.sizeDelta = me.sizeDelta + localTextRectDeltaOffset;
            textRect.localPosition = Vector3.zero; //extra make sure
            Debug.Log($"New Text Rect SizeDelta for {gameObject.name}: {textRect.sizeDelta} ({me.sizeDelta} + {localTextRectDeltaOffset})");

        }
    }
}
