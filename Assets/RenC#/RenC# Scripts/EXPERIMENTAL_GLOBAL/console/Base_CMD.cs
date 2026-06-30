namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// Base class for commands that can be used by the RenConsole.
    /// 
    /// For good practice: make sure your methods are non-public and static, those are the binding flags that the console
    /// looks through when selecting a method to fire. Make sure to have all method arguments be strings, and name them
    /// so that the real type you're looking for is clear. (For example, string_flagName as an argument, or int_newValue as an argument.)
    /// </summary>
    public abstract class Base_CMD
    {
        
    }
}
