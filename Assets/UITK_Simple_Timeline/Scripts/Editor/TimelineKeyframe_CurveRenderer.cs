#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Helper = UITK_SimpleTimeline.SimpleTimelineUITK_Helper;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace UITK_SimpleTimeline.Editor
{
    [UxmlElement]
    public partial class TimelineKeyframe_CurveRenderer : VisualElement, IRegeneratableElement
    {
        private const int lineResolution = 50;
        SerializedProperty l = null, r = null;
        private TangentHandle outTanL, inTanR;
        private static Painter2D Painter;
        private Label label;
        private VisualElement tracker;

        private TimelineKeyframe Left => l.boxedValue as TimelineKeyframe;
        private TimelineKeyframe Right => r.boxedValue as TimelineKeyframe;

        public TimelineKeyframe_CurveRenderer()
        {
            style.borderBottomColor = Helper.ThirdLayerBorder;
            style.borderRightColor = Helper.ThirdLayerBorder;
            style.borderLeftColor = Helper.ThirdLayerBorder;
            style.borderTopColor = Helper.ThirdLayerBorder;
            style.height = Length.Percent(100);
            style.width = Length.Percent(100);
            style.flexGrow = 1;
            style.flexShrink = -1;
            style.borderTopWidth = 1;
            style.borderRightWidth = 1;
            style.borderLeftWidth = 1;
            style.borderBottomWidth = 1;
            style.backgroundColor = Helper.SecondLayerBorder;
            ReceiveKeyframes(null,null);
        }

        public void ReceiveKeyframes(SerializedProperty newL, SerializedProperty newR)
        {
            if (newL != null && newR != null)
            {
                if (tracker != null) { Remove(tracker); tracker = null; }

                if (label != null)
                {
                    Remove(label);
                    label = null;
                }
                l = newL;
                r = newR;

                tracker = new VisualElement();
                tracker.style.height = 0;
                tracker.style.width = 0;
                Add(tracker);

                outTanL = new(Vector2.zero,0f);
                outTanL.GiveCurAngle += ReceiveLAngle;
                inTanR = new(Vector2.down,180f);
                inTanR.GiveCurAngle += ReceiveRAngle;

                Add(outTanL);
                Add(inTanR);

                tracker.TrackPropertyValue(l, callback => { RegenerateElement(); });
                tracker.TrackPropertyValue(r, callback => { RegenerateElement(); });
                RegenerateElement();
            }
            else
            {
                style.backgroundImage = null;
                style.minHeight = 50f;

                if(outTanL != null)
                {
                    Remove(outTanL);
                    outTanL = null;
                }
                if(inTanR != null)
                {
                    Remove(inTanR);
                    inTanR = null;
                }

                if(Painter != null)
                {
                    Painter.Clear();
                    Painter.Dispose();
                    Painter = null;
                }
                if (label == null)
                {
                    label = new();
                    label.style.whiteSpace = WhiteSpace.Normal;
                    label.style.flexWrap = Wrap.Wrap;

                    if(newL == null)
                    {
                        label.text = "Select a keyframe to preview its curve.";
                    }else if(newR == null)
                    {
                        label.text = "No keyframe to the right of selected keyframe.";
                    }
                    Add(label);
                }
            }
        }

        public void RegenerateElement()
        {
            float minY = 0;
            float maxY = 1;
            if (l == null || r == null)
            {
                return;
            }
            if (Painter != null) { Painter.Clear(); Painter.Dispose(); }
            Painter = new();
            Painter.lineWidth = 1f;
            Painter.lineCap = LineCap.Round;
            Painter.lineJoin = LineJoin.Round;
            Painter.strokeColor = Helper.NeonGreen;
            Painter.BeginPath();
            float[] tangents = GetTangents();
            Painter.MoveTo(Vector2.zero);
            for(int i = 0; i < lineResolution; i++)
            {
                float percT = (float)i / (float)lineResolution;
                Vector2 v2 = new(percT, SplineAtTime(percT, tangents[0], tangents[1]) * -1);
                maxY = Mathf.Max(maxY, v2.y * -1);
                minY = Mathf.Min(minY, v2.y * -1);
                v2 *= 200f;
                Painter.LineTo(v2);
            }
            Painter.Stroke();
            VectorImage goose = ScriptableObject.CreateInstance<VectorImage>();
            Painter.SaveToVectorImage(goose);

            float difference = maxY - minY;

            style.backgroundImage = new StyleBackground(Background.FromVectorImage(goose));
            style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
            style.minHeight = difference;
            style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Top,minY);
        }

        private void ReceiveLAngle(float angle)
        {
            l.FindPropertyRelative("OutTangent").floatValue = AngleToTangent(angle);
            Helper.ApplyChangesToObject();
        }

        private void ReceiveRAngle(float angle)
        {
            r.FindPropertyRelative("InTangent").floatValue = AngleToTangent(angle);
            Helper.ApplyChangesToObject();
        }

        private float AngleToTangent(float angle)
        {
            float toReturn = 0f;

            return toReturn;
        }

        private float[] GetTangents()
        {
            float[] toReturn = new float[2];
            float difT = Right.Time - Left.Time;
            toReturn[0] = Left.OutTangent * difT;
            toReturn[1] = Right.InTangent * difT;
            return toReturn;
        }

        private float SplineAtTime(float time, float startingTangent, float endingTangent)
        {
            float timeSqr = time * time;
            float timeCub = timeSqr * time;

            float toReturn;

            toReturn = ((2 * timeCub - 3 * timeSqr + 1) * 0) + //val1
               ((timeCub - 2 * timeSqr + time) * startingTangent) +
               ((-2 * timeCub + 3 * timeSqr) * 1) + //val2
               ((timeCub - timeSqr) * endingTangent);

            return toReturn;
        }
    }
}
#endif