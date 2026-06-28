using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace RenCSharp.EXPERIMENTAL
{
    public class RenConsole
    {
        private static readonly Assembly consoleAssembly = Assembly.GetAssembly(typeof(RenConsole));
        private static Type[] allTypes = consoleAssembly.GetTypes().Where(t => t.IsClass).ToArray();
        private static readonly string[] consoleLogs = new string[20];
        public static string[] ConsoleLogs => consoleLogs;
        public static RenConsole instance = new RenConsole();
        public static Type[] AllConsoleAccessibleTypes => allTypes;

        public static void Log(string message, bool debugLog = true)
        {
            for (int i = 0; i < consoleLogs.Length; i++)
            {
                if (consoleLogs[i] == null || consoleLogs[i] == "") { consoleLogs[i] = message; break; }
            }

            if (consoleLogs[consoleLogs.Length - 1] != "") //if the list is full.
            {
                for(int i = 0; i < consoleLogs.Length - 1; i++)
                {
                    consoleLogs[i] = consoleLogs[i + 1];
                }
                consoleLogs[consoleLogs.Length - 1] = message;
            }

            if (debugLog) Debug.Log(message);
            //reinit view or some shite
        }

        public static void ParseCommand(string cmd)
        {
            string[] split = Regex.Split(cmd, "[ ,]");//0 should be function name, 1 and after are arguments.
            List<object> arguments = new();
            for(int i = 1; i < split.Length; i++)
            {
                arguments.Add(split[i]);
            }
            MethodInfo method;
            foreach(Type T in allTypes)
            {
                method = T.GetMethod(split[0], BindingFlags.Static & BindingFlags.Public);
                if(method != null)
                {
                    method.Invoke(instance, arguments.ToArray());
                    break;
                }
            }
        }
    }
}
