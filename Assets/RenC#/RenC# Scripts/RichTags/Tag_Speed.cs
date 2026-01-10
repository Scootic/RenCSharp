using EXPERIMENTAL;

namespace RenCSharp.Tags
{
    public class Tag_Speed : Base_Tag
    {
        protected static void Speed(string value)
        {
            if (float.TryParse(value, out float valley))
            {
                valley = 1 / (valley * 10);
                Event_Bus.TryFireDoubleObjEvent("SMSpeed", (object)valley, (object)false);
            }
        }

        protected static void EndSpeed()
        {
            Event_Bus.TryFireDoubleObjEvent("SMSpeed", (object)0f, (object)true);
        }
    }
}
