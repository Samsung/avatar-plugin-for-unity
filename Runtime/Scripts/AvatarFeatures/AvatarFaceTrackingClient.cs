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
using UnityEngine;
using static AvatarPluginForUnity.AvatarComponent;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [DisallowMultipleComponent]
    public class AvatarFaceTrackingClient : MonoBehaviour
    {

        /// <summary>
        /// The RAD sec
        /// </summary>
        private static float RAD_SEC = 57.29578f;
        /// <summary>
        /// The avatar Component
        /// </summary>
        private AvatarComponent avatarComponent = null;
        /// <summary>
        /// The blendshape driver
        /// </summary>
        private AvatarBlendshapeDriver avatarblendshapeDriver;
        /// <summary>
        /// The is tracking
        /// </summary>
        private Transform l_eye_JNT = null;
        /// <summary>
        /// The r eye JNT
        /// </summary>
        private Transform r_eye_JNT = null;
        /// <summary>
        /// The tracking handler
        /// </summary>
        private Action<FaceTrackingObject> trackingHandler = null;

        void Start()
        {
            avatarComponent = GetComponent<AvatarComponent>();
            trackingHandler = new Action<FaceTrackingObject>(CallUpdateHandler);
            FaceTrackingCore.Instance.TrackingHandler += trackingHandler;
        }

        public void InitFaceTrackingClient()
        {
            Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>(true);
            foreach (Transform child_ in allChildren)
            {
                if (l_eye_JNT != null && r_eye_JNT != null)
                    break;
                else if (child_.name.Equals("r_eye_JNT"))
                    r_eye_JNT = child_;
                else if (child_.name.Equals("l_eye_JNT"))
                    l_eye_JNT = child_;
            }
            avatarblendshapeDriver = gameObject.GetComponentInChildren<AvatarBlendshapeDriver>();          
        }

        private void CallUpdateHandler(FaceTrackingObject face)
        {
            if (this.avatarblendshapeDriver == null)
            {
                Debug.Log("blendshapeDriver is null!!, Avatar should be loaded!!");
            }
            else if (face != null)
            {
                float[] weights = face.weights;
                avatarblendshapeDriver.UpdateBlendShapeWeights(weights);
                float[] leftEyeRotation = face.leftEyeRotation;
                float[] rightEyeRotation = face.rightEyeRotation;
                l_eye_JNT.localRotation = Quaternion.AngleAxis(leftEyeRotation[3] * RAD_SEC, new Vector3(leftEyeRotation[0], -leftEyeRotation[1], leftEyeRotation[2]));
                r_eye_JNT.localRotation = Quaternion.AngleAxis(rightEyeRotation[3] * RAD_SEC, new Vector3(rightEyeRotation[0], -rightEyeRotation[1], rightEyeRotation[2]));    
            }
        }

        public void ClearFaceBlendShape()
        {
            if (avatarblendshapeDriver == null)
            {
                Debug.Log("blendshapeDriver is null!!, Avatar should be loaded!!. Waiting for Initialize");
            }
            else
            {
                float[] weights = new float[AvatarBlendshapeDriver.avatarBlendshapeList.Count];
                avatarblendshapeDriver.UpdateBlendShapeWeights(weights);
                l_eye_JNT.localRotation = Quaternion.Euler(0f, -0.028f, 0f);
                r_eye_JNT.localRotation = Quaternion.Euler(0f, -0.028f, 0f);
            }
        }

        /// <summary>
        /// Called when [enable].
        /// </summary>
        void OnEnable()
        {
            if (trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler += trackingHandler;
        }

        /// <summary>
        /// Called when [disable].
        /// </summary>
        void OnDisable()
        {
            if (trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler -= trackingHandler;
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        void OnDestroy()
        {
            if (trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler -= trackingHandler;
        }
    }
}