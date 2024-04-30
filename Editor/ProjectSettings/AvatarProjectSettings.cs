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
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;
using GLTFast;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [InitializeOnLoad]
    public class AvatarProjectSettings
    {

        /// <summary>
        /// Initializes the <see cref="AvatarProjectSettings" /> class.
        /// </summary>
        static AvatarProjectSettings()
        {
            CallProjectSettings();
        }

        /// <summary>
        /// Calls the project settings.
        /// </summary>
        private static void CallProjectSettings()
        {
            //For All
            AddBuiltInShaders("glTF/PbrMetallicRoughness");
            if (RenderPipelineUtils.RenderPipeline.Equals(GLTFast.RenderPipeline.Universal))
                AddBuiltInShaders("Shader Graphs/glTF-pbrMetallicRoughness");

            //For Android
            Rendering();
            Configuration();

        }

        /// <summary>
        /// Renderings this instance.
        /// </summary>
        private static void Rendering()
        {
            //Set NormalMapEncodin to XYZ
            PlayerSettings.SetNormalMapEncoding(NamedBuildTarget.Android, NormalMapEncoding.XYZ);
        }

        /// <summary>
        /// Configurations this instance.
        /// </summary>
        private static void Configuration()
        {
            //Set Android Build Configuration
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel28)
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28;

            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

            if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        }

        /// <summary>
        /// Builts the in shaders.
        /// </summary>
        private static void AddBuiltInShaders(string shaderName)
        {
            //Set BuiltIn Shaders
            var shader = Shader.Find(shaderName);
            if (shader == null)
                return;
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
            bool hasShader = false;
            for (int i = 0; i < arrayProp.arraySize; ++i)
            {
                var arrayElem = arrayProp.GetArrayElementAtIndex(i);
                if (shader == arrayElem.objectReferenceValue)
                {
                    hasShader = true;
                    break;
                }
            }
            if (!hasShader)
            {
                int arrayIndex = arrayProp.arraySize;
                arrayProp.InsertArrayElementAtIndex(arrayIndex);
                var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
                arrayElem.objectReferenceValue = shader;
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
            
        }
    }
}
#endif