using Coffee.UIExtensions;
using RenCSharp.EXPERIMENTAL;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RenCSharp
{
    [RequireComponent(typeof(UIParticle))]
    public sealed class UIParticle_Helper : MonoBehaviour, IRemovableObject
    {
        private ParticleToken me;
        public ParticleToken SetMyParticleToken { set { me = value; } }
        public void OnRemove(bool b = false)
        {
            Event_Bus.TryFireSingleObjEvent("RemoveParticleFromList", (object)me);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIParticle_Helper))]
    public class UIParticle_Helper_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Remove from OF"))
            {
                Object_Factory.RemoveObject((target as UIParticle_Helper).gameObject.name);
            }
        }
    }
#endif
}
