/* ****************************************************************
 *
 * Copyright 2023 Samsung Electronics All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 ******************************************************************/
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
using UnityEngine;
using UnityEditor;

namespace AvatarPluginForUnity.Editor
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEditor.PropertyDrawer" />
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        #region Fields
        /// <summary>
        /// The draw if
        /// </summary>
        DrawIfAttribute drawIf;
        /// <summary>
        /// The compared field
        /// </summary>
        SerializedProperty comparedField;
        #endregion

        /// <summary>
        /// Override this method to specify how tall the GUI for this field is in pixels.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        /// <returns>
        /// The height in pixels.
        /// </returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }

        /// <summary>
        /// Shows me.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        private bool IsTrue(SerializedProperty property)
        {
            drawIf = attribute as DrawIfAttribute;

            string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;
            comparedField = property.serializedObject.FindProperty(path);
    
            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return false;
            }

            if (path.Contains("Flags"))
            {
                if ((comparedField.enumValueFlag & (int)drawIf.comparedValue) != 0)
                    return true;
                else
                    return false;
            }
            else
                switch (comparedField.type)
                {
                    case "bool":
                        return comparedField.boolValue.Equals(drawIf.comparedValue);
                    case "Enum":
                        return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                    default:
                        Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                        return false;
                }
        }

        /// <summary>
        /// Override this method to make your own IMGUI based GUI for the property.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsTrue(property) && (drawIf.disablingType != DrawIfAttribute.DisablingType.DrawExclude))
            {
                EditorGUILayout.PropertyField(property, label);
            }
            else if (!IsTrue(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DrawExclude)
            {
                EditorGUILayout.PropertyField(property, label);            
            }
        }
    }
}
#endif