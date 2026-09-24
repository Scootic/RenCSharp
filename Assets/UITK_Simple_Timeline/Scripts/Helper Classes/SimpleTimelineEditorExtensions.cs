#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UObject = UnityEngine.Object;
namespace UITK_SimpleTimeline
{
    public static class SimpleTimelineEditorExtensions 
    {
        /// <summary>
        /// Set an asset to be addressable, returning the new AssetReference.
        /// </summary>
        /// <param name="obj">The object you want to be addressable.</param>
        /// <param name="intendedAssetGroupName">Optional string so you can set the asset's group.</param>
        /// <returns>The AssetReference that will load the given object.</returns>
        public static AssetReference SetObjectAddressable(this UObject obj, string intendedAssetGroupName = "")
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            AddressableAssetGroup intendedAssetGroup = intendedAssetGroupName == "" ? settings.DefaultGroup : settings.FindGroup(intendedAssetGroupName);
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(assetGUID, intendedAssetGroup);
            entry.address = assetPath;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved | AddressableAssetSettings.ModificationEvent.EntryCreated, entry, true);
            AssetDatabase.SaveAssets();

            AssetReference toReturn = settings.CreateAssetReference(entry.guid);

            return toReturn;
        }

        /// <summary>
        /// Extract the SerializedProperty out of a PropertyField using Reflection, because Unity is so racist you can't just do that.
        /// </summary>
        /// <param name="field">The PropertyField you want to get the SerializedProperty out of.</param>
        /// <returns>The bound SerializedProperty (m_SerializedProperty in the PropertyField class). Null if the process fails.</returns>
        public static SerializedProperty GetBoundProperty(this PropertyField field)
        {
            SerializedProperty toReturn;
            FieldInfo spField;
            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            try
            {
                spField = typeof(PropertyField).GetField("m_SerializedProperty", bf);
            }
            catch
            {
                Debug.LogWarning("FieldInfo couldn't grab the stinkin' m_SerializedProperty?!?!");
                return null;
            }
            try
            {
                toReturn = spField.GetValue(field) as SerializedProperty;
            }
            catch
            {
                Debug.LogWarning("Given PropertyField can't handle getting its m_SerializedProperty read?!");
                return null;
            }

            return toReturn;
        }
    }
}
#endif