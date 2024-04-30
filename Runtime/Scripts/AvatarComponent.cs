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
using GLTFast;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using AvatarPluginForUnity.Editor;
#endif
using static AvatarPluginForUnity.AvatarBoneConstructor;
using static AvatarPluginForUnity.AvatarMeshComposer;
using static AvatarPluginForUnity.LoadDefines;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(Animator))]
    public class AvatarComponent : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private enum LoadStatus
        {
            /// <summary>
            /// The notstarted
            /// </summary>
            NOTSTARTED,
            /// <summary>
            /// The inprogress
            /// </summary>
            INPROGRESS,
            /// <summary>
            /// The gltfloaded
            /// </summary>
            GLTFLOADED,
            /// <summary>
            /// The done
            /// </summary>
            DONE,
        }

        /// <summary>
        /// The load on start up
        /// </summary>
        [Header("[Load Option]")]
        [SerializeField] private bool loadOnStartUp;
        /// <summary>
        /// The asset location
        /// </summary>
        [SerializeField] private AssetLocation assetLocation;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The load type
        /// </summary>
        [DrawIf("assetLocation", AssetLocation.Server, DrawIfAttribute.DisablingType.DrawExclude)]
#endif
        [SerializeField] private LoadType loadType;
        /// <summary>
        /// The URL
        /// </summary>
        [SerializeField] private string Url;
        /// <summary>
        /// The use mesh combine
        /// </summary>
        [Header("[Render Option]")]
        [SerializeField] private bool useMeshCombine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The mesh combine option
        /// </summary>
        [DrawIf("useMeshCombine", true, DrawIfAttribute.DisablingType.Draw)]
