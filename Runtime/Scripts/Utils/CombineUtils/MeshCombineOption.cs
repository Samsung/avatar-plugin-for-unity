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

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MeshCombineOption
    {
        /// <summary>
        /// 
        /// </summary>
        [Flags]
        public enum CombineFlags : byte
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
            /// The use material combine
            /// </summary>
            UseMaterialCombine = 4,
        }

        /// <summary>
        /// The combine flags
        /// </summary>
        [SerializeField] public CombineFlags combineFlags = CombineFlags.RemoveTargetMeshes | CombineFlags.UseMaterialCombine;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The material combine option
        /// </summary>
        [DrawIf("combineFlags", (int)CombineFlags.UseMaterialCombine, DrawIfAttribute.DisablingType.Draw)]
#endif
        [SerializeField] public MaterialCombineOption materialCombineOption;
    }
}
