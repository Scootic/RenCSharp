
namespace RenCSharp
{
    /// <summary>
    /// An interface to let the user replace Screen_Events that they consider to be deprecated with updated replacement versions.
    /// Since it's an interface, the method called by the class can take in data that's stored in the deprecated version
    /// to be used however the user seems fit.
    /// Obviously, the name implies that this is meant to be used for Screen_Events, but the typing is generic to sidestep
    /// pointless assembly references/spaghetti and to let the user do whatever they want.
    /// </summary>
    /// <typeparam name="R">The type that the replacement will be. The only requirement is that it's a class.</typeparam>
    public interface IDeprecatedReplaceable<R> where R : class
    {
        /// <summary>
        /// Replaces an event you deem to be deprecated with a preferred replacement.
        /// </summary>
        /// <returns>The replacment version of a new event type.</returns>
        public R Replacement();
    }
}
