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
using UnityEngine.Rendering;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    public class AREmojiMeshComposer
    {
        /// <summary>
        /// The load node
        /// </summary>
        private GameObject loadNode;
        /// <summary>
        /// The aremoji none head mesh set
        /// </summary>
        private List<Renderer> aremojiMeshSet = new List<Renderer>();
        /// <summary>
        /// Initializes a new instance of the <see cref="AREmojiMeshComposer" /> class.
        /// </summary>
        /// <param name="loadNode">The load node.</param>
        public AREmojiMeshComposer(GameObject loadNode)
        {
            this.loadNode = loadNode;
            SetAREmojiMeshSet();
            SetAREmojiShadow(ShadowCastingMode.Off, false);
        }

        /// <summary>
        /// Set the ar emoji shadow.
        /// </summary>
        /// <param name="castingMode">The casting mode.</param>
        /// <param name="isReceiveShadow">if set to <c>true</c> [is receive shadow].</param>
        public void SetAREmojiShadow(ShadowCastingMode castingMode, bool isReceiveShadow)
        {
            foreach (Renderer meshNode in aremojiMeshSet)
            {
                meshNode.shadowCastingMode = castingMode;
                meshNode.receiveShadows = isReceiveShadow;
            }
        }

        public void SetAREmojiMeshVisible(bool isVisible)
        {
            foreach (Renderer meshNode in aremojiMeshSet)
                meshNode.enabled = isVisible;
        }

        /// <summary>
        /// Set the ar emoji mesh set.
        /// </summary>
        private void SetAREmojiMeshSet()
        {
            Transform[] allChildren = loadNode.GetComponentsInChildren<Transform>(true);
            foreach (Transform child_ in allChildren)
            {
                Renderer mr = child_.GetComponent<Renderer>();
                if (mr != null)
                    aremojiMeshSet.Add(mr);
            }
        }


    }
}