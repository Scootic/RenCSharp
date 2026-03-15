using UnityEngine;
using System;
using RenCSharp.Combat.Interfaces;
namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_DespawnType : IEditorValidate
    {
        public abstract void OnEditorValidate();
        public abstract void OnDespawn(bool playerTurn, Transform despawningTransform);
    }
}
