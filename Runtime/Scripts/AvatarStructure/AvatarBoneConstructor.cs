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
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class AvatarBoneConstructor
    {
        /// <summary>
        /// 
        /// </summary>
        public enum BodyType
        {
            /// <summary>
            /// The Female
            /// </summary>
            Female,
            /// <summary>
            /// The Male
            /// </summary>
            Male,
            /// <summary>
            /// The Junior
            /// </summary>
            Junior,
        }
        /// <summary>
        /// Gets the type of the body.
        /// </summary>
        /// <value>
        /// The type of the body.
        /// </value>
        public BodyType bodyType => _bodyType;
        /// <summary>
        /// The body type
        /// </summary>
        private BodyType _bodyType = BodyType.Female;
        /// <summary>
        /// The load node
        /// </summary>
        private GameObject loadNode;
        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarBoneComposer" /> class.
        /// </summary>
        /// <param name="loadNode">The load node.</param>
        public AvatarBoneConstructor(GameObject loadNode)
        {
            this.loadNode = loadNode;
            ReDefineNodeStructure();
        }
        /// <summary>
        /// Res the define node structure.
        /// </summary>
        private void ReDefineNodeStructure()
        {

            Transform head_GRP = null;
            Transform rootNode = null;
            Transform model = null; ;
            Transform[] allChildren = loadNode.GetComponentsInChildren<Transform>(true);

            foreach (Transform child_ in allChildren)
            {
                if (child_.name.Equals(NodeDefines.HEAD_GRP))
                    head_GRP = child_;
                else if (child_.name.Equals(NodeDefines.MODEL))
                    model = child_;
                else if (child_.name.Equals(NodeDefines.ROOT_NODE))
                    rootNode = child_;
                else if (child_.name.Equals(NodeDefines.MaleBody) || child_.name.Equals(NodeDefines.JuniorBody))
                    _bodyType = child_.name.Equals(NodeDefines.MaleBody) ? BodyType.Male : BodyType.Junior;
            }
            if (!head_GRP.parent.name.Equals(NodeDefines.MODEL))
                head_GRP.parent = model;
            Transform mayDestroyedNode = rootNode.GetChild(0);
            if (mayDestroyedNode.name != NodeDefines.MODEL_GRP && mayDestroyedNode.name != NodeDefines.RIG_GRP)
            {
                List<Transform> chilren = new List<Transform>();
                for (int idx = 0; idx < mayDestroyedNode.childCount; idx++)
                    chilren.Add(mayDestroyedNode.GetChild(idx));
                foreach (var child in chilren)
                    child.parent = mayDestroyedNode.parent;
                Object.Destroy(mayDestroyedNode.gameObject);
            }
        }
        /// <summary>
        /// Gets the constructed avatar.
        /// </summary>
        /// <returns></returns>
        public Avatar GetConstructedAvatar()
        {
            return new AvatarConstructor(loadNode.transform.parent.gameObject).ConstructAvatar();
        }

    }
}