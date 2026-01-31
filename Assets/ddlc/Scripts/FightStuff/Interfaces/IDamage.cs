
namespace RenCSharp.Combat.Interfaces
{
    public interface IDamage 
    {
        /// <summary>
        /// Take Damage from an attack.
        /// </summary>
        /// <param name="f">FLOAT The amount of damage that should be inflicted.</param>
        /// <param name="DoT">BOOL Whether or not the damage is being dealt over time, or in bulk
        public void TakeDamage(object f, object DoT);
        /// <summary>
        /// For making funny enemy scaling in difficulty
        /// </summary>
        /// <returns>Percentage resistance to damage</returns>
        public float Resistance();
    }
}
