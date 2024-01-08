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
using System.IO;
using System.Linq;
using UnityEngine;

namespace AvatarPluginForUnity
{
    /*public class FaceAniLoader
    {

        private GameObject loadNode;
        private List<SkinnedMeshRenderer> blendShapeMeshNodes;
        private Dictionary<string, string> blendMeshPath = new Dictionary<string, string>();

        public FaceAniLoader(GameObject loadNode, List<SkinnedMeshRenderer> blendShapeMeshNodes)
        {

            this.loadNode = loadNode;
            this.blendShapeMeshNodes = blendShapeMeshNodes;
            SetBlendMeshPath();
        }

        private void SetBlendMeshPath()
        {
            foreach (SkinnedMeshRenderer so in blendShapeMeshNodes)
            {
                Transform to = so.transform;
                blendMeshPath[to.name] = FindPath(to);
            }
        }

        private string FindPath(Transform node)
        {
            if (node.name.Equals(loadNode.name))
                return "";
            string path = FindPath(node.parent);
            return (string.IsNullOrEmpty(path) ? path : (path + "/")) + node.name;
        }

        public AnimationClip CreateAnimationClip(string path)
        {
            AnimationClip mAnimationClip = new AnimationClip();
            mAnimationClip.legacy = false;
            BlendShapeAnimation jsonAnimation = JsonUtility.FromJson<BlendShapeAnimation>(File.ReadAllText(path));
            foreach (var blendShape in jsonAnimation.blendShapes)
            {
                foreach (var item_ in blendShape.morphname.Select((val, idx) => (val, idx)))
                {
                    int idx_ = item_.idx;
                    var morphname = item_.val;
                    var curve = new AnimationCurve();

                    foreach (var item__ in blendShape.key.Select((val, idx) => (val, idx)))
                    {
                        int idx__ = item__.idx;
                        var key = item__.val;
                        var time = jsonAnimation.time[idx__] / 1000.0f;
                        curve.AddKey(time, key[idx_]);
                    }
                    mAnimationClip.SetCurve(blendMeshPath[blendShape.name], typeof(SkinnedMeshRenderer), "blendShape." + morphname, curve);
                }
            }
            mAnimationClip.EnsureQuaternionContinuity();
            mAnimationClip.name = "BlendAnimation";
            mAnimationClip.legacy = false;
            mAnimationClip.wrapMode = WrapMode.Loop;
            return mAnimationClip;
        }
    }*/
}