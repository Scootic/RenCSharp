using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
namespace RenCSharp
{
    /// <summary>
    /// Provides various ways to duplicate values from one component onto another.
    /// </summary>
    public static class ComponentCopier
    {
        /// <summary>
        /// Probably can only copy values that are public/serializefielded. use carefully.
        /// </summary>
        /// <typeparam name="T">The type of component being copied.</typeparam>
        /// <param name="destination">The recipient of data</param>
        /// <param name="source">The data being taken</param>
        /// <returns>The component, now copied.</returns>
        public static T CopyValuesJSON<T>(this T destination, T source) where T : Component 
        {
            var json = JsonUtility.ToJson(source);
            JsonUtility.FromJsonOverwrite(json, destination);
            return destination;
        }
        /// <summary>
        /// copies a generic component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyValuesThroughReflection<T>(this T destination, T source) where T : Component
        {
            Type type = typeof(T);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

            FieldInfo[] fields = type.GetFields(flags)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null || field.IsPrivate).ToArray(); //grab every single body?

            PropertyInfo[] propertyInfos = type.GetProperties(flags);

            foreach(PropertyInfo property in propertyInfos)
            {
                if (property.CanWrite && property.CanRead)
                {
                    try
                    {
                        if (property.Name == "materials" || property.Name == "material")
                        {
                            Debug.LogWarning("cannot access materials from prefabs, so safety measure says no can do!");
                            continue;
                        }
                        property.SetValue(destination, property.GetValue(source, null), null);
                    }
                    catch(Exception ex)
                    {
                        //????
                        Debug.LogError($"can't set that '{property.Name}' value, dingleberry! {ex.Message}");
                    }
                }
            }

            foreach(FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
                Debug.Log("Field: " + field.Name + ": " + field.FieldType);
            }

            return destination as T;
        }
        /// <summary>
        /// copies a generic struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CopyStructValuesThroughReflection<T>(this T destination, T source) where T : struct
        {
            Type type = typeof(T);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default;

            FieldInfo[] fields = type.GetFields(flags)
                .Where(field => field.IsPublic || field.GetCustomAttribute<SerializeField>() != null || field.IsPrivate).ToArray(); //grab every single body?

            PropertyInfo[] propertyInfos = type.GetProperties(flags);

            foreach(PropertyInfo property in propertyInfos)
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

            foreach(FieldInfo field in fields)
            {
                field.SetValue(destination, field.GetValue(source));
                Debug.Log("Field: " + field.Name + ": " + field.FieldType);
            }

            return destination;
        }
        /// <summary>
        /// Goes through every single sub module and reflects it, because the ParticleSystem component is actually really evil.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static ParticleSystem CopyParticleSystem(this ParticleSystem destination, ParticleSystem src)
        {
            destination.main.CopyStructValuesThroughReflection(src.main);
            //the emission module is particularly buns?!?!?
            destination.emission.CopyStructValuesThroughReflection(src.emission);
            ParticleSystem.Burst[] veryStupidTempArray = new ParticleSystem.Burst[src.emission.burstCount];
            src.emission.GetBursts(veryStupidTempArray);
            destination.emission.SetBursts(veryStupidTempArray);

            destination.shape.CopyStructValuesThroughReflection(src.shape);
            destination.velocityOverLifetime.CopyStructValuesThroughReflection(src.velocityOverLifetime);
            destination.limitVelocityOverLifetime.CopyStructValuesThroughReflection(src.limitVelocityOverLifetime);
            destination.inheritVelocity.CopyStructValuesThroughReflection(src.inheritVelocity);
            destination.lifetimeByEmitterSpeed.CopyStructValuesThroughReflection(src.lifetimeByEmitterSpeed);
            destination.forceOverLifetime.CopyStructValuesThroughReflection(src.forceOverLifetime);
            destination.colorOverLifetime.CopyStructValuesThroughReflection(src.colorOverLifetime);
            destination.colorBySpeed.CopyStructValuesThroughReflection(src.colorBySpeed);
            destination.sizeOverLifetime.CopyStructValuesThroughReflection(src.sizeOverLifetime);
            destination.sizeBySpeed.CopyStructValuesThroughReflection(src.sizeBySpeed);
            destination.rotationOverLifetime.CopyStructValuesThroughReflection(src.rotationOverLifetime);
            destination.rotationBySpeed.CopyStructValuesThroughReflection(src.rotationBySpeed);
            destination.externalForces.CopyStructValuesThroughReflection(src.externalForces);
            destination.noise.CopyStructValuesThroughReflection(src.noise);
            destination.collision.CopyStructValuesThroughReflection(src.collision);
            destination.trigger.CopyStructValuesThroughReflection(src.trigger);
            destination.subEmitters.CopyStructValuesThroughReflection(src.subEmitters);
            destination.textureSheetAnimation.CopyStructValuesThroughReflection(src.textureSheetAnimation);
            destination.lights.CopyStructValuesThroughReflection(src.lights);
            destination.trails.CopyStructValuesThroughReflection(src.trails);
            destination.customData.CopyStructValuesThroughReflection(src.customData);

            return destination;
        }
    }
}
