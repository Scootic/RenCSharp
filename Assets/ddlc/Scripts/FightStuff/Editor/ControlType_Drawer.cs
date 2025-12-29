using UnityEditor;
namespace RenCSharp.Combat
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
