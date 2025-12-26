
using System.Collections.Generic;
using System;
namespace EXPERIMENTAL
{
    /// <summary>
    /// Bastardo Omnibus. contains dictionaries of every flavor of action you'd probably need.
    /// Instead of spaghetti dependencies on managers in some jerkoff AssDef, you just tell this 
    /// guy about it.
    /// </summary>
    public static class Event_Bus 
    {
        private static Dictionary<string, Action> voidEvents = new();
        private static Dictionary<string, Action<bool>> boolEvents = new();
        private static Dictionary<string, Action<int>> intEvents = new();
        private static Dictionary<string, Action<float>> floatEvents = new();
        private static Dictionary<string, Action<string>> stringEvents = new();

        /// <summary>
        /// Dangerous as shit. Removes ALL actions from ALL dictionaries. Important because these bastards are static. Gc moment.
        /// </summary>
        public static void PurgeEvents()
        {
            voidEvents.Clear();
            boolEvents.Clear();
            intEvents.Clear();
            floatEvents.Clear();
            stringEvents.Clear();
        }
        //every region basically has the same contents: a method for adding, firing, removing, and getting actions.
        #region VoidEvents
        public static void AddVoidEvent(string name, Action Event)
        {
            if(!voidEvents.ContainsKey(name))voidEvents.Add(name, Event);
        }

        public static bool TryFireVoidEvent(string name)
        {
            if (voidEvents.ContainsKey(name))
            {
                voidEvents[name]?.Invoke();
                return true;
            }
            else return false;
        }

        public static bool TryGetVoidEvent(string name, out Action Event)
        {
            Event = null;
            if (voidEvents.ContainsKey(name))
            {
                Event = voidEvents[name];
                return true;
            }
            else return false;
        }

        public static bool TryRemoveVoidEvent(string name)
        {
            if (voidEvents.ContainsKey(name))
            {
                voidEvents.Remove(name);
                return true;
            }
            else return false;
        }
        #endregion
        #region BoolEvents
        public static void AddBoolEvent(string name, Action<bool> Event)
        {
            if(!boolEvents.ContainsKey(name))boolEvents.Add(name, Event);
        }

        public static bool TryFireBoolEvent(string name, bool b)
        {
            if (boolEvents.ContainsKey(name))
            {
                boolEvents[name]?.Invoke(b);
                return true;
            }
            else return false;
        }

        public static bool TryGetBoolEvent(string name, out Action<bool> Event)
        {
            Event = null;
            if (boolEvents.ContainsKey(name))
            {
                Event = boolEvents[name];
                return true;
            }
            else return false;
        }

        public static bool TryRemoveBoolEvent(string name)
        {
            if (boolEvents.ContainsKey(name))
            {
                boolEvents.Remove(name);
                return true;
            }
            else return false;
        }
        #endregion
        #region IntEvents
        public static void AddIntEvent(string name, Action<int> Event)
        {
            if(!intEvents.ContainsKey(name))intEvents.Add(name, Event);
        }

        public static bool TryFireIntEvent(string name, int i)
        {
            if (intEvents.ContainsKey(name))
            {
                intEvents[name]?.Invoke(i);
                return true;
            }
            else return false;
        }

        public static bool TryGetIntEvent(string name, out Action<int> Event)
        {
            Event = null;
            if (intEvents.ContainsKey(name))
            {
                Event = intEvents[name];
                return true;
            }
            else return false;
        }

        public static bool TryRemoveIntEvent(string name)
        {
            if (intEvents.ContainsKey(name))
            {
                intEvents.Remove(name);
                return true;
            }
            else return false;
        }
        #endregion
        #region FloatEvents
        public static void AddFloatEvent(string name, Action<float> Event)
        {
            if(!floatEvents.ContainsKey(name))floatEvents.Add(name, Event);
        }

        public static bool TryFireFloatEvent(string name, float f)
        {
            if (floatEvents.ContainsKey(name))
            {
                floatEvents[name]?.Invoke(f);
                return true;
            }
            else return false;
        }

        public static bool TryGetFloatEvent(string name, out Action<float> Event)
        {
            Event = null;
            if (floatEvents.ContainsKey(name))
            {
                Event = floatEvents[name];
                return true;
            }
            else return false;
        }

        public static bool TryRemoveFloatEvent(string name)
        {
            if (floatEvents.ContainsKey(name))
            {
                floatEvents.Remove(name);
                return true;
            }
            else return false;
        }
        #endregion
        #region StringEvents
        public static void AddStringEvent(string name, Action<string> Event)
        {
            if(!stringEvents.ContainsKey(name))stringEvents.Add(name, Event);
        }

        public static bool TryFireStringEvent(string name, string s)
        {
            if (stringEvents.ContainsKey(name))
            {
                stringEvents[name]?.Invoke(s);
                return true;
            }
            else return false;
        }

        public static bool TryGetStringEvent(string name, out Action<string> Event)
        {
            Event = null;
            if (stringEvents.ContainsKey(name))
            {
                Event = stringEvents[name];
                return true;
            }
            else return false;
        }

        public static bool TryRemoveStringEvent(string name)
        {
            if (stringEvents.ContainsKey(name))
            {
                stringEvents.Remove(name);
                return true;
            }
            else return false;
        }
        #endregion
    }
}
