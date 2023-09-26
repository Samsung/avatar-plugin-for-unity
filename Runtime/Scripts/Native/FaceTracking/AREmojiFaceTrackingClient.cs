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
using System.Collections;
using UnityEngine;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AREmojiComponent))]
    public class AREmojiFaceTrackingClient : MonoBehaviour
    {

        /// <summary>
        /// The RAD sec
        /// </summary>
        private static float RAD_SEC = 57.29578f;
        /// <summary>
        /// The aremoji Component
        /// </summary>
        private AREmojiComponent aremojiComponent = null;
        /// <summary>
        /// The blendshape driver
        /// </summary>
        private AREmojiBlendshapeDriver aremojiblendshapeDriver;
        /// <summary>
        /// The is tracking
        /// </summary>
        private Transform l_eye_JNT = null;
        /// <summary>
        /// The r eye JNT
        /// </summary>
        private Transform r_eye_JNT = null;
        /// <summary>
        /// The is initializing
        /// </summary>
        private bool IsInitializing = false;
        /// <summary>
        /// The tracking handler
        /// </summary>
        private EventHandler<FaceTrackingObject> trackingHandler = null;


        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Start()
        {
            aremojiComponent = GetComponent<AREmojiComponent>();
            trackingHandler = new EventHandler<FaceTrackingObject>(CallUpdateHandler);
            FaceTrackingCore.Instance.TrackingHandler += CallUpdateHandler;
            StartCoroutine(InitFaceTrackingClient());
        }

        /// <summary>
        /// Initializes the face tracking client.
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitFaceTrackingClient()
        {
            if (!IsInitializing)
            {
                IsInitializing = true;
                yield return new WaitUntil(() => (!aremojiComponent.isLoading && aremojiComponent.loadNode != null));

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
                aremojiblendshapeDriver = gameObject.GetComponentInChildren<AREmojiBlendshapeDriver>();
                aremojiblendshapeDriver.isTracking = true;
                IsInitializing = false;
            }
        }

        /// <summary>
        /// Calls the update handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="face">The face.</param>
        private void CallUpdateHandler(object sender, FaceTrackingObject face)
        {
            if (this.aremojiblendshapeDriver == null || IsInitializing)
            {
                Debug.LogError("blendshapeDriver is null!!, AREmoji should be loaded!!. Waiting for Initialize");
                StartCoroutine(InitFaceTrackingClient());
            }
            else
            {
                if (face != null)
                {
                    float[] weights = face.weights;

                    aremojiblendshapeDriver.FaceTackingUpdate(weights);

                    float[] leftEyeRotation = face.leftEyeRotation;
                    float[] rightEyeRotation = face.rightEyeRotation;
                    l_eye_JNT.localRotation = Quaternion.AngleAxis(leftEyeRotation[3] * RAD_SEC, new Vector3(leftEyeRotation[0], -leftEyeRotation[1], leftEyeRotation[2]));
                    r_eye_JNT.localRotation = Quaternion.AngleAxis(rightEyeRotation[3] * RAD_SEC, new Vector3(rightEyeRotation[0], -rightEyeRotation[1], rightEyeRotation[2]));
                }
            }
        }

        public void UpdateFaceBlendShape(FaceTrackingObject face)
        {
            if (aremojiblendshapeDriver == null || IsInitializing)
            {
                Debug.LogError("blendshapeDriver is null!!, AREmoji should be loaded!!. Waiting for Initialize");
                StartCoroutine(InitFaceTrackingClient());
            }
            else if (face != null)
            {
                float[] weights = face.weights;
                aremojiblendshapeDriver.FaceTackingUpdate(weights);
                float[] leftEyeRotation = face.leftEyeRotation;
                float[] rightEyeRotation = face.rightEyeRotation;
                l_eye_JNT.localRotation = Quaternion.AngleAxis(leftEyeRotation[3] * RAD_SEC, new Vector3(leftEyeRotation[0], 0f - leftEyeRotation[1], leftEyeRotation[2]));
                r_eye_JNT.localRotation = Quaternion.AngleAxis(rightEyeRotation[3] * RAD_SEC, new Vector3(rightEyeRotation[0], 0f - rightEyeRotation[1], rightEyeRotation[2]));
            }
        }

        public void ClearFaceBlendShape()
        {
            if (aremojiblendshapeDriver == null || IsInitializing)
            {
                Debug.LogError("blendshapeDriver is null!!, AREmoji should be loaded!!. Waiting for Initialize");
                StartCoroutine(InitFaceTrackingClient());
            }
            else
            {
                float[] weights = new float[AREmojiBlendshapeDriver.aremojiBlendshapeList.Count];
                aremojiblendshapeDriver.FaceTackingUpdate(weights);
                l_eye_JNT.localRotation = Quaternion.Euler(0f, -0.028f, 0f);
                r_eye_JNT.localRotation = Quaternion.Euler(0f, -0.028f, 0f);
            }
        }

        /// <summary>
        /// Called when [enable].
        /// </summary>
        void OnEnable()
        {
            if(trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler += trackingHandler;
            if (aremojiblendshapeDriver != null)
                aremojiblendshapeDriver.isTracking = true;
        }

        /// <summary>
        /// Called when [disable].
        /// </summary>
        void OnDisable()
        {
            if (trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler -= trackingHandler;
            if (aremojiblendshapeDriver != null)
                aremojiblendshapeDriver.isTracking = false;
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        void OnDestroy()
        {
            if (trackingHandler != null)
                FaceTrackingCore.Instance.TrackingHandler -= trackingHandler;
            if (aremojiblendshapeDriver != null)
                aremojiblendshapeDriver.isTracking = false;
        }
    }
}