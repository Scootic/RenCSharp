
using System;
using System.Reflection;

namespace RenCSharp.EXPERIMENTAL
{
    public class Help_CMD 
    {
        public static void Help()
        {
            string msg = "List of available commands:";

            foreach(Type T in RenConsole.AllConsoleAccessibleTypes)
            {
                MethodInfo[] methodInfos = T.GetMethods(BindingFlags.Public & BindingFlags.Static);
                if (methodInfos.Length == 0) continue;
                msg += $"\n{T.Name}: ";
                foreach(MethodInfo mi in methodInfos)
                {
                    msg += $"{mi.Name}, ";
                }

                if (msg.EndsWith(','))
                {
                    char[] glum = msg.ToCharArray();
                    glum[msg.Length - 1] = '.';
                    msg = glum.ToString();
                }
            }

            RenConsole.Log(msg, false);
        }

        //public static void help() { Help(); }
    }
}
