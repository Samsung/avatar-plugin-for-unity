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
    public class AvatarMeshComposer
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MeshType
        {
            /// <summary>
            /// The head
            /// </summary>
            Head,
            /// <summary>
            /// The body
            /// </summary>
            Body,
            /// <summary>
            /// The whole
            /// </summary>
            Whole
        }
        /// <summary>
        /// The load node
        /// </summary>
        private GameObject loadNode;
        /// <summary>
        /// The combined mesh dic
        /// </summary>
        private Dictionary<MeshType, SkinnedMeshRenderer> combinedMeshDic;
        /// <summary>
        /// The avatar eye mesh set
        /// </summary>
        private List<Renderer> noneCombinedMeshSet = new List<Renderer>();
        /// <summary>
        /// The use mesh combine
        /// </summary>
        private bool useMeshCombine;
        /// <summary>
        /// The mesh combine option
        /// </summary>
        private CombineOption meshCombineOption;


        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarMeshComposer"/> class.
        /// </summary>
        /// <param name="loadNode">The load node.</param>
        /// <param name="useMeshCombine">if set to <c>true</c> [use mesh combine].</param>
        /// <param name="meshCombineOption">The mesh combine option.</param>
        public AvatarMeshComposer(GameObject loadNode, bool useMeshCombine, CombineOption meshCombineOption)
        {
            this.loadNode = loadNode;
            this.useMeshCombine = useMeshCombine;
            this.meshCombineOption = meshCombineOption;
            Dictionary<MeshType, List<SkinnedMeshRenderer>> avatarMeshSet = InitAvatarMeshSet();

            if (useMeshCombine)
                MakeCombinedMesh(avatarMeshSet);
        
            SetAvatarShadow(ShadowCastingMode.Off, false);
        }
        /// <summary>
        /// Set the ar emoji shadow.
        /// </summary>
        /// <param name="castingMode">The casting mode.</param>
        /// <param name="isReceiveShadow">if set to <c>true</c> [is receive shadow].</param>
        public void SetAvatarShadow(ShadowCastingMode castingMode, bool isReceiveShadow)
        {
            if (useMeshCombine)
            {
                if (meshCombineOption.separateHeadBody)
                {
                    combinedMeshDic[MeshType.Head].shadowCastingMode = combinedMeshDic[MeshType.Body].shadowCastingMode = castingMode;
                    combinedMeshDic[MeshType.Head].receiveShadows = combinedMeshDic[MeshType.Body].receiveShadows = isReceiveShadow;
                }
                else
                {
                    combinedMeshDic[MeshType.Whole].shadowCastingMode  = castingMode;
                    combinedMeshDic[MeshType.Whole].receiveShadows  = isReceiveShadow;
                }
            }
            foreach (Renderer meshNode in noneCombinedMeshSet)
            {
                meshNode.shadowCastingMode = castingMode;
                meshNode.receiveShadows = isReceiveShadow;
            }
        }
        /// <summary>
        /// Sets the ar emoji mesh visible.
        /// </summary>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        public void SetAvatarMeshVisible(bool isVisible)
        {
            if (useMeshCombine)
            {
                if (meshCombineOption.separateHeadBody)
                    combinedMeshDic[MeshType.Head].enabled = combinedMeshDic[MeshType.Body].enabled = isVisible;
                else
                    combinedMeshDic[MeshType.Whole].enabled = isVisible;
            }
            foreach (Renderer meshNode in noneCombinedMeshSet)
                meshNode.enabled = isVisible;
        }
        /// <summary>
        /// Gets the combined mesh.
        /// </summary>
        /// <param name="combineType">Type of the combine.</param>
        /// <returns></returns>
        public SkinnedMeshRenderer GetCombinedMesh(MeshType combineType)
        {
            if (combinedMeshDic.ContainsKey(combineType))
                return combinedMeshDic[combineType];
            else
            {
                Debug.Log("There is no Combined Mesh corresponding to " + combineType.ToString() + "!!");
                return null;
            }
        }


        /// <summary>
        /// Makes the combined mesh.
        /// </summary>
        /// <param name="avatarMeshSet">The avatar mesh set.</param>
        private void MakeCombinedMesh(Dictionary<MeshType, List<SkinnedMeshRenderer>> avatarMeshSet)
        {
            combinedMeshDic = new Dictionary<MeshType, SkinnedMeshRenderer>();
            Transform hips_JNT = GetChildGameObject(NodeDefines.HIP_JNT, loadNode).transform;
            if (meshCombineOption.separateHeadBody)
            {
                combinedMeshDic[MeshType.Head] = MakeCombinedMesh(hips_JNT, avatarMeshSet[MeshType.Head], "CombinedHeadMesh");
                combinedMeshDic[MeshType.Body] = MakeCombinedMesh(hips_JNT, avatarMeshSet[MeshType.Body], "CombinedBodyMesh");
            }
            else
                combinedMeshDic[MeshType.Whole] = MakeCombinedMesh(hips_JNT, avatarMeshSet[MeshType.Whole], "CombinedMesh");
        }
        /// <summary>
        /// Makes the combined mesh.
        /// </summary>
        /// <param name="hips_JNT">The hips JNT.</param>
        /// <param name="combineTargetMeshSet">The combine target mesh set.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private SkinnedMeshRenderer MakeCombinedMesh(Transform hips_JNT, List<SkinnedMeshRenderer> combineTargetMeshSet, string name)
        {
            GameObject combinedBodyMeshObject = MakeSubNode(name, loadNode.transform);
            SkinnedMeshRenderer combinedMesh = combinedBodyMeshObject.AddComponent<SkinnedMeshRenderer>();
            combinedMesh.rootBone = hips_JNT;
            Transform avatarCompTransform = loadNode.transform.parent;
            loadNode.SetActive(false);
            loadNode.transform.parent = null;
            loadNode.transform.localScale = Vector3.one * 0.01f;
            AvatarMeshCombiner.CombineSkinnedMesh(combineTargetMeshSet.ToArray(), combinedMesh, meshCombineOption.combineFlags, meshCombineOption.materialCombineOption);
            loadNode.transform.parent = avatarCompTransform;
            loadNode.transform.localScale = Vector3.one * 0.01f;
            loadNode.SetActive(true);
            return combinedMesh;
        }
        /// <summary>
        /// Set the ar emoji mesh set.
        /// </summary>
        /// <returns></returns>
        private Dictionary<MeshType, List<SkinnedMeshRenderer>> InitAvatarMeshSet()
        {
            Dictionary<MeshType, List<SkinnedMeshRenderer>> avatarMeshSet = new Dictionary<MeshType, List<SkinnedMeshRenderer>>();

            Renderer[] allChildren = loadNode.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer child_ in allChildren)
            {

                if (child_ is SkinnedMeshRenderer)
                    NormalizeBoneWeights((SkinnedMeshRenderer)child_);

                if (!useMeshCombine || !AvatarMaterialCombiner.MaterialCombineVerification(child_.material) || child_.name.Contains("iris_") || child_.name.Contains("cornea_"))
                {
                    if (useMeshCombine)
                        AvatarMeshCombiner.InstantiateRendererParms(child_, meshCombineOption.materialCombineOption.textureResolutionRatio);
                    noneCombinedMeshSet.Add(child_);
                }
                else if (useMeshCombine && AvatarMaterialCombiner.MaterialCombineVerification(child_.material) && child_ is SkinnedMeshRenderer)
                {
                    if (meshCombineOption.separateHeadBody)
                    {
                        if (IsContainedHeadNode(child_.transform))
                        {
                            if (!avatarMeshSet.ContainsKey(MeshType.Head))
                                avatarMeshSet[MeshType.Head] = new List<SkinnedMeshRenderer>();
                            avatarMeshSet[MeshType.Head].Add((SkinnedMeshRenderer)child_);
                        }
                        else
                        {
                            if (!avatarMeshSet.ContainsKey(MeshType.Body))
                                avatarMeshSet[MeshType.Body] = new List<SkinnedMeshRenderer>();
                            avatarMeshSet[MeshType.Body].Add((SkinnedMeshRenderer)child_);
                        }
                    }
                    else if (!meshCombineOption.separateHeadBody)
                    {
                        if (!avatarMeshSet.ContainsKey(MeshType.Whole))
                            avatarMeshSet[MeshType.Whole] = new List<SkinnedMeshRenderer>();
                        avatarMeshSet[MeshType.Whole].Add((SkinnedMeshRenderer)child_);
                    }
                }
            }
            return avatarMeshSet;
        }

        /// <summary>
        /// Normalizes the bone weights.
        /// </summary>
        /// <param name="skinnedMesh">The skinned mesh.</param>
        private void NormalizeBoneWeights(SkinnedMeshRenderer skinnedMesh)
        {
            var mesh = skinnedMesh.sharedMesh;
            var boneWeightList = mesh.boneWeights;
            for (int i = 0; i < boneWeightList.Length; i++)
            {
                var weight = boneWeightList[i];
                float allWeight = 0.0f;
                if (weight.weight0 > 0)
                    allWeight += weight.weight0;             
                if (weight.weight1 > 0)
                    allWeight += weight.weight1;            
                if (weight.weight2 > 0)
                    allWeight += weight.weight2;
                if (weight.weight3 > 0)
                    allWeight += weight.weight3;

                if (allWeight == 0.0f)
                    continue;

                float calcWeight = 1 / allWeight;
                weight.weight0 *= calcWeight;
                weight.weight1 *= calcWeight;
                weight.weight2 *= calcWeight;
                weight.weight3 *= calcWeight;
                boneWeightList[i] = weight;
            }
            mesh.boneWeights = boneWeightList;
        }
        /// <summary>
        /// Determines whether [is contained head node] [the specified node].
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if [is contained head node] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsContainedHeadNode(Transform node)
        {
            if (NodeDefines.INCLUDED_HEADONLY_PARENTS.Contains(node.name))
                return true;
            else if (node.parent != null)
                return IsContainedHeadNode(node.parent);
            else
                return false;
        }
        /// <summary>
        /// Makes the sub node.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private GameObject MakeSubNode(string name, Transform parent)
        {
            GameObject subNode = new GameObject(name);
            subNode.transform.parent = parent;
            subNode.transform.localPosition = Vector3.zero;
            subNode.transform.localEulerAngles = Vector3.zero;
            subNode.transform.localScale = Vector3.one;
            subNode.SetActive(true);
            return subNode;
        }
        /// <summary>
        /// Gets the child game object.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="avatar">The avatar.</param>
        /// <returns></returns>
        private GameObject GetChildGameObject(string name, GameObject avatar)
        {
            Transform[] allChildren = avatar.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
                if (child.name == name)
                    return child.gameObject;

            return null;
        }

    }
}