using RenCSharp.Combat.Interfaces;
using System;

namespace RenCSharp.Combat.Enemies
{
    [Serializable]
    public abstract class Projectile_CustomUpdate : IEditorValidate, IEnable, IRemovableObject
    {
        public abstract void OnEnable();
        public abstract void OnRemove(bool playerTurn);
        public abstract void OnEditorValidate();
        public abstract void UpdateBehavior();
    }
}
