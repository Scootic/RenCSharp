using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
namespace RenCSharp
{
    ///Author: Scootic Rowlann
    /// <summary>
    /// Abstract solution to make any abstract parent class work with the [SerializeReference] property in Unity. Spawns a dropdown menu
    /// at the top of the drawer to let the user select which child they want. Only able to check
    /// within T's assembly.
    /// </summary>
    /// <typeparam name="T">The class being displayed by PropertyDrawer</typeparam>
    public abstract class PolymorphicPropertyDrawer<T> : PropertyDrawer where T : class
    {
        /// <summary>
        /// Noted problem with this solution, it can only check the assembly of the parent class. Any other assembly that references
        /// the T assembly won't be included in the dropdown menu's search of valid types.
        /// </summary>
        protected static Assembly typeAssembly = Assembly.GetAssembly(typeof(T));
        protected static Type[] allTChildren;
        protected abstract string DropDownMenuName();
        [SerializeReference] protected SerializedProperty m_SE;

        /// <summary>
        /// By default, will render the property drawer one line below a childType selector dropdown menu.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            m_SE = property;
            Rect dDownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            DropDownMenu(dDownRect, property);
            Rect newPos = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height);
            EditorGUI.PropertyField(newPos, property, new GUIContent(property.type), true);
            EditorGUI.EndProperty();
        }

        protected virtual void DropDownMenu(Rect pos, SerializedProperty property)
        {
            if (EditorGUI.DropdownButton(pos, new GUIContent(DropDownMenuName()), FocusType.Keyboard))
            {
                //set all the types that are children of passed in parent type from class declaration
                allTChildren = typeAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(T))).ToArray();
                GenericMenu menu = new GenericMenu();

                foreach (Type childType in allTChildren)
                {
                    T instance = Activator.CreateInstance(childType) as T;
                    menu.AddItem(new GUIContent(instance.ToString()),
                        !(property.managedReferenceValue == null) ? property.managedReferenceValue.ToString() == instance.ToString() : false,
                        SetChildType, instance);
                }

                menu.DropDown(pos);
            }
        }

        protected virtual void SetChildType(object obj)
        {
            T selectedType = obj as T;
            m_SE.managedReferenceValue = selectedType;
            m_SE.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float returner = Drawer_Helper.PropertyHeight(property);
            returner += EditorGUIUtility.singleLineHeight * 2;
            return returner;
        }
    }
}
