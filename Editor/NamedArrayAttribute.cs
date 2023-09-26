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
using System.Linq;
using UnityEditor;
using UnityEngine;
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
namespace AvatarPluginForUnity.Editor
{
    public class NamedArrayAttribute : PropertyAttribute
    {
        public readonly string[] names;
        public NamedArrayAttribute(string[] names) { this.names = names; }

    }

    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class StringInListDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            try
            {
                var path = property.propertyPath;
                int pos = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
                EditorGUI.PropertyField(rect, property, new GUIContent(ObjectNames.NicifyVariableName(((NamedArrayAttribute)attribute).names[pos])), true);
            }
            catch
            {
                EditorGUI.PropertyField(rect, property, label, true);
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif

