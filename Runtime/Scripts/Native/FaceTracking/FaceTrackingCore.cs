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

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    public class FaceTrackingCore : MonoBehaviour
    {

        /// <summary>
        /// The instance
        /// </summary>
        private static FaceTrackingCore _instance = null;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static FaceTrackingCore Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject("FaceTracking Core").AddComponent<FaceTrackingCore>();
                return _instance;
            }
        }
        /// <summary>
        /// The use tracking
        /// </summary>
        private bool _useTracking = false;
        /// <summary>
        /// Gets a value indicating whether [use tracking].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use tracking]; otherwise, <c>false</c>.
        /// </value>
        public bool useTracking => _useTracking;

#if !(UNITY_EDITOR || UNITY_STANDALONE_WIN)
    /// <summary>
    /// The face tracking native
    /// </summary>
    private static AndroidJavaObject faceTrackingNative = (new AndroidJavaClass("com.samsung.avatarnative.facetracking.FaceTrackingClient")).CallStatic<AndroidJavaObject>("instance");
#elif (UNITY_EDITOR || UNITY_STANDALONE_WIN)
        private static AndroidJavaObject faceTrackingNative = null;
#endif
        /// <summary>
        /// The client handler
        /// </summary>
        private EventHandler<FaceTrackingObject> trackingHandler;
        /// <summary>
        /// Occurs when [client handler].
        /// </summary>
        public event EventHandler<FaceTrackingObject> TrackingHandler
        {
            add
            {
                trackingHandler += value;
            }
            remove
            {
                trackingHandler -= value;

            }
        }

        /// <summary>
        /// Starts the tracking.
        /// </summary>
        public void StartTracking()
        {
            if (_useTracking) return;
            if (faceTrackingNative == null)
            {
                Debug.LogError("AndroidJavaObject instance is null");
                return;
            }
            faceTrackingNative.Call("startTracking");
            _useTracking = true;
        }
        /// <summary>
        /// Stops the tracking.
        /// </summary>
        public void StopTracking()
        {
            if (!_useTracking) return;
            if (faceTrackingNative == null)
            {
                Debug.LogError("AndroidJavaObject instance is null");
                return;
            }
            _useTracking = false;
            faceTrackingNative.Call("stopTracking");
        }

        /// <summary>
        /// Gets the face data.
        /// </summary>
        /// <returns></returns>
        private FaceTrackingObject GetFaceData()
        {
            if (faceTrackingNative == null)
            {
                Debug.LogError("AndroidJavaObject instance is null");
                return null;
            }
            AndroidJavaObject face = faceTrackingNative.Call<AndroidJavaObject>("getTrackingData");
            return (face == null) ? null : new FaceTrackingObject(face.Get<float[]>("weights"), face.Get<float[]>("leftEyeRotation"), face.Get<float[]>("rightEyeRotation"));
        }

        /// <summary>
        /// Lates the update.
        /// </summary>
        private void LateUpdate()
        {
            if (_useTracking)
            {
                FaceTrackingObject face = GetFaceData();
                if (face != null)
                {
                    if (trackingHandler != null)
                        trackingHandler(this, face);
                }
            }
        }
    }
}
