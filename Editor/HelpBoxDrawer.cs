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
    [CustomPropertyDrawer( typeof(HelpBoxAttribute))]
    public class HelpBoxPropertyDrawer : PropertyDrawer
    {
        #region Fields
    
        private HelpBoxAttribute helpBoxAttribute { get { return attribute as HelpBoxAttribute; } }
        SerializedProperty comparedField;
        SerializedProperty property;
    
        #endregion
 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            return isAssetLocationStreamingAsset(property) ? GetHelpBoxHeight()*2 : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(isAssetLocationStreamingAsset(property)){
                var helpBoxPosition = EditorGUI.IndentedRect( position );
                helpBoxPosition.height = GetHelpBoxHeight();
                EditorGUI.HelpBox( helpBoxPosition, helpBoxAttribute.message, GetMessageType(helpBoxAttribute.type));
                position.y += GetHelpBoxHeight() + 20;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        private bool isAssetLocationStreamingAsset(SerializedProperty property){
            string path = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, helpBoxAttribute.comparedPropertyName) : helpBoxAttribute.comparedPropertyName;
            comparedField = property.serializedObject.FindProperty(path);
    
            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            switch (comparedField.type)
            {
                case "bool":
                    return comparedField.boolValue.Equals(helpBoxAttribute.comparedValue);
                case "Enum":
                    return comparedField.enumValueIndex.Equals((int)helpBoxAttribute.comparedValue);
                default:
                    Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
                    return true;
            }
        }

        public MessageType GetMessageType( HelpBoxAttribute.HelpBoxType type )
        {
            switch ( type )
            {
                case HelpBoxAttribute.HelpBoxType.Error:     return MessageType.Error;
                case HelpBoxAttribute.HelpBoxType.Info:      return MessageType.Info;
                case HelpBoxAttribute.HelpBoxType.None:      return MessageType.None;
                case HelpBoxAttribute.HelpBoxType.Warning:   return MessageType.Warning;
            }
            return 0;
        }

        public float GetHelpBoxHeight()
        {
            var style   = new GUIStyle( "HelpBox" );
            var content = new GUIContent( helpBoxAttribute.message );
            return Mathf.Max( style.CalcHeight( content, Screen.width - ( helpBoxAttribute.type != HelpBoxAttribute.HelpBoxType.None ? 53 : 21) ), 40);
        }
    }
}
#endif
