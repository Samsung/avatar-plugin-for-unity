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
using UnityEngine;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    internal class MeshCombineDescriptor
    {
        /// <summary>
        /// Gets or sets the sub mes combine instance.
        /// </summary>
        /// <value>
        /// The sub mes combine instance.
        /// </value>
        public CombineInstance subMesCombineInstance { get; set; }
        /// <summary>
        /// Gets or sets the parent mesh.
        /// </summary>
        /// <value>
        /// The parent mesh.
        /// </value>
        public SkinnedMeshRenderer parentMesh { get; set; }
        /// <summary>
        /// Gets or sets the size of the texture.
        /// </summary>
        /// <value>
        /// The size of the texture.
        /// </value>
        public Texture mainTexture { get; set; }
        /// <summary>
        /// Gets or sets the size of the texture.
        /// </summary>
        /// <value>
        /// The size of the texture.
        /// </value>
        public Vector2 textureSize { get; set; }


        public Material material { get; set; }
        public IEnumerable<BoneWeight> boneWeights;
        /// <summary>
        /// Initializes a new instance of the <see cref="MeshCombineDescriptor" /> struct.
        /// </summary>
        /// <param name="subMesCombineInstance">The sub mes combine instance.</param>
        /// <param name="parentMesh">The parent mesh.</param>
        /// <param name="material">The material.</param>
        /// <param name="mainTexture">The main texture.</param>
        /// <param name="textureSize">Size of the texture.</param>
        private MeshCombineDescriptor(CombineInstance subMesCombineInstance, SkinnedMeshRenderer parentMesh,  Material material,Texture mainTexture, Vector2 textureSize)
        {
            this.subMesCombineInstance = subMesCombineInstance;
            this.parentMesh = parentMesh;
            this.mainTexture = mainTexture;
            this.textureSize = textureSize;
        }

        /// <summary>
        /// Gets the mesh combine descriptor.
        /// </summary>
        /// <param name="skMesh">The sk mesh.</param>
        /// <returns></returns>
        public static List<MeshCombineDescriptor> GetMeshCombineDescriptorsFromSubmeshes(SkinnedMeshRenderer skMesh)
        {
            Mesh mesh = skMesh.sharedMesh;
            List<MeshCombineDescriptor> meshCombineDescriptors = new List<MeshCombineDescriptor>();
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Material material = skMesh.materials[i];
                Texture baseColorTexture = material.GetTexture("baseColorTexture");
                Vector2 textureSize = new Vector2(baseColorTexture.width, baseColorTexture.height);
                meshCombineDescriptors.Add(new MeshCombineDescriptor(new CombineInstance { mesh = mesh, subMeshIndex = i}, skMesh, skMesh.materials[i], baseColorTexture, textureSize));
            }
            return meshCombineDescriptors;
        }
    }
}
