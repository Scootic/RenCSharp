using UnityEngine;
using System.Collections;
namespace RenCSharp.Combat
{
    public class Dodge : ControlType
    {
        private bool midDodge = false;
        [SerializeField] private AnimationCurve dodgeCurve;
        [SerializeField, Min(1)] private float distanceFromCenter = 50;
        [SerializeField, Min(0.1f)] private float timeToDodge = 0.5f;
        private Vector3 originPosition;
        private float t = 0;
        private Coroutine dodgeRoutine;
        protected override void MovementEffect(Vector2 dir)
        {
            Debug.Log("Received Dodge Input!~");
            if (midDodge) return;
            Debug.Log("Should be dodgering now!");
            midDodge = true;
            float x = 0;
            float y = 0;

            if (dir.x > 0.5f) x = 1;
            else if (dir.x < -0.5f) x = -1;

            if (dir.y > 0.5f) y = 1;
            else if (dir.y < -0.5f) y = -1;

            Vector3 desPos = originPosition + new Vector3(x * distanceFromCenter,y * distanceFromCenter,0);

            dodgeRoutine = Fight_Manager.FM.StartCoroutine(DodgeAnimation(desPos));
        }

        public override void EnterControl()
        {
            base.EnterControl();
            playerObj.transform.localPosition = Vector3.zero;
            originPosition = playerObj.transform.position;
            midDodge = false;
        }

        public override void ExitControl()
        {
            if (dodgeRoutine != null) Fight_Manager.FM.StopCoroutine(dodgeRoutine);
            base.ExitControl();
        }

        private IEnumerator DodgeAnimation(Vector3 desPos)
        {
            t = 0;
            playerObj.transform.position = originPosition;
            while (t <= timeToDodge)
            {
                t += Time.deltaTime;
                float eval = t / timeToDodge;

                playerObj.transform.position = Vector3.Lerp(originPosition, desPos, dodgeCurve.Evaluate(eval));

                yield return null;
            }
            playerObj.transform.position = originPosition;
            midDodge = false;
        }

        protected override Color PlayerColor()
        {
            return Color.magenta;
        }
        public override string ToString()
        {
            return "Dodge";
        }
    }
}
