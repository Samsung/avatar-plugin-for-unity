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
using UnityEngine;
using static AvatarPluginForUnity.AvatarBoneConstructor;

namespace AvatarPluginForUnity
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class AvatarHairSimulator : MonoBehaviour
    {
        /// <summary>
        /// The global gravity
        /// </summary>
        private static Vector3 GLOBAL_GRAVITY = Physics.gravity;
        /// <summary>
        /// The dist range
        /// </summary>
        private static float DIST_RANGE = 0.4f;
        /// <summary>
        /// The reposition difference angle maximum
        /// </summary>
        private static float REPOSITION_DIFF_ANGLE_MAX = 30.0f;
        /// <summary>
        /// The reposition difference position maximum
        /// </summary>
        private float REPOSITION_DIFF_POS_MAX = 2.0f;
        /// <summary>
        /// The spring factor range
        /// </summary>
        private static Vector2 SPRING_FACTOR_RANGE = new Vector2(5000.0f, 10000.0f);
        /// <summary>
        /// The limit factor range
        /// </summary>
        private static Vector2 LIMIT_FACTOR_RANGE = new Vector2(15.0f, 40.0f);
        /// <summary>
        /// The earring size limit
        /// </summary>
        private static float EARRING_SIZE_LIMIT = 0.05f;
        /// <summary>
        /// The avatar component
        /// </summary>
        private AvatarComponent avatarComponent = null;
        /// <summary>
        /// The avatar scale
        /// </summary>
        private float _AvatarScale = 1.0f;
        /// <summary>
        /// The enable hair simulation
        /// </summary>
        private bool _EnableHairSimulation = false;
        /// <summary>
        /// The head center JNT
        /// </summary>
        private Transform headCenter_JNT;
        /// <summary>
        /// The hair jn ts
        /// </summary>
        private List<Transform> hairJNTs;
        /// <summary>
        /// The hair org position set
        /// </summary>
        private (Vector3, Vector3)[] hairORGPositionSet;
        /// <summary>
        /// The hair target dic
        /// </summary>
        private Dictionary<Transform, Transform> hairTargetDic;
        /// <summary>
        /// The hair container
        /// </summary>
        private Transform hairContainer;
        /// <summary>
        /// The enable earring simulation
        /// </summary>
        private bool _EnableEarring_Simulation = false;
        /// <summary>
        /// The head JNT
        /// </summary>
        private Transform head_JNT;
        /// <summary>
        /// The earring jn ts
        /// </summary>
        private List<Transform> earringJNTs;
        /// <summary>
        /// The earring org position set
        /// </summary>
        private (Vector3, Vector3)[] earringORGPositionSet;
        /// <summary>
        /// The earring target dic
        /// </summary>
        private Dictionary<Transform, Transform> earringTargetDic;
        /// <summary>
        /// The earring container
        /// </summary>
        private Transform earringContainer;
        /// <summary>
        /// The head pre rot
        /// </summary>
        private Quaternion headPreRot;
        /// <summary>
        /// The refresh
        /// </summary>
        private bool refresh = false;
        /// <summary>
        /// The difference local
        /// </summary>
        private float _diffLocal = 0.0f;
        /// <summary>
        /// The result dist
        /// </summary>
        private float resultDist = 0.0f;
        /// <summary>
        /// The avatar collider jn ts
        /// </summary>
        private Dictionary<string, Transform> avatarColliderJNTs;
        /// <summary>
        /// The hair simulation option
        /// </summary>
        private HairSimulationOption hairSimulationOption;
        /// <summary>
        /// Initializes the hair simulator.
        /// </summary>
        /// <param name="hairSimulationOption">The hair simulation option.</param>
        /// <param name="bodyType">Type of the body.</param>
        public void InitHairSimulator(HairSimulationOption hairSimulationOption, BodyType bodyType)
        {
            avatarComponent = transform.parent.GetComponent<AvatarComponent>();
            head_JNT = avatarComponent.FindAvatarTransformByName("head_JNT");
            this.hairSimulationOption = hairSimulationOption;
            this.hairSimulationOption._IntensityMultiplier = 1 / this.hairSimulationOption._IntensityMultiplier;

            if (hairSimulationOption._AvatarCollider)
                InitAvatarCollider(bodyType);
        }
        /// <summary>
        /// Excutes the hair simulator.
        /// </summary>
        public void ExcuteHairSimulator()
        {
            _AvatarScale = avatarComponent.transform.lossyScale.x;
            _EnableHairSimulation = ApplyHairSimulation();
            _EnableEarring_Simulation = ApplyAccessorySimulation();

            if (hairSimulationOption._AvatarCollider && !_EnableHairSimulation && !_EnableEarring_Simulation)
            {
                foreach(var avatarCollider in avatarColliderJNTs)
                    DestroyImmediate(avatarCollider.Value.gameObject);
                DestroyImmediate(this);
            }
            else
                REPOSITION_DIFF_POS_MAX = resultDist * 1.5f;

        }
        /// <summary>
        /// Applies the hair simulation.
        /// </summary>
        /// <returns></returns>
        private bool ApplyHairSimulation()
        {
            headCenter_JNT = avatarComponent.FindAvatarTransformByName("headCenter_JNT");
            Rigidbody headCenter_JNTrigidbody = headCenter_JNT.gameObject.AddComponent<Rigidbody>();
            headCenter_JNTrigidbody.isKinematic = true;

            Transform[] rootChilren = new Transform[headCenter_JNT.childCount];
            int headCenter_JNTCount = headCenter_JNT.childCount;
            if (headCenter_JNTCount != 0)
                hairJNTs = new List<Transform>();
            else
                return false;

            for (int i = 0; i < headCenter_JNTCount; i++)
                rootChilren[i] = headCenter_JNT.GetChild(i);

            MakeSimulationJoint(rootChilren, 0.02f, true);

            return true;
        }
        /// <summary>
        /// Applies the accessory simulation.
        /// </summary>
        /// <returns></returns>
        private bool ApplyAccessorySimulation()
        {

            Rigidbody head_JNTrigidbody = head_JNT.gameObject.AddComponent<Rigidbody>();
            head_JNTrigidbody.isKinematic = true;

            List<Transform> _rootChilren = new List<Transform>();

            if (head_JNT.Find("l_earring_accessory_JNT"))
            {
                Transform l_earring_accessory_JNT = head_JNT.Find("l_earring_accessory_JNT");
                _rootChilren.Add(l_earring_accessory_JNT);
            }
            if (head_JNT.Find("r_earring_accessory_JNT"))
            {
                Transform r_earring_accessory_JNT = head_JNT.Find("r_earring_accessory_JNT");
                _rootChilren.Add(r_earring_accessory_JNT);
            }


            Transform[] rootChilren = _rootChilren.ToArray();

            int head_JNTCount = rootChilren.Length;
            if (head_JNTCount != 0)
            {
                Transform earring_GEO = avatarComponent.FindAvatarTransformByName("earring_GEO");
                if (!earring_GEO || earring_GEO.GetComponent<SkinnedMeshRenderer>().bounds.size.y < EARRING_SIZE_LIMIT * _AvatarScale)
                    return false;

                earringJNTs = new List<Transform>();
            }
            else
                return false;

            MakeSimulationJoint(rootChilren, 0.01f, false);

            return true;
        }
        /// <summary>
        /// Makes the simulation joint.
        /// </summary>
        /// <param name="rootChilren">The root chilren.</param>
        /// <param name="colliderSize">Size of the collider.</param>
        /// <param name="isHair">if set to <c>true</c> [is hair].</param>
        private void MakeSimulationJoint(Transform[] rootChilren, float colliderSize, bool isHair)
        {
            int jntCount = rootChilren.Length;

            Transform rootParent = isHair ? headCenter_JNT : head_JNT;
            List<(Vector3, Vector3)> orgPositionDic = new List<(Vector3, Vector3)>();
            for (int i = 0; i < jntCount; i++)
            {
                Transform[] allChild = rootChilren[i].GetComponentsInChildren<Transform>();
                int childrenCount = allChild.Length;
                Transform parent = rootParent;
                float preXAngle = 0.0f;

                List<Transform> joints = isHair ? hairJNTs : earringJNTs;

                List<Transform> jnt_Tmps = new List<Transform>();
                for (int j = 0; j < childrenCount; j++)
                {
                    Transform child = allChild[j];
                    Transform tunnedNode = new GameObject(child.name + "_tunned").transform;
                    tunnedNode.localScale = Vector3.one;
                    tunnedNode.position = child.position;
                    tunnedNode.parent = parent;
                    tunnedNode.LookAt(parent);

                    child.parent = tunnedNode;
                    child.localPosition = Vector3.zero;
                    if (child.childCount != 0)
                        child.GetChild(0).parent = tunnedNode;

                    preXAngle += 180.0f < tunnedNode.rotation.eulerAngles.x ? tunnedNode.rotation.eulerAngles.x - 360.0f : tunnedNode.rotation.eulerAngles.x;
                    parent = tunnedNode;
                    joints.Add(tunnedNode);

                    if (tunnedNode.parent != rootParent)
                        resultDist += Vector3.Distance(Vector3.zero, tunnedNode.localPosition) / _AvatarScale;

                    orgPositionDic.Add((tunnedNode.localPosition, tunnedNode.localEulerAngles));

                    jnt_Tmps.Add(tunnedNode);
                }

                if (isHair)
                    hairORGPositionSet = orgPositionDic.ToArray();
                else
                    earringORGPositionSet = orgPositionDic.ToArray();

                float _dist = 0.0f;
                foreach (var joint in jnt_Tmps)
                {
                    if (joint.parent != rootParent)
                    _dist += Vector3.Distance(Vector3.zero, joint.localPosition) / _AvatarScale;

                    var _spring = GetValueFromDistanceWeight(SPRING_FACTOR_RANGE, _dist, false);
                    var _limit = GetValueFromDistanceWeight(LIMIT_FACTOR_RANGE, _dist, true);

                    var anglePenalty = (isHair && 0.0f < preXAngle);
                    var singleJointPenalty = (joint.childCount != 2 && joint.parent == rootParent);

                    if (anglePenalty)
                    {
                        _spring = anglePenalty && singleJointPenalty ? 0.0f: anglePenalty || singleJointPenalty ? _spring*2.0f: _spring;
                        _limit = anglePenalty && singleJointPenalty ? 0.0f : anglePenalty || singleJointPenalty ? _limit*2.0f : _limit;
                    }

                    var _limitXY = _limit * 0.5f;

                    CharacterJoint characterJoint = joint.gameObject.AddComponent<CharacterJoint>();
                    characterJoint.connectedBody = joint.parent.GetComponent<Rigidbody>();
                    characterJoint.anchor = Vector3.zero;
                    characterJoint.enablePreprocessing = false;
                    characterJoint.twistLimitSpring = new SoftJointLimitSpring() { spring = _spring, damper = 1.0f };
                    characterJoint.lowTwistLimit = new SoftJointLimit() { limit = -_limitXY, bounciness = 0.01f, contactDistance = 10.0f }; //X
                    characterJoint.highTwistLimit = new SoftJointLimit() { limit = _limitXY, bounciness = 0.01f, contactDistance = 10.0f }; //X
                    characterJoint.swingLimitSpring = new SoftJointLimitSpring() { spring = _spring, damper = 1.0f };
                    characterJoint.swing1Limit = new SoftJointLimit() { limit = _limitXY, bounciness = 0.01f, contactDistance = 10.0f }; //Y
                    characterJoint.swing2Limit = new SoftJointLimit() { limit = _limit, bounciness = 0.01f, contactDistance = 10.0f }; //Z

                    if (hairSimulationOption._AvatarCollider)
                    {
                        SphereCollider sphereCollider = joint.gameObject.AddComponent<SphereCollider>();
                        sphereCollider.radius = colliderSize * _AvatarScale;
                    }
                    float scaleWeight = Mathf.Pow(_AvatarScale, 2.0f);
                    joint.GetComponent<Rigidbody>().useGravity = false;
                    joint.GetComponent<Rigidbody>().angularDrag = 0.01f * scaleWeight * hairSimulationOption._IntensityMultiplier;
                    joint.GetComponent<Rigidbody>().drag = 0.01f * scaleWeight * hairSimulationOption._IntensityMultiplier;
                    joint.GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
                    joint.GetComponent<Rigidbody>().inertiaTensor = Vector3.one * 0.01f * scaleWeight * hairSimulationOption._IntensityMultiplier;
                    joint.GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
                }
            }
            if (/*_PositionBase*/true)
            {
                Transform container;
                if (isHair)
                    container = hairContainer = new GameObject("hairContainer").transform;
                else
                    container = earringContainer = new GameObject("earringContainer").transform;

                container.parent = avatarComponent.FindAvatarTransformByName("LoadNode");
                container.localPosition = Vector3.zero;
                container.localEulerAngles = Vector3.zero;
                container.localScale = Vector3.one;
                List<Transform> joints = isHair ? hairJNTs : earringJNTs;
                Dictionary<Transform, Transform> targetDic = isHair ? hairTargetDic = new Dictionary<Transform, Transform>() : earringTargetDic = new Dictionary<Transform, Transform>();

                foreach (var joint in joints)
                {
                    if (joint.parent == rootParent)
                    {
                        Transform targetJNT = new GameObject(joint.name + "_Target").transform;
                        targetJNT.parent = rootParent;
                        targetJNT.localPosition = joint.localPosition;
                        targetJNT.localRotation = joint.localRotation;
                        targetJNT.localScale = joint.localScale;
                        targetDic[joint] = targetJNT;
                        joint.parent = container;
                    }
                }
            }
        }

        //Hair Distance Range (0.0f ~ 400.0f)
        /// <summary>
        /// Gets the value from distance weight.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="hairDist">The hair dist.</param>
        /// <param name="isIncrease">if set to <c>true</c> [is increase].</param>
        /// <returns></returns>
        private float GetValueFromDistanceWeight(Vector2 range, float hairDist, bool isIncrease)
        {
            hairDist = DIST_RANGE < hairDist ? DIST_RANGE : hairDist;
            var hairDistWeight = hairDist / DIST_RANGE;
            float val = (range.y - range.x) * hairDistWeight;
            val = isIncrease ? (range.x + val) : range.y - val;
            if (val < range.x)
                return range.x;
            else if (range.y < val)
                return range.y;
            return val;
        }

        /// <summary>
        /// Initializes the avatar collider.
        /// </summary>
        /// <param name="bodyType">Type of the body.</param>
        private void InitAvatarCollider(BodyType bodyType)
        {
            GameObject avatarCollider = (GameObject)Instantiate(Resources.Load("Collider/Collider_" + bodyType));
            avatarCollider.transform.parent = avatarComponent.FindAvatarTransformByName("LoadNode");
            avatarCollider.transform.localPosition = Vector3.zero;
            avatarCollider.transform.localEulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
            avatarCollider.transform.localScale = Vector3.one * 10000.0f;
            avatarColliderJNTs = avatarCollider.GetComponentsInChildren<Transform>(true).Where(x => x.name.Contains("_COL")).ToList().ToDictionary(x => x.name, x => x.transform);

            foreach (var avatarColliderJNT in avatarColliderJNTs)
            {
                Transform joint = avatarComponent.FindAvatarTransformByName(avatarColliderJNT.Key.Replace("_COL", "_JNT"));
                Transform collider = avatarColliderJNT.Value;
                collider.parent = joint;
                collider.gameObject.AddComponent<MeshCollider>().sharedMesh = collider.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(collider.GetComponent<MeshRenderer>());
                DestroyImmediate(collider.GetComponent<MeshFilter>());
            }

            Transform head_COL = new GameObject("head_COL").transform;
            Transform head_GEO = avatarComponent.FindAvatarTransformByName("head_GEO");
            Mesh headMesh = new Mesh();

            head_GEO.GetComponent<SkinnedMeshRenderer>().BakeMesh(headMesh);
            head_COL.parent = head_GEO;
            head_COL.localPosition = Vector3.zero;
            head_COL.localEulerAngles = Vector3.zero;
            head_COL.gameObject.AddComponent<MeshCollider>().sharedMesh = headMesh;
            head_COL.parent = head_JNT;
            avatarColliderJNTs["head_COL"] = head_COL;
            DestroyImmediate(avatarCollider);
        }
        /// <summary>
        /// Refreshes the specified joints.
        /// </summary>
        /// <param name="joints">The joints.</param>
        /// <param name="container">The container.</param>
        /// <param name="orgPositionSet">The org position set.</param>
        /// <param name="targetDic">The target dic.</param>
        private void Refresh(List<Transform> joints, Transform container, (Vector3, Vector3)[] orgPositionSet, Dictionary<Transform, Transform> targetDic)
        {
            for (int i = 0; i < joints.Count; i++)
            {
                var joint = joints[i];
                var hairTransform = orgPositionSet[i];

                if (joint.parent.Equals(container))
                {
                    Transform targetJNT = targetDic[joint];
                    joint.position = targetJNT.position;
                    joint.rotation = targetJNT.rotation;
                }
                else
                {
                    joint.localPosition = hairTransform.Item1;
                    joint.localRotation = Quaternion.Euler(hairTransform.Item2.x, hairTransform.Item2.y, hairTransform.Item2.z);
                }
            }
        }
        /// <summary>
        /// Fixeds the update.
        /// </summary>
        private void FixedUpdate()
        {
            if (_EnableHairSimulation)
                foreach (var hairJNT in hairJNTs)
                    hairJNT.GetComponent<Rigidbody>().AddForce(GLOBAL_GRAVITY * _AvatarScale);

            if (_EnableEarring_Simulation)
                foreach (var earringJNT in earringJNTs)
                    earringJNT.GetComponent<Rigidbody>().AddForce(GLOBAL_GRAVITY * _AvatarScale);

        }
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (_EnableHairSimulation || _EnableEarring_Simulation)
            {
                refresh = false;
                _diffLocal = 0.0f;
                if (_EnableHairSimulation)
                {
                    for (int i = 0; i < hairJNTs.Count; i++)
                        if (!hairJNTs[i].parent.Equals(hairContainer)) 
                            _diffLocal += Vector3.Distance(hairJNTs[i].localPosition, hairORGPositionSet[i].Item1);                                            
                }

                if (_EnableEarring_Simulation)
                {
                    for (int i = 0; i < earringJNTs.Count; i++)
                        if (!earringJNTs[i].parent.Equals(earringContainer))
                            _diffLocal += Vector3.Distance(earringJNTs[i].localPosition, earringORGPositionSet[i].Item1);
                }

                if (REPOSITION_DIFF_POS_MAX < _diffLocal / _AvatarScale)
                    refresh = true;

                if (headPreRot != null && REPOSITION_DIFF_ANGLE_MAX < Quaternion.Angle(headPreRot, head_JNT.rotation))
                    refresh = true;

                headPreRot = head_JNT.rotation;

                if (refresh)
                {
                    if (_EnableHairSimulation)
                        Refresh(hairJNTs, hairContainer, hairORGPositionSet, hairTargetDic);

                    if (_EnableEarring_Simulation)
                        Refresh(earringJNTs, earringContainer, earringORGPositionSet, earringTargetDic);
                }
            }

        }
    }
}
