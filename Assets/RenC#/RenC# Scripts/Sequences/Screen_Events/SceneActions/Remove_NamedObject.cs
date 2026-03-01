using UnityEngine;

namespace RenCSharp.Sequences
{
    public class Remove_NamedObject : Screen_Event
    {
        [SerializeField] private string objectName = "object";

        public override void DoEvent()
        {
            Object_Factory.RemoveObject(objectName);
        }

        public override string ToString()
        {
            return "Remove Named Object";
        }
    }
}
