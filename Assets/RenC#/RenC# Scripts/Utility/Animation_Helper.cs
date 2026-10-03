#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace RenCSharp
{
    /// <summary>
    /// Provides a bunch of useful static AnimationCurve presets.
    /// </summary>
    public readonly struct Animation_Helper 
    {
        //have to use the silly return a copy thing; if you don't, since it's static, changes made in inspector will affect
        //all anim curves that equal the static reference.
        /// <summary>
        /// logarithmic type curve from 0 to 1
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve EaseOutCurve()
        {
            AnimationCurve toReturn = new();
            toReturn.CopyFrom(new AnimationCurve(EaseOut1(), EaseOut2()));
            for(int i = 0; i < toReturn.keys.Length ; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
            }
            return toReturn;
        }
        /// <summary>
        /// initial burst, before slowing down to zero
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve EarlyPeakToZeroCurve()
        {
            AnimationCurve toReturn = new();
            toReturn.CopyFrom(new AnimationCurve(EarlyPeak1(), EarlyPeak2()));
            for (int i = 0; i < toReturn.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
            }
            return toReturn;
        }
        /// <summary>
        /// oscillates rapidly between max and max negative, before winding down back to zero.
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve JostleCurve()
        {
            AnimationCurve toReturn = new();
            toReturn.CopyFrom(new AnimationCurve(JostleKeyframes));
            for (int i = 0; i < toReturn.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
            }
            return toReturn;
        }
        /// <summary>
        /// logarithmic type curve from 1 to 0
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve EaseDownCurve()
        {
            AnimationCurve toReturn = new();
            toReturn.CopyFrom(new AnimationCurve(EaseDown1(), EaseDown2()));
            for (int i = 0; i < toReturn.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
                AnimationUtility.SetKeyRightTangentMode(toReturn, i, AnimationUtility.TangentMode.Free);
            }
            return toReturn;
        }

        public static AnimationCurve EarlyPeakToZero => EarlyPeakToZeroCurve();
        public static AnimationCurve Jostle => JostleCurve();
        public static AnimationCurve EaseOut => EaseOutCurve();
        public static AnimationCurve EaseDown => EaseDownCurve();
        #region EaseOut
        private static Keyframe EaseOut1()
        {
            return new(0, 0, 2, 2, 0, 0) 
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe EaseOut2()
        {
            return new(1, 1, 0, 0, 0, 0)
            {
                weightedMode = WeightedMode.None
            };
        }
        #endregion
        #region EaseDown
        private static Keyframe EaseDown1()
        {
            return new(0, 1, 0, 0, 0, 0)
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe EaseDown2()
        {
            return new(1, 0, -2, 0, 0, 0)
            {
                weightedMode = WeightedMode.None
            };
        }
        #endregion
        #region EarlyPeakToZero
        private static Keyframe EarlyPeak1()
        {
            return new(0, 0, 5.77f, 5.77f, 0, 0.02f) 
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe EarlyPeak2()
        {
            return new(1, 0, -1.35f, -1.35f, 0, 0.02f)
            {
                weightedMode = WeightedMode.None
            };
        }
        #endregion
        #region Jostle
        private static Keyframe Jostle1()
        {
            return new(0, 0, 0, 0, 0, 0) 
            {
                weightedMode = WeightedMode.None 
            };

        }
        private static Keyframe Jostle2()
        {
            return new(0.05f, 1, 0.05f, 0.05f, 1, 0.33f)
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle3()
        {
            return new(0.15f, -1, -0.01f, -0.01f, 0.33f, 1)
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle4()
        {
            return new(0.3f, 1, -0.003f, -0.003f, 0.8f, 0.5f) 
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle5()
        {
            return new(0.4f, -1, 0.03f, 0.03f, 0.33f, 0.45f)
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle6()
        {
            return new(0.5f, 0.62f, 0.01f, 0.01f, 1, 0.33f) 
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle7()
        {
            return new(0.7f, -0.5f, -0.04f, -0.04f, 1, 0.33f) 
            {
                weightedMode = WeightedMode.None
            };
        }
        private static Keyframe Jostle8() 
        {
            return new(0.85f, 0.2f, -0.1f, -0.1f, 1, 0.33f) 
            {
                weightedMode = WeightedMode.None
            };
            
        }
        private static Keyframe Jostle9()
        {
            return new(1, 0, -0.03f, -0.03f, 1, 0)
            {
                weightedMode = WeightedMode.None
            };
        }
        private static readonly Keyframe[] JostleKeyframes =
        {
            Jostle1(), Jostle2(), Jostle3(), Jostle4(), Jostle5(), Jostle6(), Jostle7(), Jostle8(), Jostle9()
        };
        #endregion
    }
}
#endif