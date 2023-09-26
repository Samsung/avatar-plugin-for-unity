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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEditor.Editor" />
    [InitializeOnLoad]
    public class SetProjectSettings
    {
        /// <summary>
        /// The symbols
        /// </summary>
        static private readonly string[] Symbols = new string[] { "GLTFAST_SAFE" };

        /// <summary>
        /// The shaders
        /// </summary>
        static private readonly string[] Shaders = new string[] { "glTF/PbrMetallicRoughness" };

        /// <summary>
        /// Initializes the <see cref="SetProjectSettings" /> class.
        /// </summary>
        static SetProjectSettings()
        {
            CallProjectSettings();
        }

        /// <summary>
        /// Calls the project settings.
        /// </summary>
        static private void CallProjectSettings()
        {
            //For All
            BuiltInShaders();

            //For Default(Window/Mac Etc..)
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                ScriptCompilation(EditorUserBuildSettings.selectedBuildTargetGroup);

            //For Android
            Rendering();
            Configuration();
            ScriptCompilation(BuildTargetGroup.Android);
        }

        /// <summary>
        /// Renderings this instance.
        /// </summary>
        static private void Rendering()
        {
            //Set ColorSpace
            //PlayerSettings.colorSpace = ColorSpace.Linear;

            //Set GraphicsAPI to Auto
            //PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);

            //Set NormalMapEncodin to XYZ
            PlayerSettings.SetNormalMapEncoding(BuildTargetGroup.Android, NormalMapEncoding.XYZ);
        }

        /// <summary>
        /// Configurations this instance.
        /// </summary>
        static private void Configuration()
        {
            //Set Android Build Configuration
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel28)
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel28;

            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.ARM64)
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        }

        /// <summary>
        /// Scripts the compilation.
        /// </summary>
        /// <param name="targetGroup">The target group.</param>
        static private void ScriptCompilation(BuildTargetGroup targetGroup)
        {
            //Set Script Compilation
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            allDefines.AddRange(Symbols.Except(allDefines));
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
            targetGroup,
            string.Join(";", allDefines.ToArray()));
        }

        /// <summary>
        /// Builts the in shaders.
        /// </summary>
        static private void BuiltInShaders()
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