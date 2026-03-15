using RenCSharp.Combat.Interfaces;
using System;
using UnityEngine;

namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_CustomUpdate : IEditorValidate
    {
        public abstract void OnEditorValidate();
        public abstract void UpdateBehavior();
    }
}
