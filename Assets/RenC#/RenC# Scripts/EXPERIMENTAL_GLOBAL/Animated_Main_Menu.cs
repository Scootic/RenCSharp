using UnityEngine;

namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// Does funny random animations on a main menu. Feel free to not use.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class Animated_Main_Menu : MonoBehaviour
    {
        [SerializeField, Tooltip("In seconds.")] private float minTime = 5, maxTime = 10;
        [SerializeField, Min(0)] private int animatorMinRollInclusive = 0;
        [SerializeField,Min(1)] private int animatorMaxRollExclusive = 5;
        [SerializeField] private string animatorIntParameterName, animatorTriggerParameterName;
        private float desTime, t;
        private Animator me;
        
        void Start()
        {
            me = GetComponent<Animator>();
            desTime = Random.Range(minTime, maxTime);
            t = 0;
        }

        
        void Update()
        {
            t += Time.deltaTime;
            if(t >= desTime)
            {
                t = 0;
                desTime = Random .Range(minTime, maxTime);
                int roll = RandomHelper.NoRepeatRoll("mainmenuanimator", animatorMinRollInclusive, animatorMaxRollExclusive);
                me.SetInteger(animatorIntParameterName, roll);
                me.SetTrigger(animatorTriggerParameterName);
            }
        }
    }
}
