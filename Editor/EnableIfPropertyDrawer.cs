#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
using UnityEditor;
using UnityEngine;
namespace AvatarPluginForUnity.Editor
{
    public class EnableIfAttribute : PropertyAttribute
    {
        #region Fields
        public string boolPropertyName { get; private set; }
        public string enableTargetPropertyName { get; private set; }
        #endregion

        public EnableIfAttribute(string boolPropertyName, string enableTargetPropertyName)
        {
            this.boolPropertyName = boolPropertyName;
            this.enableTargetPropertyName = enableTargetPropertyName;
        }
    }

    namespace UnityEditor
    {
        [CustomPropertyDrawer(typeof(EnableIfAttribute))]
        public class EnableIfPropertyDrawer : PropertyDrawer
        {
            #region Fields
            EnableIfAttribute enableIf;
            SerializedProperty boolField;
            SerializedProperty enableTargetField;
            #endregion

            private bool EnableMe(SerializedProperty property)
            {
                enableIf = attribute as EnableIfAttribute;

                string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, enableIf.boolPropertyName) : enableIf.boolPropertyName;
                boolField = property.serializedObject.FindProperty(path);

                path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, enableIf.enableTargetPropertyName) : enableIf.enableTargetPropertyName;
                enableTargetField = property.serializedObject.FindProperty(path);

                if (boolField == null)
                {
                    Debug.LogError("Cannot find bool property with name: " + path);
                    return true;
                }

                if (enableTargetField == null)
                {
                    Debug.LogError("Cannot find enable target property with name: " + path);
                    return true;
                }
                return boolField.boolValue;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (!EnableMe(property))
                {
                    GUI.enabled = false;
                }
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
#endif