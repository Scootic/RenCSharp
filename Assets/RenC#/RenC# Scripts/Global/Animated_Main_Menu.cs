using UnityEngine;

namespace EXPERIMENTAL
{
    /// <summary>
    /// Does funny random animations on a main menu. Feel free to not use.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class Animated_Main_Menu : MonoBehaviour
    {
        [SerializeField, Tooltip("In seconds.")] private float minTime = 5, maxTime = 10;
        [SerializeField,Min(1), Tooltip("Set to be the same as how many possible states you have, probably.")] private int animatorIntRange;
        [SerializeField] private string animatorIntParameterName, animatorTriggerParameterName;
        private float desTime, t;
        private int prevRoll;
        private Animator me;
        
        void Start()
        {
            me = GetComponent<Animator>();
            desTime = Random.Range(minTime, maxTime);
            t = 0;
            prevRoll = animatorIntRange;
        }

        
        void Update()
        {
            t += Time.deltaTime;
            if(t >= desTime)
            {
                t = 0;
                desTime = Random .Range(minTime, maxTime);
                int roll = Random.Range(0, animatorIntRange);
                while (roll == prevRoll) roll = Random.Range(0, animatorIntRange);
                prevRoll = roll;
                me.SetInteger(animatorIntParameterName, roll);
                me.SetTrigger(animatorTriggerParameterName);
            }
        }
    }
}
