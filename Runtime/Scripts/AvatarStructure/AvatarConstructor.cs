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
using System.Collections.Generic;
using UnityEngine;

namespace AvatarPluginForUnity
{
    public class AvatarConstructor
    {
      
        private static Dictionary<string, string> boneStructure = new Dictionary<string, string>
        {
            ["Hips"] = "hips_JNT",
            ["RightUpperLeg"] = "r_upleg_JNT",
            ["RightLowerLeg"] = "r_leg_JNT",
            ["RightFoot"] = "r_foot_JNT",
            ["RightToes"] = "r_toebase_JNT",
            ["Spine"] = "spine_JNT",
            ["Chest"] = "spine1_JNT",
            ["UpperChest"] = "spine2_JNT",
            ["RightShoulder"] = "r_shoulder_JNT",
            ["RightUpperArm"] = "r_arm_JNT",
            ["RightLowerArm"] = "r_forearm_JNT",
            ["RightHand"] = "r_hand_JNT",
            ["Neck"] = "neck_JNT",
            ["Head"] = "head_JNT",
            //["RightEye"] = "r_eye_JNT",
            //["LeftEye"] = "l_eye_JNT",
            ["LeftShoulder"] = "l_shoulder_JNT",
            ["LeftUpperArm"] = "l_arm_JNT",
            ["LeftLowerArm"] = "l_forearm_JNT",
            ["LeftHand"] = "l_hand_JNT",
            ["LeftUpperLeg"] = "l_upleg_JNT",
            ["LeftLowerLeg"] = "l_leg_JNT",
            ["LeftFoot"] = "l_foot_JNT",
            ["LeftToes"] = "l_toebase_JNT",
            ["Left Thumb Proximal"] = "l_handThumb1_JNT",
            ["Left Thumb Intermediate"] = "l_handThumb2_JNT",
            ["Left Thumb Distal"] = "l_handThumb3_JNT",
            ["Left Index Proximal"] = "l_handIndex1_JNT",
            ["Left Index Intermediate"] = "l_handIndex2_JNT",
            ["Left Index Distal"] = "l_handIndex3_JNT",
            ["Left Middle Proximal"] = "l_handMiddle1_JNT",
            ["Left Middle Intermediate"] = "l_handMiddle2_JNT",
            ["Left Middle Distal"] = "l_handMiddle3_JNT",
            ["Left Ring Proximal"] = "l_handRing1_JNT",
            ["Left Ring Intermediate"] = "l_handRing2_JNT",
            ["Left Ring Distal"] = "l_handRing3_JNT",
            ["Left Little Proximal"] = "l_handPinky1_JNT",
            ["Left Little Intermediate"] = "l_handPinky2_JNT",
            ["Left Little Distal"] = "l_handPinky3_JNT",
            ["Right Thumb Proximal"] = "r_handThumb1_JNT",
            ["Right Thumb Intermediate"] = "r_handThumb2_JNT",
            ["Right Thumb Distal"] = "r_handThumb3_JNT",
            ["Right Index Proximal"] = "r_handIndex1_JNT",
            ["Right Index Intermediate"] = "r_handIndex2_JNT",
            ["Right Index Distal"] = "r_handIndex3_JNT",
            ["Right Middle Proximal"] = "r_handMiddle1_JNT",
            ["Right Middle Intermediate"] = "r_handMiddle2_JNT",
            ["Right Middle Distal"] = "r_handMiddle3_JNT",
            ["Right Ring Proximal"] = "r_handRing1_JNT",
            ["Right Ring Intermediate"] = "r_handRing2_JNT",
            ["Right Ring Distal"] = "r_handRing3_JNT",
            ["Right Little Proximal"] = "r_handPinky1_JNT",
            ["Right Little Intermediate"] = "r_handPinky2_JNT",
            ["Right Little Distal"] = "r_handPinky3_JNT"
        };

        private GameObject _gameObject;
        private Transform _transform;
        private AvatarBone[] _avatarBone;
        private HumanBone[] _humanBones;
        private SkeletonBone[] _skeletonBones;

