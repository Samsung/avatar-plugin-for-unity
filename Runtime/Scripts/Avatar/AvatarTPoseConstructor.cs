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

namespace AvatarPluginForUnity
{
    public class AvatarTPoseConstructor
    {
        private class AvatarBoneInfo
        {
            public Vector3 direction = Vector3.zero;
            public bool compareInGlobalSpace = false;
            public float maxAngle;
            public int[] childIndices = null;
            public Vector3 planeNormal = Vector3.zero;
            public AvatarBoneInfo(Vector3 dir, bool globalSpace, float maxAngleDiff)
            {
                direction = (dir == Vector3.zero ? dir : dir.normalized);
                compareInGlobalSpace = globalSpace;
                maxAngle = maxAngleDiff;
            }

            public AvatarBoneInfo(Vector3 dir, bool globalSpace, float maxAngleDiff, int[] children) : this(dir, globalSpace, maxAngleDiff)
            {
                childIndices = children;
            }

            public AvatarBoneInfo(Vector3 dir, bool globalSpace, float maxAngleDiff, Vector3 planeNormal, int[] children) : this(dir, globalSpace, maxAngleDiff, children)
            {
                this.planeNormal = planeNormal;
            }
        }

        private static AvatarBoneInfo[] _sAvatarBonePoses = new AvatarBoneInfo[]
        {
        new AvatarBoneInfo(Vector3.up, true, 15),  // Hips,
        new AvatarBoneInfo(new Vector3(-0.05f, -1, 0),      true, 15),   // LeftUpperLeg,
        new AvatarBoneInfo(new Vector3(0.05f, -1, 0),      true, 15),    // RightUpperLeg,
        new AvatarBoneInfo(new Vector3(-0.05f, -1, -0.15f), true, 20),   // LeftLowerLeg,
        new AvatarBoneInfo(new Vector3(0.05f, -1, -0.15f), true, 20),    // RightLowerLeg,
        new AvatarBoneInfo(new Vector3(-0.05f, 0, 1),       true, 20, Vector3.up, null),   // LeftFoot,
        new AvatarBoneInfo(new Vector3(0.05f, 0, 1),       true, 20, Vector3.up, null),    // RightFoot,
        new AvatarBoneInfo(Vector3.up, true, 30, new int[] {(int)HumanBodyBones.Chest, (int)HumanBodyBones.UpperChest, (int)HumanBodyBones.Neck, (int)HumanBodyBones.Head}),  // Spine,
        new AvatarBoneInfo(Vector3.up, true, 30, new int[] {(int)HumanBodyBones.UpperChest, (int)HumanBodyBones.Neck, (int)HumanBodyBones.Head}),  // Chest,
        null, // Neck
        //        new AvatarBoneInfo(Vector3.up, true, 30),  // Neck,
        null, // Head,
        new AvatarBoneInfo(-Vector3.right, true, 20),  // LeftShoulder,
        new AvatarBoneInfo(Vector3.right, true, 20),   // RightShoulder,
        new AvatarBoneInfo(-Vector3.right, true, 05),  // LeftArm,
        new AvatarBoneInfo(Vector3.right, true, 05),   // RightArm,
        new AvatarBoneInfo(-Vector3.right, true, 05),  // LeftForeArm,
        new AvatarBoneInfo(Vector3.right, true, 05),   // RightForeArm,
        new AvatarBoneInfo(-Vector3.right, false, 10, Vector3.forward, new int[] {(int)HumanBodyBones.LeftMiddleProximal}),  // LeftHand,
        new AvatarBoneInfo(Vector3.right, false, 10, Vector3.forward, new int[] {(int)HumanBodyBones.RightMiddleProximal}),   // RightHand,
        null, // LeftToes,
        null, // RightToes,
        null, // LeftEye,
        null, // RightEye,
        null, // Jaw,
        new AvatarBoneInfo(new Vector3(-1, 0, 1), false, 10), // Left Thumb
        new AvatarBoneInfo(new Vector3(-1, 0, 1), false, 05),
        new AvatarBoneInfo(new Vector3(-1, 0, 1), false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 10),  // Left Index
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 10),  // Left Middle
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 10),  // Left Ring
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 10),  // Left Little
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(-Vector3.right, false, 05),
        new AvatarBoneInfo(new Vector3(1, 0, 1), false, 10),  // Right Thumb
        new AvatarBoneInfo(new Vector3(1, 0, 1), false, 05),
        new AvatarBoneInfo(new Vector3(1, 0, 1), false, 05),
        new AvatarBoneInfo(Vector3.right, false, 10),   // Right Index
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 10),   // Right Middle
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 10),   // Right Ring
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 10),   // Right Little
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.right, false, 05),
        new AvatarBoneInfo(Vector3.up, true, 30, new int[] {(int)HumanBodyBones.Neck, (int)HumanBodyBones.Head}),  // UpperChest,
        };

        public AvatarBone[] MakePoseValid(GameObject modelInstance, Dictionary<string, string> existingMappings)
        {
            Dictionary<Transform, bool> modelBones = GetModelBones(modelInstance.transform, false, null);
            if (modelBones == null)
            {
                Debug.LogWarning("modelBones has null value");
                return null;
            }
            AvatarBone[] avatarBones = GetHumanBones(existingMappings, modelBones);
            Quaternion orientation = AvatarComputeOrientation(avatarBones);

            for (int i = 0; i < _sAvatarBonePoses.Length; i++)
            {
                MakeBoneAlignmentValid(avatarBones, orientation, i);
                // Recalculate orientation after handling hips since they may have changed it
                if (i == (int)HumanBodyBones.Hips)
                    orientation = AvatarComputeOrientation(avatarBones);
            }

            // Move feet to ground plane
            MakeCharacterPositionValid(avatarBones);
            return avatarBones;
        }

        private AvatarBone[] GetHumanBones(Dictionary<string, string> existingMappings, Dictionary<Transform, bool> actualBones)
        {
            string[] humanBoneNames = HumanTrait.BoneName;
            AvatarBone[] avatarBones = new AvatarBone[humanBoneNames.Length];
            for (int i = 0; i < humanBoneNames.Length; i++)
            {
                Transform bone = null;

                var humanBoneName = humanBoneNames[i];
                if (existingMappings?.ContainsKey(humanBoneName) == true)
                {
                    string boneName = existingMappings[humanBoneName];
                    bone = actualBones.Keys.FirstOrDefault(b => (b?.name == boneName));
                }
                avatarBones[i] = new AvatarBone(bone);
            }
            return avatarBones;
        }

        private Dictionary<Transform, bool> GetModelBones(Transform root, bool includeAll, AvatarBone[] humanAvatarBones)
        {
            if (root == null)
                return null;

            // Find out which transforms are actual avatarBones and which are parents of actual avatarBones
            Dictionary<Transform, bool> bones = new Dictionary<Transform, bool>();
            List<Transform> skinnedBones = new List<Transform>();

            if (!includeAll)
            {
                // Find out in advance which avatarBones are used by SkinnedMeshRenderers
                SkinnedMeshRenderer[] skinnedMeshRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                foreach (SkinnedMeshRenderer rend in skinnedMeshRenderers)
                {
                    Transform[] meshBones = rend.bones;
                    bool[] meshBonesUsed = new bool[meshBones.Length];
                    BoneWeight[] weights = rend.sharedMesh.boneWeights;
                    foreach (BoneWeight w in weights)
                    {
                        if (w.weight0 != 0)
                            meshBonesUsed[w.boneIndex0] = true;
                        if (w.weight1 != 0)
                            meshBonesUsed[w.boneIndex1] = true;
                        if (w.weight2 != 0)
                            meshBonesUsed[w.boneIndex2] = true;
                        if (w.weight3 != 0)
                            meshBonesUsed[w.boneIndex3] = true;
                    }
                    for (int i = 0; i < meshBones.Length; i++)
                    {
                        if (meshBonesUsed[i])
                            if (!skinnedBones.Contains(meshBones[i]))
                                skinnedBones.Add(meshBones[i]);
                    }
                }

                // Recursive call
                DetermineIsActualBone(root, bones, skinnedBones, false, humanAvatarBones);
            }

            // If not enough avatarBones were found, fallback to treating all transforms as avatarBones
            if (bones.Count < HumanTrait.RequiredBoneCount)
            {
                bones.Clear();
                skinnedBones.Clear();
                DetermineIsActualBone(root, bones, skinnedBones, true, humanAvatarBones);
            }

            return bones;
        }

        private bool DetermineIsActualBone(Transform tr, Dictionary<Transform, bool> bones, List<Transform> skinnedBones, bool includeAll, AvatarBone[] humanAvatarBones)
        {
            bool actualBone = includeAll;
            bool boneParent = false;
            bool boneChild = false;

            // Actual avatarBone parent if any of children are avatarBones
            int childBones = 0;
            foreach (Transform child in tr)
                if (DetermineIsActualBone(child, bones, skinnedBones, includeAll, humanAvatarBones))
                    childBones++;

            if (childBones > 0)
                boneParent = true;
            if (childBones > 1)
                actualBone = true;

            // Actual avatarBone if used by skinned mesh
            if (!actualBone)
                if (skinnedBones.Contains(tr))
                    actualBone = true;

            // Actual avatarBone if contains component other than transform
            if (!actualBone)
            {
                Component[] components = tr.GetComponents<Component>();
                if (components.Length > 1)
                {
                    foreach (Component comp in components)
                    {
                        if ((comp is Renderer) && !(comp is SkinnedMeshRenderer))
                        {
                            Bounds bounds = (comp as Renderer).bounds;

                            // Double size of bounds in order to still make avatarBone valid
                            // if its pivot is just slightly outside of renderer bounds.
                            bounds.extents = bounds.size;

                            // If the parent is inside the bounds, this transform is probably
                            // just a geometry dummy for the parent avatarBone
                            if (tr.childCount == 0 && tr.parent && bounds.Contains(tr.parent.position))
                            {
                                if (tr.parent.GetComponent<Renderer>() != null)
                                    actualBone = true;
                                else
                                    boneChild = true;
                            }
                            // if not, give transform itself a chance.
                            // If pivot is way outside of bounds, it's not an actual avatarBone.
                            else if (bounds.Contains(tr.position))
                            {
                                actualBone = true;
                            }
                        }
                    }
                }
            }

            // Actual avatarBone if the avatarBone is define in human definition.
            if (!actualBone && humanAvatarBones != null)
            {
                foreach (var boneWrapper in humanAvatarBones)
                {
                    if (tr == boneWrapper.bone)
                    {
                        actualBone = true;
                        break;
                    }
                }
            }

            if (actualBone)
                bones[tr] = true;
            else if (boneParent)
            {
                if (!bones.ContainsKey(tr))
                    bones[tr] = false;
            }
            else if (boneChild)
                bones[tr.parent] = true;

            return bones.ContainsKey(tr);
        }

        private Quaternion AvatarComputeOrientation(AvatarBone[] avatarBones)
        {
            Transform leftUpLeg = avatarBones[(int)HumanBodyBones.LeftUpperLeg].bone;
            Transform rightUpLeg = avatarBones[(int)HumanBodyBones.RightUpperLeg].bone;
            Transform leftArm = avatarBones[(int)HumanBodyBones.LeftUpperArm].bone;
            Transform rightArm = avatarBones[(int)HumanBodyBones.RightUpperArm].bone;
            if (leftUpLeg != null && rightUpLeg != null && leftArm != null && rightArm != null)
                return AvatarComputeOrientation(leftUpLeg.position, rightUpLeg.position, leftArm.position, rightArm.position);
            else
                return Quaternion.identity;
        }

        private Quaternion AvatarComputeOrientation(Vector3 leftUpLeg, Vector3 rightUpLeg, Vector3 leftArm, Vector3 rightArm)
        {
            Vector3 legsRightDir = Vector3.Normalize(rightUpLeg - leftUpLeg);
            Vector3 armsRightDir = Vector3.Normalize(rightArm - leftArm);
            Vector3 torsoRightDir = Vector3.Normalize(legsRightDir + armsRightDir);

            // Find out if torso right dir seems sensible or completely arbitrary.
            // It's sensible if it's aligned along some axis.
            bool sensibleOrientation =
                Mathf.Abs(torsoRightDir.x * torsoRightDir.y) < 0.05f &&
                Mathf.Abs(torsoRightDir.y * torsoRightDir.z) < 0.05f &&
                Mathf.Abs(torsoRightDir.z * torsoRightDir.x) < 0.05f;

            Vector3 legsAvgPos = (leftUpLeg + rightUpLeg) * 0.5f;
            Vector3 armsAvgPos = (leftArm + rightArm) * 0.5f;
            Vector3 torsoUpDir = Vector3.Normalize(armsAvgPos - legsAvgPos);

            // If the orientation is sensible, assume character up vector is aligned along x, y, or z axis, so fix it to closest axis
            if (sensibleOrientation)
            {
                int axisIndex = 0;
                for (int i = 1; i < 3; i++)
                    if (Mathf.Abs(torsoUpDir[i]) > Mathf.Abs(torsoUpDir[axisIndex]))
                        axisIndex = i;
                float sign = Mathf.Sign(torsoUpDir[axisIndex]);
                torsoUpDir = Vector3.zero;
                torsoUpDir[axisIndex] = sign;
            }

            Vector3 torsoForwardDir = Vector3.Cross(torsoRightDir, torsoUpDir);

            if (torsoForwardDir == Vector3.zero || torsoUpDir == Vector3.zero)
                return Quaternion.identity;

            return Quaternion.LookRotation(torsoForwardDir, torsoUpDir);
        }

        private void MakeBoneAlignmentValid(AvatarBone[] avatarBones, Quaternion avatarOrientation, int boneIndex)
        {
            if (boneIndex < 0 || boneIndex >= _sAvatarBonePoses.Length || boneIndex >= avatarBones.Length)
                return;

            AvatarBone avatarBone = avatarBones[boneIndex];
            AvatarBoneInfo pose = _sAvatarBonePoses[boneIndex];

            if (avatarBone.bone == null || pose == null)
                return;

            if (boneIndex == (int)HumanBodyBones.Hips)
            {
                float angleX = Vector3.Angle(avatarOrientation * Vector3.right, Vector3.right);
                float angleY = Vector3.Angle(avatarOrientation * Vector3.up, Vector3.up);
                float angleZ = Vector3.Angle(avatarOrientation * Vector3.forward, Vector3.forward);
                if (angleX > pose.maxAngle || angleY > pose.maxAngle || angleZ > pose.maxAngle)
                    avatarBone.bone.rotation = Quaternion.Inverse(avatarOrientation) * avatarBone.bone.rotation;
                return;
            }

            Vector3 dir = GetBoneAlignmentDirection(avatarBones, avatarOrientation, boneIndex);
            if (dir == Vector3.zero)
                return;

            Quaternion space = GetRotationSpace(avatarBones, avatarOrientation, boneIndex);
            Vector3 goalDir = space * pose.direction;

            if (pose.planeNormal != Vector3.zero)
                dir = Vector3.ProjectOnPlane(dir, space * pose.planeNormal);

            // If the avatarBone direction is not close enough to the target direction,
            // rotate it so it matches the target direction.
            float deltaAngle = Vector3.Angle(dir, goalDir);

            //        if(boneIndex == (int)HumanBodyBones.Neck)
            //            return;

            if (deltaAngle > pose.maxAngle * 0.99f)
            {
                Quaternion adjust = Quaternion.FromToRotation(dir, goalDir);

                // If this avatarBone is hip or knee, remember global foor rotation and apply it after this adjustment
                Transform footBone = null;
                Quaternion footRot = Quaternion.identity;
                if (boneIndex == (int)HumanBodyBones.LeftUpperLeg || boneIndex == (int)HumanBodyBones.LeftLowerLeg)
                    footBone = avatarBones[(int)HumanBodyBones.LeftFoot].bone;
                if (boneIndex == (int)HumanBodyBones.RightUpperLeg || boneIndex == (int)HumanBodyBones.RightLowerLeg)
                    footBone = avatarBones[(int)HumanBodyBones.RightFoot].bone;
                if (footBone != null)
                    footRot = footBone.rotation;

                // Adjust only enough to fall within maxAngle
                float adjustAmount = Mathf.Clamp01(1.05f - (pose.maxAngle / deltaAngle));

                adjust = Quaternion.Slerp(Quaternion.identity, adjust, adjustAmount);

                //            Debug.Log("Adjusting: " + avatarBone.bone.name);
                avatarBone.bone.rotation = adjust * avatarBone.bone.rotation;

                // Revert foot rotation to what it was
                if (footBone != null)
                    footBone.rotation = footRot;
            }
        }

        private Vector3 GetBoneAlignmentDirection(AvatarBone[] avatarBones, Quaternion avatarOrientation, int boneIndex)
        {
            if (_sAvatarBonePoses[boneIndex] == null)
                return Vector3.zero;

            AvatarBone avatarBone = avatarBones[boneIndex];
            Vector3 dir;

            // Get the child avatarBone
            AvatarBoneInfo pose = _sAvatarBonePoses[boneIndex];
            int childBoneIndex = -1;
            if (pose.childIndices != null)
            {
                foreach (int i in pose.childIndices)
                {
                    if (avatarBones[i].bone != null)
                    {
                        childBoneIndex = i;
                        break;
                    }
                }
            }
            else
            {
                childBoneIndex = GetHumanBoneChild(avatarBones, boneIndex);
            }

            // TODO@MECANIM Something si wrong with the indexes
            //if (boneIndex == (int)HumanBodyBones.LeftHand)
            //  Debug.Log ("Child avatarBone for left hand: "+childBoneIndex);

            if (childBoneIndex >= 0 && avatarBones[childBoneIndex] != null && avatarBones[childBoneIndex].bone != null)
            {
                // Get direction from avatarBone to child
                AvatarBone childAvatarBone = avatarBones[childBoneIndex];
                dir = childAvatarBone.bone.position - avatarBone.bone.position;

                // TODO@MECANIM Something si wrong with the indexes
                //if (boneIndex == (int)HumanBodyBones.LeftHand)
                //  Debug.Log (" - "+childAvatarBone.humanBoneName + " - " +childAvatarBone.avatarBone.name);
            }
            else
            {
                if (avatarBone.bone.childCount != 1)
                    return Vector3.zero;

                dir = Vector3.zero;
                // Get direction from avatarBone to child
                foreach (Transform child in avatarBone.bone)
                {
                    dir = child.position - avatarBone.bone.position;
                    break;
                }
            }

            return dir.normalized;
        }

        private int GetHumanBoneChild(AvatarBone[] avatarBones, int boneIndex)
        {
            for (int i = 0; i < HumanTrait.BoneCount; i++)
                if (HumanTrait.GetParentBone(i) == boneIndex)
                    return i;
            return -1;
        }

        private Quaternion GetRotationSpace(AvatarBone[] avatarBones, Quaternion avatarOrientation, int boneIndex)
        {
            Quaternion parentDelta = Quaternion.identity;
            AvatarBoneInfo pose = _sAvatarBonePoses[boneIndex];
            if (!pose.compareInGlobalSpace)
            {
                int parentIndex = HumanTrait.GetParentBone(boneIndex);

                if (parentIndex > 0)
                {
                    AvatarBoneInfo parentPose = _sAvatarBonePoses[parentIndex];
                    if (avatarBones[parentIndex].bone != null && parentPose != null)
                    {
                        Vector3 parentDir = GetBoneAlignmentDirection(avatarBones, avatarOrientation, parentIndex);
                        if (parentDir != Vector3.zero)
                        {
                            Vector3 parentPoseDir = avatarOrientation * parentPose.direction;
                            parentDelta = Quaternion.FromToRotation(parentPoseDir, parentDir);
                        }
                    }
                }
            }

            return parentDelta * avatarOrientation;
        }

        private void MakeCharacterPositionValid(AvatarBone[] avatarBones)
        {
            float error;
            Vector3 adjustVector = GetCharacterPositionAdjustVector(avatarBones, out error);
            if (adjustVector != Vector3.zero)
                avatarBones[(int)HumanBodyBones.Hips].bone.position += adjustVector;
        }

        private Vector3 GetCharacterPositionAdjustVector(AvatarBone[] avatarBones, out float error)
        {
            error = 0;

            // Get hip avatarBones
            Transform leftUpLeg = avatarBones[(int)HumanBodyBones.LeftUpperLeg].bone;
            Transform rightUpLeg = avatarBones[(int)HumanBodyBones.RightUpperLeg].bone;
            if (leftUpLeg == null || rightUpLeg == null)
                return Vector3.zero;
            Vector3 avgHipPos = (leftUpLeg.position + rightUpLeg.position) * 0.5f;

            // Get foot avatarBones
            // Prefer toe avatarBones but use foot avatarBones if toes are not mapped
            bool usingToes = true;
            Transform leftFoot = avatarBones[(int)HumanBodyBones.LeftToes].bone;
            Transform rightFoot = avatarBones[(int)HumanBodyBones.RightToes].bone;
            if (leftFoot == null || rightFoot == null)
            {
                usingToes = false;
                leftFoot = avatarBones[(int)HumanBodyBones.LeftFoot].bone;
                rightFoot = avatarBones[(int)HumanBodyBones.RightFoot].bone;
            }
            if (leftFoot == null || rightFoot == null)
                return Vector3.zero;
            Vector3 avgFootPos = (leftFoot.position + rightFoot.position) * 0.5f;

            // Get approximate length of legs
            float hipsHeight = avgHipPos.y - avgFootPos.y;
            if (hipsHeight <= 0)
                return Vector3.zero;

            Vector3 adjustVector = Vector3.zero;

            // We can force the feet to be at an approximate good height.
            // But the feet might be at a perfect height from the start if the bind pose is good.
            // So only do it if the feet look like they're not at a good position from the beginning.
            // Check if feet are already at height that looks about right.
            if (avgFootPos.y < 0 || avgFootPos.y > hipsHeight * (usingToes ? 0.1f : 0.3f))
            {
                // Current height is not good, so adjust it using best guess based on human anatomy.
                float estimatedFootBottomHeight = avgHipPos.y - hipsHeight * (usingToes ? 1.03f : 1.13f);
                adjustVector.y = -estimatedFootBottomHeight;
            }

            // Move the avg hip pos to the center on the left-right axis if it's not already there.
            if (Mathf.Abs(avgHipPos.x) > 0.01f * hipsHeight)
                adjustVector.x = -avgHipPos.x;

            // Move the avg hip pos to the center on the front-back axis if it's not already approximately there.
            if (Mathf.Abs(avgHipPos.z) > 0.2f * hipsHeight)
                adjustVector.z = -avgHipPos.z;

            error = adjustVector.magnitude * 100 / hipsHeight;
            return adjustVector;
        }
    }

    public class AvatarBone
    {
        public Transform bone;

        public AvatarBone(Transform bone)
        {
            this.bone = bone;
        }
    }

}