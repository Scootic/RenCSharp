#if UNITY_EDITOR
using UnityEditor;
namespace RenCSharp.Combat.Player
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