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
        /// Gets the avatar bone tamplate.
        /// </summary>
        /// <value>
        /// The avatar bone tamplate.
        /// </value>
        private Dictionary<BodyType, Dictionary<string, Dictionary<string, List<float>>>> avatarBoneTamplate 
        { 
            get 
            {
                if (_avatarBoneTamplate == null)
                    _avatarBoneTamplate = GetPoseData();            
                return _avatarBoneTamplate;
            } 
        }
        /// <summary>
        /// The avatar bone tamplate
        /// </summary>
        private static Dictionary<BodyType, Dictionary<string, Dictionary<string, List<float>>>> _avatarBoneTamplate = null;
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
            SetAvatarDefaultRig(GetChildGameObject(loadNode, NodeDefines.HIP_JNT).transform, avatarBoneTamplate[bodyType]);
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
        /// The setAvatarDefaultRig.
        /// </summary>
        /// <param name="node">The node<see cref="Transform" />.</param>
        /// <param name="boneTamplate">The bone tamplate.</param>
        private void SetAvatarDefaultRig(Transform node , Dictionary<string, Dictionary<string, List<float>>> boneTamplate)
        {

            Queue<Transform> childrenQueue = new Queue<Transform>();
            childrenQueue.Enqueue(node);
            while (childrenQueue.Count > 0)
            {
                Transform dequeueNode = childrenQueue.Dequeue();
                if (boneTamplate.ContainsKey(dequeueNode.name))
                {
                    List<float> pos = boneTamplate[dequeueNode.name]["m_LocalPosition"];
                    List<float> rot = boneTamplate[dequeueNode.name]["m_LocalRotation"];
                    //List<float> scl = boneTamplate[dequeueNode.name]["m_LocalScale"];
                    dequeueNode.transform.localPosition = new Vector3(pos[0], pos[1], pos[2]);
                    dequeueNode.transform.localEulerAngles = new Vector3(rot[0], rot[1], rot[2]);
                    //node.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                for (int idx = 0; idx < dequeueNode.childCount; idx++)
                    childrenQueue.Enqueue(dequeueNode.GetChild(idx));
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
        /// <summary>
        /// Sets the body type pose data.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<BodyType, Dictionary<string, Dictionary<string, List<float>>>> GetPoseData()
        {
            PoseData mAvatarDefaultRig = JsonUtility.FromJson<PoseData>((Resources.Load("AvatarDefaultRig") as TextAsset).text);
            Dictionary<BodyType, Dictionary<string, Dictionary<string, List<float>>>> boneTamplate = new Dictionary<BodyType, Dictionary<string, Dictionary<string, List<float>>>>();
            boneTamplate[BodyType.Female] = GetBodyTypePoseData(mAvatarDefaultRig, BodyType.Female);
            boneTamplate[BodyType.Male] = GetBodyTypePoseData(mAvatarDefaultRig, BodyType.Male);
            boneTamplate[BodyType.Junior] = GetBodyTypePoseData(mAvatarDefaultRig, BodyType.Junior);
            return boneTamplate;
        }

        /// <summary>
        /// Gets the body type pose data.
        /// </summary>
        /// <param name="mAvatarDefaultRig">The m avatar default rig.</param>
        /// <param name="bodyType">Type of the body.</param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<string, List<float>>> GetBodyTypePoseData(PoseData mAvatarDefaultRig, BodyType bodyType)
        {
            List<BoneData> mBodyTypeBoneData = bodyType.Equals(BodyType.Female) ? mAvatarDefaultRig.female : bodyType.Equals(BodyType.Male) ? mAvatarDefaultRig.male : mAvatarDefaultRig.junior;

            Dictionary<string, Dictionary<string, List<float>>> bodyTypeBoneTamplate = new Dictionary<string, Dictionary<string, List<float>>>();
            foreach (var boneData in mBodyTypeBoneData)
            {
                Dictionary<string, List<float>> transformVals = new Dictionary<string, List<float>>();
                transformVals["m_LocalPosition"] = boneData.m_LocalPosition;
                transformVals["m_LocalRotation"] = boneData.m_LocalRotation;
                transformVals["m_LocalScale"] = boneData.m_LocalScale;
                bodyTypeBoneTamplate[boneData.name] = transformVals;
            }
            return bodyTypeBoneTamplate;
        }

        /// <summary>
        /// The getChildGameObject.
        /// </summary>
        /// <param name="avatar">The avatar<see cref="GameObject" />.</param>
        /// <param name="name">The name<see cref="string" />.</param>
        /// <returns>
        /// The <see cref="GameObject" />.
        /// </returns>
        internal static GameObject GetChildGameObject(GameObject avatar, string name)
        {
            Transform[] allChildren = avatar.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
                if (child.name == name)
                    return child.gameObject;

            return null;
        }
    }
}