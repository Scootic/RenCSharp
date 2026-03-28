using UnityEngine;

namespace RenCSharp.Tags
{

    /// <summary>
    /// EXISTS SOLELY FOR TAGS TO BE ABLE TO DO cOROUTINES!
    /// </summary>
    public class TagRoutineHandler : MonoBehaviour
    {
        public static TagRoutineHandler TRH;
        [SerializeField] private bool debug = false;
        public bool Debug => debug;

        private void Awake()
        {
            if (TRH == null) TRH = this;
            else if (TRH != this) Destroy(this);
        }
    }
}
