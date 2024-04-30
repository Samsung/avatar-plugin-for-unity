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
using System;
using UnityEngine;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MaterialCombineOption
    {
        /// <summary>
        /// 
        /// </summary>
        public enum TextureResolutionRatio : short
        {
            /// <summary>
            /// The one
            /// </summary>
            One = 1,
            /// <summary>
            /// The half
            /// </summary>
            Half = 2,
            /// <summary>
            /// The quarter
            /// </summary>
            Quarter = 4,
            /// <summary>
            /// The eighth
            /// </summary>
            Eighth = 8,
        }
        /// <summary>
        /// The texture resolution ratio
        /// </summary>
        [SerializeField]
        public TextureResolutionRatio textureResolutionRatio = TextureResolutionRatio.One;
        /// <summary>
        /// The texture atlas optimization
        /// </summary>
        [SerializeField]
        public bool textureAtlasOptimization = true;
    }
}
