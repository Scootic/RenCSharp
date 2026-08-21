using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
namespace UITK_SimpleTimeline
{
    public class DestroyableVisualElement
    {
        public Action DeleteMe;
        public VisualElement VE;
        private static GenericMenu deletThis;

        public DestroyableVisualElement(VisualElement ve)
        {
            deletThis = new();
            deletThis.AddItem(new GUIContent("Delete Element"), false, delegate
            {
                DeleteMe?.Invoke();
            });

            VE = ve;
            VE.RegisterCallback<PointerDownEvent>(evt =>
            {
                if(evt.button == 1)
                {
                    deletThis.ShowAsContext();
                }
            });
        }
    }
}
