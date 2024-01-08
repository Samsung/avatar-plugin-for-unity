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
        public enum LoadStatus
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
            /// The done
            /// </summary>
            DONE,
        }
        /// <summary>
        /// The load on start up
        /// </summary>
        [SerializeField] private bool loadOnStartUp;
        /// <summary>
        /// The asset location
        /// </summary>
        [SerializeField] public AssetLocation assetLocation;
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
        /// The ues mesh combine
        /// </summary>
        [SerializeField] public bool useMeshCombine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The mesh combine option
        /// </summary>
        [DrawIf("useMeshCombine", true, DrawIfAttribute.DisablingType.Draw)]
#endif
        [SerializeField] public AvatarCombineOption meshCombineOption;
        /// <summary>
        /// The animation type
        /// </summary>
        [SerializeField] private AnimationType animatorType;
        /// <summary>
        /// Gets the type of the body.
        /// </summary>
        /// <value>
        /// The type of the body.
        /// </value>
        public BodyType BodyType => _bodyType;
        /// <summary>
        /// The body type
        /// </summary>
        private BodyType _bodyType = BodyType.Female;
        /// <summary>
        /// Gets the load node.
        /// </summary>
        /// <value>
        /// The load node.
        /// </value>
        public GameObject loadNode => _loadNode;
        /// <summary>
        /// The load node
        /// </summary>
        private GameObject _loadNode = null;
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
        /// Occurs when [on status changed callback].
        /// </summary>
        private event Action<LoadStatus> _onStatusChangedCallback;

        /// <summary>
        /// Occurs when [on status changed callback].
        /// </summary>
        public event Action<LoadStatus> OnStatusChangedCallback
        {
            add
            {
                _onStatusChangedCallback += value;
            }
            remove
            {
                _onStatusChangedCallback -= value;

            }
        }
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
                //Set Bone Constructor
                avatarBoneConstructor = new AvatarBoneConstructor(_loadNode);

                //Set Mesh Composer
                avatarMeshComposer = new AvatarMeshComposer(_loadNode, useMeshCombine, meshCombineOption);
                if (useMeshCombine && meshCombineOption.combineFlags.HasFlag(AvatarCombineOption.AvatarCombineFlags.RemoveTargetMeshes))
                    Destroy(gltfAsset);

                //Set Blendshape Driver
                if (!useMeshCombine || (useMeshCombine && !meshCombineOption.combineFlags.HasFlag(AvatarCombineOption.AvatarCombineFlags.RemoveBlendshapes)))
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

                _loadNode.SetActive(true);
                ChangeStatus(LoadStatus.DONE);
            }
            else
            {
                Debug.LogError("Failed to load Avatar!! ");
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
            gltfAsset = _loadNode.GetComponent<GltfAsset>();
            gltfAsset.StreamingAsset = (assetLocation == AssetLocation.StreamingAsset) ? true : false;
            return gltfAsset;
        }
        /// <summary>
        /// Initloads the node.
        /// </summary>
        private void InitComponent()
        {
            if (_loadNode != null)
                Destroy(_loadNode);
            _loadNode = null;
            avatarMeshComposer = null;
            avatarBoneConstructor = null;
            if(avatarBlendshapeDriver!=null)
                Destroy(avatarBlendshapeDriver);
            avatarBlendshapeDriver = null;
        }
        /// <summary>
        /// Makes the sub node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        private void InstantiateLoadNode(Transform parent)
        {
            _loadNode = Instantiate(Resources.Load(NodeDefines.LOAD_NODE, typeof(GameObject))) as GameObject;
            _loadNode.name = NodeDefines.LOAD_NODE;
            _loadNode.transform.parent = parent;
            _loadNode.transform.localPosition = Vector3.zero;
            _loadNode.transform.localEulerAngles = Vector3.zero;
            _loadNode.transform.localScale = Vector3.one;
            _loadNode.SetActive(false);
        }

        /// <summary>
        /// Changes the status.
        /// </summary>
        /// <param name="status">The status.</param>
        private void ChangeStatus(LoadStatus status)
        {
            loadstatus = status;
            _onStatusChangedCallback?.Invoke(status);
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
        public SkinnedMeshRenderer GetCombinedMesh(CombinedMeshType combineType)
        {
            if (!(loadstatus == LoadStatus.DONE) || !useMeshCombine)
            {
                Debug.Log("There is no CombinedMesh or CombinedMesh can be obtained after Avatar is loaded!!");
                return null;
            }
            return avatarMeshComposer.GetCombinedMesh(combineType);
        }

        /// <summary>
        /// Gets the type of the body.
        /// </summary>
        /// <returns></returns>
        public BodyType? GetBodyType()
        {
            if (!(loadstatus == LoadStatus.DONE))
            {
                Debug.Log("BodyType can be obtained after Avatar is loaded!!");
                return null;
            }
            return avatarBoneConstructor.bodyType;
        }
    }
}
