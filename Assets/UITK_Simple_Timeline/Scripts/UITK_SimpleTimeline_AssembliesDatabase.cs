using UnityEngine;
using System;
using System.Reflection;
using UnityEditorInternal;
namespace UITK_SimpleTimeline
{
    [CreateAssetMenu(menuName = "UITK_SimpleTimeline/Assembly Database")]
    public class UITK_SimpleTimeline_AssembliesDatabase : ScriptableObject
    {
        [SerializeField] private Assembly[] assemblies;
        [SerializeField] AssemblyDefinitionAsset basset;

        void Do()
        {
            
        }
    }
}
