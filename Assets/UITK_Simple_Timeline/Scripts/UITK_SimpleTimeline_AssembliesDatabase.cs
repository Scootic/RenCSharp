using UnityEngine;
using System;
using System.Reflection;
using UnityEditorInternal;
using System.Linq;
using System.Collections.Generic;
namespace UITK_SimpleTimeline
{
    [CreateAssetMenu(menuName = "UITK_SimpleTimeline/Assembly Database")]
    public class UITK_SimpleTimeline_AssembliesDatabase : ScriptableObject
    {
        private static Assembly[] assemblies = new Assembly[0];
        public static List<Type> GetValidTimelineCurveTypes
        {
            get
            {
                List<Type> toReturn = new();
                if(assemblies == null)
                {
                    Debug.LogWarning("No valid timeline curve types could be found because there's no AssemblyDatabase to parse through." +
                        " Make sure to create one and assign relevant assemblies. (CreateAssetMenu->UITK_SimpleTimeline->Assembly Database.)"
                         + " You should only need one.");
                    return null;
                }

                foreach(Assembly a in assemblies)
                {
                    Type[] validTypes = a.GetTypes().Where(a => a.IsSubclassOfGenericType(typeof(TypedTimelineCurve<,>)) && !a.IsAbstract).ToArray();
                    //Debug.Log($"Valid Type Length in Assembly {a.FullName}: {validTypes.Length}");
                    foreach(Type t in validTypes)
                    {
                        toReturn.Add(t);
                    }
                }
                
                return toReturn;
            }
        }

        [SerializeField] private AssemblyDefinitionAsset[] assemblyAssets;
        [SerializeField] private bool debug = false;

        void Do()
        {
            if (assemblies.Length == assemblyAssets.Length) //if we have matching length already, check for discrepencies.
            {
                for(int i = 0; i < assemblies.Length; i++)
                {
                    string assemblyName = JsonUtility.FromJson<RealAssemblyName>(assemblyAssets[i].text).name;
                    if (assemblies[i].GetName().Name == assemblyName) continue;
                    assemblies[i] = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
                }

                return;
            }
            assemblies = new Assembly[assemblyAssets.Length];

            for(int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    string assemblyName = JsonUtility.FromJson<RealAssemblyName>(assemblyAssets[i].text).name;

                    Assembly t = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
                    assemblies[i] = t;
                }
                catch
                {
                    Debug.LogError($"Super evil error occured when trying to get an assembly from AssemblyAssetDef: " +
                        $"{(assemblyAssets[i] != null ? assemblyAssets[i].name : "null asset :(")}!" +
                        $" Bailing from assembly gathering method and discarding any assemblies we may have gathered!");
                    assemblies = new Assembly[0];
                    return;
                } 
            }

            if (!debug) return; 
            
            Debug.Log($"Found some assemblies! Assembly count: {assemblies.Length}");
            foreach(Assembly a in assemblies)
            {
                Debug.Log("Assembly Name: " + a.FullName);
            }
            
        }

        private void OnValidate()
        {
            Do();
        }

        private void OnEnable()
        {
            Do();
        }

        [Serializable]
        private class RealAssemblyName
        {
            public string name;
        }
    }
}
