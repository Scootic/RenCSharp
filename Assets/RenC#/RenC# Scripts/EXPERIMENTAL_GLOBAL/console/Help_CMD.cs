using System;
using System.Reflection;

namespace RenCSharp.EXPERIMENTAL
{
    public class Help_CMD : Base_CMD
    {
        protected static void Help()
        {
            string msg = "List of available commands:";

            foreach(Type T in RenConsole.AllConsoleAccessibleTypes)
            {
                MethodInfo[] methodInfos = T.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                if (methodInfos.Length == 0) continue;
                msg += $"\n{T.Name}: ";
                foreach(MethodInfo mi in methodInfos)
                {
                    ParameterInfo[] parameters = mi.GetParameters();
                    if(parameters.Length > 0)
                    {
                        msg += $"{mi.Name} -> ";
                        foreach(ParameterInfo p in parameters)
                        {
                            msg += $"{p.Name}, ";
                        }
                        msg += "; ";
                    }
                    else
                    {
                        msg += $"{mi.Name}; ";
                    }
                }

                if (msg.EndsWith(';'))
                {
                    char[] glum = msg.ToCharArray();
                    glum[msg.Length - 1] = '.';
                    msg = glum.ToString();
                }
            }

            RenConsole.Log(msg, LogSeverity.LogPositive, false);
        }

        //public static void help() { Help(); }
    }
}
