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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using AvatarPluginForUnity.Editor;
#endif
using System;
using UnityEngine;
using static AvatarPluginForUnity.MaterialCombineOption;
using static AvatarPluginForUnity.MeshCombineOption;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AvatarPluginForUnity.CombineOption" />
    [Serializable]
    public class AvatarCombineOption
    {
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum AvatarCombineFlags : byte
        {
            /// <summary>
            /// The none
            /// </summary>
            None = 0,
            /// <summary>
            /// The remove blendshapes
            /// </summary>
            RemoveBlendshapes = 1,
            /// <summary>
            /// The remove target meshes
            /// </summary>
            RemoveTargetMeshes = 2,
           /// <summary>
            /// The separate head body
            /// </summary>
            SeparateHeadBody = 4,
            /// <summary>
            /// The use material combine
            /// </summary>
            UseMaterialCombine = 8,
        }
        /// <summary>
        /// The avatar combine flags
        /// </summary>
        [SerializeField] public AvatarCombineFlags combineFlags = AvatarCombineFlags.RemoveTargetMeshes | AvatarCombineFlags.UseMaterialCombine;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The material combine option
        /// </summary>
        [DrawIf("combineFlags", (int)AvatarCombineFlags.UseMaterialCombine, DrawIfAttribute.DisablingType.Draw)]
#endif
        [SerializeField] public MaterialCombineOption materialCombineOption;

        /// <summary>
        /// Converts to meshcombineoption.
        /// </summary>
        /// <returns></returns>
        public MeshCombineOption ToMeshCombineOption()
        {
            MeshCombineOption meshCombineOption = new MeshCombineOption()
            {
                materialCombineOption = new MaterialCombineOption() {
                    textureResolutionRatio = this.materialCombineOption.textureResolutionRatio,
                    textureAtlasOptimization = this.materialCombineOption.textureAtlasOptimization,
                    customMaterialData = this.materialCombineOption.customMaterialData
                }, 
                combineFlags = CombineFlags.None 
            };
            meshCombineOption.combineFlags |= combineFlags.HasFlag(AvatarCombineFlags.RemoveBlendshapes) ? CombineFlags.RemoveBlendshapes : CombineFlags.None;
            meshCombineOption.combineFlags |= combineFlags.HasFlag(AvatarCombineFlags.RemoveTargetMeshes) ? CombineFlags.RemoveTargetMeshes : CombineFlags.None;
            meshCombineOption.combineFlags |= combineFlags.HasFlag(AvatarCombineFlags.UseMaterialCombine) ? CombineFlags.UseMaterialCombine : CombineFlags.None;
            return meshCombineOption;
        }
    }
}
