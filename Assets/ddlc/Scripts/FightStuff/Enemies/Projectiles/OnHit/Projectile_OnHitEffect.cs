
using System;
using UnityEngine;
using RenCSharp.Combat.Interfaces;
using System.Collections;
namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_OnHitEffect : IEditorValidate
    {
        public abstract void OnEditorValidate();
        public abstract void OnHit(Collider other);

        [SerializeField, Min(0)] protected float hitcooldown = 0.2f;
        protected bool validOnHit = true;
        protected IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(hitcooldown);
            validOnHit = true;
        }
    }
}