#endif
        [SerializeField] private CombineOption meshCombineOption;


        [Header("[Animation Option]")]
        /// <summary>
        /// The animation type
        /// </summary>
        [SerializeField] private AnimationType animatorType;
        /// <summary>
        /// The load node
        /// </summary>
        private GameObject loadNode = null;
        /// <summary>
        /// The m GLTF transforms
        /// </summary>
        private Dictionary<string, Transform> m_gltfTransforms = null;
        /// <summary>
        /// The avatar bone constructor
        /// </summary>
        private AvatarBoneConstructor avatarBoneConstructor = null;
        /// <summary>
        /// The avatar mesh composer
        /// </summary>
        private AvatarMeshComposer avatarMeshComposer = null;
        /// <summary>
        /// The avatar blendshape driver
        /// </summary>
        private AvatarBlendshapeDriver avatarBlendshapeDriver = null;
        /// <summary>
        /// The in progress feature event
        /// </summary>
        [Header("[Load Status Event]")]
        [SerializeField]
        public UnityEvent InProgressFeatureEvent;

        /// <summary>
        /// The GLTF loaded feature event
        /// </summary>
        [SerializeField]
        public UnityEvent GltfLoadedFeatureEvent;

        /// <summary>
        /// The done feature event
        /// </summary>
        [SerializeField]
        public UnityEvent DoneFeatureEvent;
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        private LoadStatus loadstatus = LoadStatus.NOTSTARTED;

        /// <summary>
        /// The full path
        /// </summary>
        private string fullPath = String.Empty;
        /// <summary>
        /// The animator
        /// </summary>
        private Animator _animator = null;
        /// <summary>
        /// The GLTF asset
        /// </summary>
        private GltfAsset gltfAsset = null;
        /// <summary>
        /// Awakes this instance.
        /// </summary>
        void Awake()
        {
            if (Url != null)
            {
                Debug.Log("[profiling] Url = " + Url);
                switch (assetLocation)
                {
                    case LoadDefines.AssetLocation.StreamingAsset: fullPath = getStreamingAssetFullPath(); break;
                    case AssetLocation.Else: fullPath = getElseAssetLocationFullPath(); break;
                    case AssetLocation.Server: fullPath = Url; break;
                }

                // load on start up
                if (loadOnStartUp)
                {
                    if (assetLocation == AssetLocation.Server)
                    {
                        loadType = LoadType.Url; // url load type is only allowed when loading asset from server.
                    }
                    StartCoroutine(LoadAvatarAsync(fullPath));
                }
            }
        }
        /// <summary>
        /// Gets the streaming asset full path.
        /// </summary>
        /// <returns></returns>
        private string getStreamingAssetFullPath()
        {
            string path = string.Empty;
            if (Application.platform == RuntimePlatform.Android)
            {
                loadType = LoadType.Url; // you can only read file with load type url in Android platform Streaming Asset. WebGL platform will be added.
                path = "jar:file://" + Application.dataPath + "!/assets" + Url;
                Debug.Log("[profiling] Android Platform StreamingAssets Load file = " + path);
                //jar:file:///data/app/~~s_lJw1yShVLHNh89ZFDxkA==/com.samsung.avatarimporter-ICA4Lm2bzTruXNhsJ4BhrQ==/base.apk!/assets/Basemodel_female/model_external.gltf
            }
            else
            {
                path = Application.streamingAssetsPath + Url;
                path = (loadType == LoadType.Url) ? "file://" + path : path; // Url load type need prefix "file://"
                Debug.Log("[profiling] StreamingAssets Load file : " + path);
                //C:/cygwin64/home/sangrae.kim/unity/UnityAvatarAvatarLoader/Phase3/Unity/GLTFimporter/Assets/StreamingAssets/Basemodel_female/model_external.gltf
            }
            return path;
        }
        /// <summary>
        /// Gets the else asset location full path.
        /// </summary>
        /// <returns></returns>
        private string getElseAssetLocationFullPath()
        {
            string path = string.Empty;
            if (Application.platform == RuntimePlatform.Android)
            {
                path = (loadType == LoadType.Url) ? "file://" + Url : Url;
            }
            else
            {
                path = (loadType == LoadType.Url) ? Application.dataPath + Url : Url;
            }
            return path;
        }
        /// <summary>
        /// Loads the GLTF.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public IEnumerator loadGLTF(string filePath)
        {
            Debug.Log("[profiling] loadGLTF(For Deprecated) : filePath = " + filePath);
            Url = filePath;
            loadType = LoadType.Url;
            assetLocation = AssetLocation.Else;
            fullPath = getElseAssetLocationFullPath();
            yield return LoadAvatarAsync(fullPath);
        }
        /// <summary>
        /// Loads the GLTF with file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="isStreamingAsset">if set to <c>true</c> [is streaming asset].</param>
        /// <param name="loadTypeIndex">Index of the load type.</param>
        /// <returns></returns>
        public IEnumerator loadGLTFWithFilePath(string filePath, bool isStreamingAsset, int loadTypeIndex)
        {
            Debug.Log("[profiling] loadGLTFWithFilePath : filePath = " + filePath);
            Url = filePath;
            assetLocation = isStreamingAsset ? AssetLocation.StreamingAsset : AssetLocation.Else;
            loadType = (LoadType)loadTypeIndex;
            Debug.Log("[profiling] loadGLTFWithFilePath : loadType = " + loadType);
            fullPath = (assetLocation == AssetLocation.StreamingAsset) ? getStreamingAssetFullPath() : getElseAssetLocationFullPath();
            Debug.Log("[profiling] loadGLTFWithFilePath : fullPath = " + fullPath);
            yield return LoadAvatarAsync(fullPath);
        }
        /// <summary>
        /// Loads the GLTF from server URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public IEnumerator loadGLTFFromServerUrl(string url)
        {
            Debug.Log("[profiling] loadGLTFFromServerUrl : url = " + url);
            fullPath = url;
            loadType = LoadType.Url;
            yield return LoadAvatarAsync(fullPath);
        }


        /// <summary>
        /// Loads the ar emoji asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private IEnumerator LoadAvatarAsync(string url)
        {
            yield return new WaitUntil(() => loadstatus != LoadStatus.INPROGRESS);

            ChangeStatus(LoadStatus.INPROGRESS);
            InitComponent();
            InstantiateLoadNode(gameObject.transform);
            IntiGltfAsset();
            Task<bool> result = gltfAsset.Load(url);
            yield return new WaitUntil(() => result.IsCompleted);

            if (result.Result)
            {
                ChangeStatus(LoadStatus.GLTFLOADED);

                //Set Bone Constructor
                avatarBoneConstructor = new AvatarBoneConstructor(loadNode);

                //Set Mesh Composer
                avatarMeshComposer = new AvatarMeshComposer(loadNode, useMeshCombine, meshCombineOption);
                if (useMeshCombine && meshCombineOption.combineFlags.HasFlag(CombineFlags.RemoveTargetMeshes))
                    Destroy(gltfAsset);

                //Set Blendshape Driver
                if (!useMeshCombine || (useMeshCombine && !meshCombineOption.combineFlags.HasFlag(CombineFlags.RemoveBlendshapes)))
                {
                    avatarBlendshapeDriver = loadNode.AddComponent<AvatarBlendshapeDriver>();
                    avatarBlendshapeDriver.InitBlendshapeDriver();
                }

                if (_animator == null)
                    _animator = GetComponent<Animator>();

                if (!animatorType.Equals(AnimationType.None))
                {
                    _animator.enabled = true;
                    if (animatorType.Equals(AnimationType.Humanoid))
                        _animator.avatar = avatarBoneConstructor.GetConstructedAvatar();
                    _animator.Rebind();
                }
                else
                    _animator.enabled = false;

                loadNode.SetActive(true);
                ChangeStatus(LoadStatus.DONE);
            }
            else
            {
                Debug.Log("Failed to load Avatar!! ");
                InitComponent();
                ChangeStatus(LoadStatus.NOTSTARTED);
            }
        }
        /// <summary>
        /// Intis the GLTF asset.
        /// </summary>
        /// <returns>
        ///   <br />
        /// </returns>
        private GltfAsset IntiGltfAsset()
        {
            gltfAsset = loadNode.GetComponent<GltfAsset>();
            gltfAsset.StreamingAsset = (assetLocation == AssetLocation.StreamingAsset) ? true : false;
            return gltfAsset;
        }
        /// <summary>
        /// Initloads the node.
        /// </summary>
        private void InitComponent()
        {
            if (loadNode != null)
                Destroy(loadNode);
            loadNode = null;
            avatarMeshComposer = null;
            avatarBoneConstructor = null;
            if(avatarBlendshapeDriver!=null)
                Destroy(avatarBlendshapeDriver);
            avatarBlendshapeDriver = null;
            m_gltfTransforms = null;
        }
        /// <summary>
        /// Makes the sub node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void InstantiateLoadNode(Transform parent)
        {
            loadNode = Instantiate(Resources.Load(NodeDefines.LOAD_NODE, typeof(GameObject))) as GameObject;
            loadNode.name = NodeDefines.LOAD_NODE;
            loadNode.transform.parent = parent;
            loadNode.transform.localPosition = Vector3.zero;
            loadNode.transform.localEulerAngles = Vector3.zero;
            loadNode.transform.localScale = Vector3.one * 0.01f;
            loadNode.SetActive(false);
        }

        /// <summary>
        /// Changes the status.
        /// </summary>
        /// <param name="status">The status.</param>
        private void ChangeStatus(LoadStatus status)
        {
            loadstatus = status;
            if (status.Equals(LoadStatus.INPROGRESS))
                InProgressFeatureEvent?.Invoke();
            else if (status.Equals(LoadStatus.GLTFLOADED))
                GltfLoadedFeatureEvent?.Invoke();
            else if (status.Equals(LoadStatus.DONE))
                DoneFeatureEvent?.Invoke();
        }
        /// <summary>
        /// Set the ar emoji shadow.
        /// </summary>
        /// <param name="castingMode">The casting mode.</param>
        /// <param name="isReceiveShadow">if set to <c>true</c> [is receive shadow].</param>
        public void SetAvatarShadow(ShadowCastingMode castingMode, bool isReceiveShadow)
        {
            if (!(loadstatus == LoadStatus.DONE))
            {
                Debug.Log("AvatarShadow can be set after Avatar is loaded!!");
                return;
            }
            avatarMeshComposer.SetAvatarShadow(castingMode, isReceiveShadow);
        }
        /// <summary>
        /// Set the ar emoji shadow.
        /// </summary>
        /// <param name="combineType">Type of the combine.</param>
        /// <returns></returns>
        public SkinnedMeshRenderer GetCombinedMesh(MeshType combineType)
        {
            if (!(loadstatus == LoadStatus.DONE) || !useMeshCombine)
            {
                Debug.Log("There is no CombinedMesh or CombinedMesh can be obtained after Avatar is loaded!!");
                return null;
            }
            return avatarMeshComposer.GetCombinedMesh(combineType);
        }

        /// <summary>
        /// Gets the blendshape driver.
        /// </summary>
        /// <returns></returns>
        public AvatarBlendshapeDriver GetBlendshapeDriver()
        {
            if (!(loadstatus == LoadStatus.DONE) || avatarBlendshapeDriver == null)
            {
                Debug.Log("There is no BlendshapeDriver or BlendshapeDriver can be obtained after Avatar is loaded!!");
                return null;
            }
            return avatarBlendshapeDriver;
        }
        /// <summary>
        /// Gets the type of the body.
        /// </summary>
        /// <returns></returns>
        public BodyType GetBodyType()
        {
            if (!(loadstatus == LoadStatus.DONE))
            {
                Debug.Log("BodyType can be obtained after Avatar is loaded!!");
                return BodyType.Female;
            }
            return avatarBoneConstructor.bodyType;
        }

        /// <summary>
        /// Finds the name of the avatar transform by.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Transform FindAvatarTransformByName(string value)
        {
            if (m_gltfTransforms == null)
                m_gltfTransforms = loadNode.GetComponentsInChildren<Transform>(true).ToDictionary(x => x.name, x => x.transform);

            if (m_gltfTransforms.ContainsKey(value))
                return m_gltfTransforms[value];
            else
            {
                m_gltfTransforms = loadNode.GetComponentsInChildren<Transform>(true).ToDictionary( x =>{return x.name;}, x => x.transform);
                if (m_gltfTransforms.ContainsKey(value))
                    return m_gltfTransforms[value];
            }
            Debug.Log("The Avatar has not yet been loaded or there is no Transform for its name.!!");
            return null;
        }
    }
}
