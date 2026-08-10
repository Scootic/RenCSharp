#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        protected static List<string> typeToStrings;
        protected abstract string DropDownMenuName();
        protected static T clipboardValue;
        protected static bool debugClipboard = true;

        protected SerializedProperty m_SE; //only used by IMGUI chicanery

        protected static GenericMenu ClipboardMenu;
        #region UIToolkit
        //below only used by UIToolkit chicanery
        /// <summary>
        /// Not the container of the whole element, just the contents? (Doesn't include the draggy area in lists/arrays?!?)
        /// </summary>
        protected VisualElement container;
        protected DropdownField polymorphDropDown;
        protected Button clipboardButton;
        /// <summary>
        /// same visual element as container?!?!?!?!?!? WHAT?!?!?
        /// </summary>
        protected PropertyField polymorphPropertyField;
        protected T propertyValue;
        protected string newVal;
        protected int typeIndex;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SetDropDownUITK(property);
            polymorphPropertyField = new();

            polymorphPropertyField.label = propertyValue != null ? propertyValue.ToString() : DropDownMenuName();
            polymorphPropertyField.BindProperty(property);
            polymorphPropertyField.focusable = false;
            polymorphPropertyField.pickingMode = PickingMode.Ignore;
            polymorphPropertyField.AddToClassList(PropertyField.inspectorElementUssClassName);
            polymorphPropertyField.RegisterCallback<PointerDownEvent>(evt =>
            {
                Debug.Log("clicked on the polymorph property field... field.");
            });

            clipboardButton = new Button();
            clipboardButton.style.height = 25;
            clipboardButton.style.width = 25;
            clipboardButton.text = "...";
            clipboardButton.RegisterCallback<PointerDownEvent>(evt =>
            {
                UpdateClipboardMenu(property);
                ClipboardMenu.ShowAsContext();
            });
            polymorphDropDown.Add(clipboardButton);

            container.Add(polymorphPropertyField);

            return container;
        }

        protected void SetDropDownUITK(SerializedProperty mySP)
        {
            allTChildren = typeAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(T))).ToArray(); //might be gross calling this every time a polymorph drawer is made
            typeToStrings = new List<string>(); //ToString()s, not really names
            foreach (Type t in allTChildren)
            {
                T instance = (T)Activator.CreateInstance(t);
                typeToStrings.Add(instance.ToString());
            }
            typeToStrings.Add(DropDownMenuName());
           
            container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.pickingMode = PickingMode.Ignore;
            container.focusable = false;

            propertyValue = mySP.boxedValue as T;
            int indexOfcur = typeToStrings.Count - 1;
            if (propertyValue != null) indexOfcur = typeToStrings.IndexOf(propertyValue.ToString());

            container.AddToClassList("unity-base-field");
            container.AddToClassList("unity-base-field__aligned");

            polymorphDropDown = new DropdownField(typeToStrings, indexOfcur);
            polymorphDropDown.style.flexDirection = FlexDirection.Row;

            polymorphDropDown.RegisterValueChangedCallback(evt =>
            {
                newVal = evt.newValue;
                typeIndex = typeToStrings.IndexOf(newVal);
                if (typeIndex == typeToStrings.Count - 1) return;
                mySP.managedReferenceValue = (T)Activator.CreateInstance(allTChildren[typeIndex]);
                mySP.serializedObject.ApplyModifiedProperties();
            });

            container.Add(polymorphDropDown);
            m_SE = mySP; //?
        }
        #endregion
        #region IMGUI
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
            Rect dDownRect = new Rect(position.x, position.y, position.width - 25, EditorGUIUtility.singleLineHeight);
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
                        delegate { SetChildType(instance, property); });
                }

                menu.DropDown(pos);
            }

            Rect clipboardRectButton = new Rect(pos.x + pos.width, pos.y, 25, EditorGUIUtility.singleLineHeight);

            if(GUI.Button(clipboardRectButton, "..."))
            {
                UpdateClipboardMenu(property);
                ClipboardMenu.ShowAsContext();
            }
        }
        #endregion
        protected virtual void SetChildType(object obj, SerializedProperty sp)
        {
            T selectedType = obj as T;
            sp.managedReferenceValue = selectedType;
            sp.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void CopyToClipboard(T value)
        {
            clipboardValue = value;
            if (debugClipboard)
            {
                allTChildren = typeAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(T))).ToArray();
                Type stinkyType = null;
                foreach(Type t in allTChildren)
                {
                    object activated = Activator.CreateInstance(t);
                    if(activated.ToString() == clipboardValue.ToString())
                    {
                        stinkyType = t;break;
                    }
                }
                Debug.Log($"Copying to clipboard! {clipboardValue}");
                foreach(FieldInfo info in stinkyType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    Debug.Log($"info:{info.Name}, clipboardValue:{info.GetValue(clipboardValue)}");
                }
            }
        }

        protected virtual void PasteFromClipboard(SerializedProperty sp)
        {
            Type tType = null; //absolutely gonna get replaced by the foreach loop. i hope...
            object stinker;
            allTChildren = typeAssembly.GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(T))).ToArray(); //hate this line >:)

            foreach (Type t in allTChildren) //get the REAL type of our clipboard value (even though it's stored as parent class T)
            {
                stinker = Activator.CreateInstance(t);
                if (stinker.ToString() == clipboardValue.ToString())
                {
                    tType = t;
                    break;
                }
            }

            object temp = Activator.CreateInstance(tType); //make a temporary object we'll throw all the values from clipboard into

            foreach (FieldInfo info in tType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if(debugClipboard)Debug.Log($"Setting some shi! info:{info.Name}, clipboardValue:{info.GetValue(clipboardValue)}");
                info.SetValue(temp, info.GetValue(clipboardValue));
            }

            sp.managedReferenceValue = temp; //????????????????????
            sp.serializedObject.ApplyModifiedProperties();
        }

        protected virtual void UpdateClipboardMenu(SerializedProperty sp)
        {
            debugClipboard = EditorPrefs.GetBool("PolymorphDebugClipboard", true);
            ClipboardMenu = new();
            try
            {
                string copyString = Regex.Replace(sp.managedReferenceValue.ToString(), "/", "-");
                ClipboardMenu.AddItem(new GUIContent($"Copy: {copyString}"), false, delegate
                {
                    T instance = sp.managedReferenceValue as T;
                    CopyToClipboard(instance);
                });
            }
            catch
            {
                ClipboardMenu.AddDisabledItem(new GUIContent("Can't copy nothing!"), false);
            }
            if (clipboardValue == null)
            {
                ClipboardMenu.AddDisabledItem(new GUIContent($"No value in clipboard."), false);
            }
            else if (clipboardValue as T == null)
            {
                ClipboardMenu.AddDisabledItem(new GUIContent($"Clipboard has incorrect type attached?!?"), false);
            }
            else
            {
                string s = Regex.Replace(clipboardValue.ToString(), "/", "-");
                ClipboardMenu.AddItem(new GUIContent($"Paste: {s}"), false, delegate
                {
                    PasteFromClipboard(sp);
                });
            }
            ClipboardMenu.AddSeparator("");
            ClipboardMenu.AddItem(new GUIContent("Debug Messages"), debugClipboard, delegate
            {
                debugClipboard = !debugClipboard;
                EditorPrefs.SetBool("PolymorphDebugClipboard", debugClipboard);
            });
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float returner = Drawer_Helper.PropertyHeight(property);
            returner += EditorGUIUtility.singleLineHeight * 2;
            return returner;
        }
    }
}
#endif