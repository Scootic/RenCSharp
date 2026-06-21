using UnityEngine;

namespace RenCSharp
{
    /// <summary>
    /// Provides a bunch of useful static AnimationCurve presets.
    /// </summary>
    public struct Animation_Helper 
    {
        //have to use the silly return a copy thing; if you don't, since it's static, changes made in inspector will affect
        //all anim curves that equal the static reference.
        /// <summary>
        /// logarithmic type curve from 0 to 1
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve EaseOutCurve()
        {
            AnimationCurve toReturn = new AnimationCurve();
            toReturn.CopyFrom(new AnimationCurve(EaseOut1(), EaseOut2()));
            return toReturn;
        }
        /// <summary>
        /// initial burst, before slowing down to zero
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve EarlyPeakToZeroCurve()
        {
            AnimationCurve toReturn = new AnimationCurve();
            toReturn.CopyFrom(new AnimationCurve(EarlyPeak1(), EarlyPeak2()));
            return toReturn;
        }
        /// <summary>
        /// oscillates rapidly between max and max negative, before winding down back to zero.
        /// </summary>
        /// <returns></returns>
        private static AnimationCurve JostleCurve()
        {
            AnimationCurve toReturn = new AnimationCurve();
            toReturn.CopyFrom(new AnimationCurve(JostleKeyframes));
            return toReturn;
        }

        public static AnimationCurve EarlyPeakToZero => EarlyPeakToZeroCurve();
        public static AnimationCurve Jostle => JostleCurve();
        public static AnimationCurve EaseOut => EaseOutCurve();
        #region EaseOut
        private static Keyframe EaseOut1()
        {
            Keyframe toReturn = new Keyframe(0, 0, 2, 2, 0, 0);

            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;

            return toReturn;
        }
        private static Keyframe EaseOut2()
        {
            Keyframe toReturn = new Keyframe(1, 1, 0, 0, 0, 0);

            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;

            return toReturn;
        }
        #endregion
        #region EarlyPeakToZero
        private static Keyframe EarlyPeak1()
        {
            Keyframe toReturn = new Keyframe(0, 0, 5.77f, 5.77f, 0, 0.02f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe EarlyPeak2() 
        {
            Keyframe toReturn = new Keyframe(1, 0, -1.35f, -1.35f, 0, 0.02f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        #endregion
        #region Jostle
        private static Keyframe Jostle1()
        {
            Keyframe toReturn = new Keyframe(0, 0, 0, 0, 0, 0);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle2() 
        {
            Keyframe toReturn = new Keyframe(0.05f, 1, 0.05f, 0.05f, 1, 0.33f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle3()
        {
            Keyframe toReturn = new Keyframe(0.15f, -1, -0.01f, -0.01f, 0.33f, 1);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle4()
        {
            Keyframe toReturn = new Keyframe(0.3f, 1, -0.003f, -0.003f, 0.8f, 0.5f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None; 
            return toReturn;
        }
        private static Keyframe Jostle5() 
        {
            Keyframe toReturn = new Keyframe(0.4f, -1, 0.03f, 0.03f, 0.33f, 0.45f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle6() 
        {
            Keyframe toReturn = new Keyframe(0.5f, 0.62f, 0.01f, 0.01f, 1, 0.33f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle7() 
        {
            Keyframe toReturn = new Keyframe(0.7f, -0.5f, -0.04f, -0.04f, 1, 0.33f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle8() 
        {
            Keyframe toReturn = new Keyframe(0.85f, 0.2f, -0.1f, -0.1f, 1, 0.33f);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe Jostle9()
        {
            Keyframe toReturn = new Keyframe(1, 0, -0.03f, -0.03f, 1, 0);
            toReturn.tangentMode = 0;
            toReturn.weightedMode = WeightedMode.None;
            return toReturn;
        }
        private static Keyframe[] JostleKeyframes =
        {
            Jostle1(), Jostle2(), Jostle3(), Jostle4(), Jostle5(), Jostle6(), Jostle7(), Jostle8(), Jostle9()
        };
        #endregion
    }
}
