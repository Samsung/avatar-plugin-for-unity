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
    public class AvatarBlendshapeDriver : MonoBehaviour
    {

        /// <summary>
        /// The avatar blendshape list
        /// </summary>
        public static List<string> avatarBlendshapeList = new List<string> {
        "EyeBlink_Left",
        "EyeBlink_Right",
        "EyeSquint_Left",
        "EyeSquint_Right",
        "EyeDown_Left",
        "EyeDown_Right",
        "EyeIn_Left",
        "EyeIn_Right",
        "EyeOpen_Left",
        "EyeOpen_Right",
        "EyeOut_Left",
        "EyeOut_Right",
        "EyeUp_Left",
        "EyeUp_Right",
        "BrowsDown_Left",
        "BrowsDown_Right",
        "BrowsUp_Center",
        "BrowsUp_Left",
        "BrowsUp_Right",
        "JawFwd",
        "JawLeft",
        "JawOpen",
        "JawChew",
        "JawRight",
        "MouthLeft",
        "MouthRight",
        "MouthFrown_Left",
        "MouthFrown_Right",
        "MouthSmile_Left",
        "MouthSmile_Right",
        "MouthDimple_Left",
        "MouthDimple_Right",
        "LipsStretch_Left",
        "LipsStretch_Right",
        "LipsUpperClose",
        "LipsLowerClose",
        "LipsUpperUp",
        "LipsLowerDown",
        "LipsUpperOpen",
        "LipsLowerOpen",
        "LipsFunnel",
        "LipsPucker",
        "ChinLowerRaise",
        "ChinUpperRaise",
        "Sneer",
        "Puff",
        "CheekSquint_Left",
        "CheekSquint_Right",
        "HAPPY_48",
        "HAPPY_49",
        "HAPPY_50",
        "HAPPY_51",
        "HAPPY_52",
        "ANGRY_53",
        "ANGRY_54",
        "ANGRY_55",
        "DISGUST_56",
        "DISGUST_57",
        "SAD_58",
        "SURPRISE_59",
        "SURPRISE_60",
        "Puff_Left",
        "Puff_Right",
        "Tongue_Out",
        "Tongue_Up",
        "Tongue_Down",
        "Tongue_Left",
        "Tongue_Right"
    };


        /// <summary>
        /// The associated Mesh mesh
        /// </summary>
        private SkinnedMeshRenderer associatedMesh = null;
        /// <summary>
        /// The weights
        /// </summary>
        private float[] weights = new float[avatarBlendshapeList.Count];
        /// <summary>
        /// The avatar blendshape mesh set
        /// </summary>
        private Dictionary<string, List<SkinnedMeshRenderer>> avatarBlendshapeMeshSet = new Dictionary<string, List<SkinnedMeshRenderer>>();

        // Start is called before the first frame update
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void InitBlendshapeDriver()
        {
            SetAvatarMeshSet();
            InitializeBlendshapeAssociatedMesh();
        }

        /// <summary>
        /// Initializes the blendshape associated mesh.
        /// </summary>
        private void InitializeBlendshapeAssociatedMesh()
        {
            // The attached mesh renderer is where we're storing blendshapes being used to
            // drive facial animations for avatars.
            associatedMesh = gameObject.GetComponent<SkinnedMeshRenderer>();
            if(associatedMesh==null)
                associatedMesh = gameObject.AddComponent<SkinnedMeshRenderer>();

            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[1];
            Vector3[] normals = new Vector3[1];
            Vector4[] tangents = new Vector4[1];
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.tangents = tangents;
            foreach (var blendshapeName in avatarBlendshapeList)
                mesh.AddBlendShapeFrame(blendshapeName, 1.0f, new Vector3[1], new Vector3[1], new Vector3[1]);
            associatedMesh.sharedMesh = mesh;
        }

        /// <summary>
        /// Sets the ar emoji mesh set.
        /// </summary>
        private void SetAvatarMeshSet()
        {
            Renderer[] allChildren = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer child_ in allChildren)
            {
                if (IsUsableBlendshapeMesh(child_))
                {
                    SkinnedMeshRenderer sm = (SkinnedMeshRenderer)child_;
                    foreach (var blendshapeName in avatarBlendshapeList)
                    {
                        if (sm.sharedMesh.GetBlendShapeIndex(blendshapeName) != -1)
                        {
                            if (!avatarBlendshapeMeshSet.ContainsKey(blendshapeName))
                                avatarBlendshapeMeshSet[blendshapeName] = new List<SkinnedMeshRenderer>();
                            avatarBlendshapeMeshSet[blendshapeName].Add(sm);
                        }
                    }
                }
                
            }
        }
        /// <summary>
        /// Determines whether [is head contained node] [the specified node].
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if [is head contained node] [the specified node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUsableBlendshapeMesh(Renderer node)
        {
            if (node is SkinnedMeshRenderer && ((SkinnedMeshRenderer)node).sharedMesh.blendShapeCount != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Updates the sub meshs weights.
        /// </summary>
        /// <param name="weights">The weights.</param>
        private void UpdateSubMeshsWeights(float[] weights)
        {
            if (avatarBlendshapeMeshSet.Count == 0)
            {
                Debug.Log("No blendshape target found! ");
                return;
            }
            for (int idx = 0; idx < avatarBlendshapeList.Count; ++idx)
            {
                List<SkinnedMeshRenderer> blendShapeMeshSet;
                if (avatarBlendshapeMeshSet.TryGetValue(avatarBlendshapeList[idx], out blendShapeMeshSet))
                {
                    float source = weights[idx];
                    foreach (var sm in blendShapeMeshSet)
                    {
                        if (sm == null || sm.sharedMesh == null) continue;
                        int blendshapeIdx = sm.sharedMesh.GetBlendShapeIndex(avatarBlendshapeList[idx]);
                        sm.SetBlendShapeWeight(blendshapeIdx, source);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the blend shape weights.
        /// </summary>
        /// <param name="weights">The weights.</param>
        public void UpdateBlendShapeWeights(float[] weights)
        {
            if (associatedMesh == null)
            {
                Debug.Log("No source renderer found! ");
                return;
            }
            for (int idx = 0; idx < avatarBlendshapeList.Count; ++idx)
                associatedMesh.SetBlendShapeWeight(idx, weights[idx]);

            UpdateSubMeshsWeights(weights);
        }

        /// <summary>
        /// Lates the update.
        /// </summary>
        void LateUpdate()
        {
            if (associatedMesh == null)
            {
                Debug.Log("No source renderer found! ");
                return;
            }
            for (int idx = 0; idx < avatarBlendshapeList.Count; ++idx)
                weights[idx] = associatedMesh.GetBlendShapeWeight(idx);
            UpdateSubMeshsWeights(weights);
        }
    }
}
