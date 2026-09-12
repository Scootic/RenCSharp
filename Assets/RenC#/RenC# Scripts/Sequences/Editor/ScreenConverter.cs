#if UNITY_EDITOR
using RenCSharp.Actors;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor.UIElements;

namespace RenCSharp.Sequences.Editor
{
    public class ScreenConverter : UxmlAttributeConverter<Screen> //not sure if i even need this?
    {
        static string ValueToString(object obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

        public override Screen FromString(string value)
        {
            Screen s = new Screen();
            object[] objects = value.Split('|');
            s.SetSpeaker = (Actor)Convert.ChangeType(objects[0], typeof(Actor), CultureInfo.InvariantCulture);
            s.SetDialog = objects[1] as string;
            s.ScreenActions = (List<Screen_Event>)Convert.ChangeType(objects[2], typeof(List<Screen_Event>), CultureInfo.InvariantCulture);
            return s;
        }

        public override string ToString(Screen s)
        {
            string format = "";
            format += ValueToString(s.Speaker + "|");
            format += s.Dialog + "|";
            format += ValueToString(s.ScreenActions);
            return format;
        }
    }
}
#endif