        private float _armStretch = 0.05f;
        private float _feetSpacing = 0f;
        private float _legStretch = 0.05f;
        private float _lowerArmTwist = 0.5f;
        private float _lowerLegTwist = 0.5f;
        private float _upperArmTwist = 0.5f;
        private float _upperLegTwist = 0.5f;
        private bool _hasTranslationDoF = false;

        public AvatarConstructor(GameObject gameObject)
        {
            _gameObject = gameObject;
            var originalPosition = gameObject.transform.position;
            var originalRotation = gameObject.transform.rotation;
            var originalScale = gameObject.transform.localScale;
            var parent = gameObject.transform.parent;

            gameObject.transform.SetParent(null);
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.eulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;

            CheckBodyParts();

            _transform = gameObject.transform;
            var humanoidSet = new AvatarTPoseConstructor();
            _avatarBone = humanoidSet.MakePoseValid(gameObject, boneStructure);
            _humanBones = ConstructAvatarHumanBone();
            _skeletonBones = ConstructAvatarSkeleton();

            UpdateSkeletonBonesToTPose();
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = originalPosition;
            gameObject.transform.rotation = originalRotation;
            gameObject.transform.localScale = originalScale;
        }

        public Avatar ConstructAvatar()
        {
            var humanDescription = new HumanDescription
            {
                skeleton = _skeletonBones,
                human = _humanBones,
                armStretch = _armStretch,
                feetSpacing = _feetSpacing,
                legStretch = _legStretch,
                lowerArmTwist = _lowerArmTwist,
                lowerLegTwist = _lowerLegTwist,
                upperArmTwist = _upperArmTwist,
                upperLegTwist = _upperLegTwist,
                hasTranslationDoF = _hasTranslationDoF
            };
            return AvatarBuilder.BuildHumanAvatar(_gameObject, humanDescription);
        }

        private SkeletonBone[] ConstructAvatarSkeleton()
        {
            var childTransforms = _transform.GetComponentsInChildren<Transform>(true);
            var avatarSkeletonBone = new SkeletonBone[childTransforms.Length];
            var i = 0;

            foreach (var childTransform in childTransforms)
            {
                avatarSkeletonBone[i].name = childTransform.name;
                avatarSkeletonBone[i].position = childTransform.localPosition;
                avatarSkeletonBone[i].rotation = childTransform.localRotation;
                avatarSkeletonBone[i].scale = childTransform.localScale;
                i++;
            }

            return avatarSkeletonBone;
        }

        private void UpdateSkeletonBonesToTPose()
        {
            if (_avatarBone == null)
            {
                Debug.LogWarning("_avatarBone has null value");
                return;
            }

            foreach (var bone in _avatarBone)
            {
                if (bone.bone == null)
                    continue;

                for (int i = 0; i < _skeletonBones.Length; i++)
                {
                    if (bone.bone.name.Equals(_skeletonBones[i].name))
                    {
                        _skeletonBones[i].position = bone.bone.localPosition;
                        _skeletonBones[i].rotation = bone.bone.localRotation;
                    }
                }
            }
        }

        private HumanBone[] ConstructAvatarHumanBone()
        {
            var avatarHumanBone = new HumanBone[boneStructure.Count];
            var i = 0;

            foreach (var bone in boneStructure)
            {
                avatarHumanBone[i].humanName = bone.Key;
                avatarHumanBone[i].boneName = bone.Value;
                avatarHumanBone[i].limit = new HumanLimit
                {
                    center = Vector3.zero,
                    max = Vector3.zero,
                    min = Vector3.zero,
                    axisLength = 0.0f,
                    useDefaultValues = true
                };

                i++;
            }
            return avatarHumanBone;
        }

        private void CheckBodyParts()
        {
            List<String> keysToDelete = new List<string>();

            foreach (var bone in boneStructure)
            {
                var child = FindDeepChild(_gameObject.transform, bone.Value);

                if (child != null)
                    continue;

                Debug.LogWarning("Avatar constructor couldn't find: " + bone.Key);
                keysToDelete.Add(bone.Key);
            }

            foreach (var key in keysToDelete)
            {
                boneStructure.Remove(key);
            }
        }

        private Transform FindDeepChild(Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);

            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }
    }
}