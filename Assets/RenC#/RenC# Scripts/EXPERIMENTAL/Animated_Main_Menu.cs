using UnityEngine;

namespace EXPERIMENTAL
{
    [RequireComponent(typeof(Animator))]
    public class Animated_Main_Menu : MonoBehaviour
    {
        [SerializeField] private float minTime = 5, maxTime = 10;
        [SerializeField,Min(1)] private int animatorIntRange;
        [SerializeField] private string animatorIntParameterName, animatorTriggerParameterName;
        private float desTime, t;
        private Animator me;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            me = GetComponent<Animator>();
            desTime = Random.Range(minTime, maxTime);
            t = 0;
        }

        // Update is called once per frame
        void Update()
        {
            t += Time.deltaTime;
            if(t >= desTime)
            {
                t = 0;
                desTime = Random .Range(minTime, maxTime);
                me.SetInteger(animatorIntParameterName, Random.Range(0, animatorIntRange));
                me.SetTrigger(animatorTriggerParameterName);
            }
        }
    }
}
