using Coffee.UIExtensions;
using EXPERIMENTAL;
using UnityEngine;

namespace RenCSharp
{
    [RequireComponent(typeof(UIParticle))]
    public class UIParticle_Helper : MonoBehaviour, IRemovableObject
    {
        private ParticleToken me;
        public ParticleToken SetMyParticleToken { set { me = value; } }
        public void OnRemove()
        {
            Event_Bus.TryFireSingleObjEvent("RemoveParticleFromList", (object)me);
        }
    }
}
