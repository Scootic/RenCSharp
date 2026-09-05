using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
namespace UITK_SimpleTimeline
{
    public static class SimpleTimelineExtensions
    {
        /// <summary>
        /// copies a class. to be used with serializereference probby.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyClassValuesThroughReflection<T>(this T destination, T source) where T : class
        {
            Type type = typeof(T);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

            FieldInfo[] fields = type.GetFields(flags)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null || field.IsPrivate).ToArray(); //grab every single body?

            PropertyInfo[] propertyInfos = type.GetProperties(flags);

            foreach (PropertyInfo property in propertyInfos)
            {
                if (property.CanWrite && property.CanRead)
                {
                    try
                    {
                        property.SetValue(destination, property.GetValue(source, null), null);
                    }
                    catch
                    {
                        //????
                    }
                }
            }

            foreach (FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
                Debug.Log("Field: " + field.Name + ": " + field.FieldType);
            }

            return destination;
        }

        public static T[] CopyClassArrayValuesThroughReflection<T>(this T[] destination, T[] source) where T : class
        {
            for (int i = 0; i < destination.Length; i++)
            {
                destination[i].CopyClassValuesThroughReflection(source[i]);
            }

            return destination;
        }

        public static List<T> CopyClassListValuesThroughReflection<T>(this List<T> destination, List<T> source) where T : class
        {
            for(int i = 0; i < destination.Count; i++)
            {
                destination[i].CopyClassValuesThroughReflection(source[i]);
            }
            return destination;
        }

        public static List<TimelineCurve> CopyCurves(this List<TimelineCurve> destination, List<TimelineCurve> source)
        {
            List<Type> validCurveTypes = UITK_SimpleTimeline_AssembliesDatabase.GetValidTimelineCurveTypes;

            for(int i = 0; i < source.Count; i++)
            {
                Type theRealType = null;
                object stinker;
                foreach (Type t in validCurveTypes)
                {
                    stinker = Activator.CreateInstance(t);
                    if(stinker.ToString() == source.ToString())
                    {
                        theRealType = t;
                        break;
                    }
                }

                object curve = Activator.CreateInstance(theRealType);

                foreach (FieldInfo info in theRealType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    info.SetValue(curve, info.GetValue(source[i]));
                }


                foreach (PropertyInfo property in theRealType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic 
                    | BindingFlags.Instance))
                {
                    if (property.CanWrite && property.CanRead)
                    {
                        try
                        {
                            property.SetValue(destination, property.GetValue(source, null), null);
                        }
                        catch
                        {
                            //????
                        }
                    }
                }

                destination.Add(curve as TimelineCurve);
            }

            return destination;
        }

        public static float[] ToArray(this Vector3 v3)
        {
            return new float[] { v3.x, v3.y, v3.z };
        }

        public static Vector3 ToVector3(this float[] floatArray)
        {
            return floatArray.Length switch
            {
                0 => Vector3.zero,
                1 => new Vector3(floatArray[0], 0),
                2 => new Vector3(floatArray[0], floatArray[1]),
                _ => new Vector3(floatArray[0], floatArray[1], floatArray[2])
            };
        }

        public static bool HasNaN(this Vector3 v3)
        {
            return float.IsNaN(v3.x) || float.IsNaN(v3.y) || float.IsNaN(v3.z);
        }
        public static bool HasNaN(this Quaternion q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
        }
    }
}
