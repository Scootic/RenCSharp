#if UNITY_EDITOR
using UnityEditor;
using RenCSharp.Editor;
namespace RenCSharp.Combat.Player.Editor
{
    [CustomPropertyDrawer(typeof(ControlType))]
    public class ControlType_Drawer : PolymorphicPropertyDrawer<ControlType>
    {
        protected override string DropDownMenuName()
        {
            return "Choose Control Type";
        }
    }
}
#endif