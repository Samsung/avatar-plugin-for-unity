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
using UnityEditor;
using UnityEngine;
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
namespace AvatarPluginForUnity.Editor
{
[CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        #region Fields
        DrawIfAttribute drawIf;
        SerializedProperty comparedField;
    
        #endregion
    
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.Draw){
                return 0f;
            }

            if (ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DrawExclude){
                return 0f;
            }

            return base.GetPropertyHeight(property, label);
        }
    
        private bool ShowMe(SerializedProperty property)
        {
            drawIf = attribute as DrawIfAttribute;

            string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName) : drawIf.comparedPropertyName;
            comparedField = property.serializedObject.FindProperty(path);
    
            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            switch (comparedField.type)
            {
                case "bool":
                    return comparedField.boolValue.Equals(drawIf.comparedValue);
                case "Enum":
                    return comparedField.enumValueIndex.Equals((int)drawIf.comparedValue);
                default:
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
            }
        }
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShowMe(property) && drawIf.disablingType != DrawIfAttribute.DisablingType.DrawExclude){
                EditorGUI.PropertyField(position, property, label);
            } 
            else if(!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DrawExclude){
                EditorGUI.PropertyField(position, property, label);
            }
            else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true;
            }
        }
    }
}
#endif