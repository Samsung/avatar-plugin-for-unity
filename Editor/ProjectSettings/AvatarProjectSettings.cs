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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

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
        /// The symbols
        /// </summary>
        private static readonly string[] Symbols = new string[] { /*"GLTFAST_SAFE"*/ };

        /// <summary>
        /// The shaders
        /// </summary>
        private static readonly string[] Shaders = new string[] { "glTF/PbrMetallicRoughness" };

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
            BuiltInShaders();

            //For Default(Window/Mac Etc..)
            //if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            //    ScriptCompilation(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));

            //For Android
            Rendering();
            Configuration();
            //ScriptCompilation(NamedBuildTarget.Android);
        }

        /// <summary>
        /// Renderings this instance.
        /// </summary>
        private static void Rendering()
        {
            //Set ColorSpace
            //PlayerSettings.colorSpace = ColorSpace.Linear;

            //Set GraphicsAPI to Auto
            //PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);

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
        /// Scripts the compilation.
        /// </summary>
        /// <param name="targetGroup">The target group.</param>
        private static void ScriptCompilation(NamedBuildTarget buildTarget)
        {
            //Set Script Compilation
            string definesString = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbols(buildTarget,string.Join(";", allDefines.ToArray()));
        }

        /// <summary>
        /// Builts the in shaders.
        /// </summary>
        private static void BuiltInShaders()
        {
            //Set BuiltIn Shaders
            foreach (var shaderName in Shaders)
            {
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
}
#endif