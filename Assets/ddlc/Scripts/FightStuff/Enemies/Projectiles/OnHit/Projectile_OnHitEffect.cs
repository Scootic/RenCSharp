
using System;
using UnityEngine;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_OnHitEffect : IEditorValidate
    {
        public abstract void OnEditorValidate();
        public abstract void OnHit(Collider other);
    }
}
