using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace RenCSharp.EXPERIMENTAL
{
    /// <summary>
    /// In game console class to do commands and log things. Kinda similar to tag parser, but this is in a different assembly,
    /// and there's no <> shenanigans. 
    /// </summary>
    public class RenConsole
    {
        private static readonly Assembly consoleAssembly = Assembly.GetAssembly(typeof(RenConsole));
        private static readonly Type[] allTypes = consoleAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Base_CMD))).ToArray();
        private static readonly LogContainer[] consoleLogs = new LogContainer[20]; //resize if you wanna see moar logs
        public static LogContainer[] ConsoleLogs => consoleLogs;
        public static readonly RenConsole instance = new RenConsole();
        public static Type[] AllConsoleAccessibleTypes => allTypes;
        public static Action UpdateLogsListView;

        public static void Log(string message, LogSeverity severity = LogSeverity.Log, bool debugLog = true)
        {
            LogContainer t = new();
            t.Message = message;
            t.Severity = severity;
            t.DateTime = DateTime.Now.ToString("G");

            for (int i = 0; i < consoleLogs.Length; i++)
            {
                if (consoleLogs[i].DateTime == "") 
                {
                    consoleLogs[i] = t; 
                    break; 
                }
            }

            if (consoleLogs[consoleLogs.Length - 1].DateTime != "") //if the list is full.
            {
                for(int i = 0; i < consoleLogs.Length - 1; i++)
                {
                    consoleLogs[i] = consoleLogs[i + 1];
                }

                consoleLogs[consoleLogs.Length - 1] = t;
            }

            UpdateLogsListView?.Invoke();

            if (!debugLog) return;
            
            switch (severity) 
            {
                case LogSeverity.LogWarning:
                    Debug.LogWarning(message);
                    break;
                case LogSeverity.LogError: 
                    Debug.LogError(message); 
                    break;
                default:
                    Debug.Log(message); 
                    break;
            }
        }

        public static void ParseCommand(string cmd)
        {
            string[] split = Regex.Split(cmd, "[ ,]");//0 should be function name, 1 and after are arguments.
            List<object> arguments = new();
            for(int i = 1; i < split.Length; i++)
            {
                arguments.Add(split[i] as object);
            }
            MethodInfo method;
            foreach(Type T in allTypes)
            {
                method = T.GetMethod(split[0], BindingFlags.Static | BindingFlags.NonPublic);
                if(method != null)
                {
                    try
                    {
                        method.Invoke(instance, arguments.ToArray());
                    }
                    catch(TargetParameterCountException)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        string errorLog = $"{method.Name} was expecting {parameters.Length} parameter(s). (";
                        foreach(ParameterInfo p in parameters)
                        {
                            errorLog += $"{p.Name}, ";
                        }
                        errorLog += ")";
                        Log(errorLog, LogSeverity.LogError);
                    }
                    return;
                }
            }
            Log($"No known command by the name of: {split[0]}. Make sure you pay attention to case sensitivity!", LogSeverity.LogWarning);
        }
    }
    [Serializable]
    public struct LogContainer
    {
        public string Message;
        public string DateTime;
        public LogSeverity Severity;
    }
    [Serializable]
    public enum LogSeverity
    {
        Log,
        LogWarning,
        LogError,
        LogPositive,
        Null
    }
}
