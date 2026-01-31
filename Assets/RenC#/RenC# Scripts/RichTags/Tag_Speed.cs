using EXPERIMENTAL;
using TMPro;
namespace RenCSharp.Tags
{
    public class Tag_Speed : Base_Tag
    {
        /// <summary>
        /// Sets the script manager's speed to temporarily be a different value.
        /// </summary>
        /// <param name="mesh">UselessAHShiz</param>
        /// <param name="value">FLOAT the value that SM will set text speed to.</param>
        protected static void Speed(TextMeshProUGUI mesh, string value)
        {
            if (float.TryParse(value, out float valley))
            {
                valley = 1 / (valley * 10);
                Event_Bus.TryFireDoubleObjEvent("SMSpeed", (object)valley, (object)false);
            }
        }
        /// <summary>
        /// Make SM go back to original speed.
        /// </summary>
        protected static void EndSpeed()
        {
            Event_Bus.TryFireDoubleObjEvent("SMSpeed", (object)0f, (object)true);
        }
    }
}
