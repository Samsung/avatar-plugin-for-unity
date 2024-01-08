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
using UnityEngine;

namespace AvatarPluginForUnity
{
    public class TextureInfoBase
    {
        /// <summary>
        /// The main texture2 d
        /// </summary>
        public Texture2D mainTexture2D;
        /// <summary>
        /// The normal texture2 d
        /// </summary>
        public Texture2D normalTexture2D;
        /// <summary>
        /// The ao metallic roughness texture2 d
        /// </summary>
        public Texture2D Ao_metallicRoughnessTexture2D;
        /// <summary>
        /// The size
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// The size ratio
        /// </summary>
        public Vector2 sizeRatio;
        /// <summary>
        /// The offset ratio
        /// </summary>
        public Vector2 offsetRatio;
    }
}