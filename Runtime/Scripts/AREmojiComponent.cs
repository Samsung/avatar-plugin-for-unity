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
using static AvatarPluginForUnity.Constance;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    using AvatarPluginForUnity.Editor;
#endif
namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(Animator))]
    public class AREmojiComponent : MonoBehaviour
    {

        /// <summary>
        /// The load on start up
        /// </summary>
        [SerializeField] private bool loadOnStartUp;
        /// <summary>
        /// The asset location
        /// </summary>
        [SerializeField] private AssetLocation assetLocation;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        [DrawIf("assetLocation", AssetLocation.Server, DrawIfAttribute.DisablingType.DrawExclude)]
#endif
        /// <summary>
        /// The load type
        /// </summary>
        [SerializeField] private LoadType loadType;
        /// <summary>
        /// The URL
        /// </summary>
        [SerializeField] private string Url;
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
        public BodyType bodyType => _bodyType;
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
        /// The aremoji bone constructor
        /// </summary>
        private AREmojiBoneConstructor aremojiBoneConstructor = null;
        /// <summary>
        /// The aremoji mesh composer
        /// </summary>
        private AREmojiMeshComposer aremojiMeshComposer = null;
        /// <summary>
        /// The aremoji blendshape driver
        /// </summary>
        private AREmojiBlendshapeDriver aremojiBlendshapeDriver = null;
        /// <summary>
        /// The is loading
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool isLoading => _isLoading;
        /// <summary>
        /// The is loading
        /// </summary>
        private bool _isLoading = false;
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
                    case AssetLocation.StreamingAsset: fullPath = getStreamingAssetFullPath(); break;
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
                    StartCoroutine(LoadAREmojiAsync(fullPath));
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
                //jar:file:///data/app/~~s_lJw1yShVLHNh89ZFDxkA==/com.samsung.aremojiimporter-ICA4Lm2bzTruXNhsJ4BhrQ==/base.apk!/assets/Basemodel_female/model_external.gltf
            }
            else
            {
                path = Application.streamingAssetsPath + Url;
                path = (loadType == LoadType.Url) ? "file://" + path : path; // Url load type need prefix "file://"
                Debug.Log("[profiling] StreamingAssets Load file : " + path);
                //C:/cygwin64/home/sangrae.kim/unity/UnityAREmojiAvatarLoader/Phase3/Unity/GLTFimporter/Assets/StreamingAssets/Basemodel_female/model_external.gltf
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
            yield return LoadAREmojiAsync(fullPath);
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
            yield return LoadAREmojiAsync(fullPath);
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
            yield return LoadAREmojiAsync(fullPath);
        }
        /// <summary>
        /// Loads the ar emoji asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private IEnumerator LoadAREmojiAsync(string url)
        {
            yield return new WaitUntil(() => !_isLoading);
            _isLoading = true;
            InitLoadNode();
            InstantiateLoadNode(gameObject.transform);
            IntiGltfAsset();
            Task<bool> result = gltfAsset.Load(url);
            yield return new WaitUntil(() => result.IsCompleted);

            if (result.Result)
            {
                //Set Bone Constructor
                aremojiBoneConstructor = new AREmojiBoneConstructor(gameObject);

                //Set Mesh Composer
                aremojiMeshComposer = new AREmojiMeshComposer(_loadNode);

                //Set Blendshape Driver
                aremojiBlendshapeDriver = loadNode.AddComponent<AREmojiBlendshapeDriver>();
                aremojiBlendshapeDriver.InitBlendshapeDriver();

                if (_animator == null)
                    _animator = GetComponent<Animator>();

                if (_animator.enabled && !animatorType.Equals(AnimationType.None))
                {
                    if (animatorType.Equals(AnimationType.Humanoid))
                        _animator.avatar = aremojiBoneConstructor.GetConstructedAvatar();
                    _animator.Rebind();
                }
                _loadNode.SetActive(true);
            }
            else
            {
                Debug.LogError("Failed to load AREmoji!! ");
                InitLoadNode();
            }
            _isLoading = false;
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
        private void InitLoadNode()
        {
            if (_loadNode != null)
            {
                DestroyImmediate(_loadNode);
                _loadNode = null;
                aremojiMeshComposer = null;
                aremojiBoneConstructor = null;
                aremojiBlendshapeDriver = null;
            }
        }
        /// <summary>
        /// Makes the sub node.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private void InstantiateLoadNode(Transform parent)
        {
            _loadNode = Instantiate(Resources.Load("LoadNode", typeof(GameObject))) as GameObject;
            _loadNode.name = "LoadNode";
            _loadNode.transform.parent = parent;
            _loadNode.transform.localPosition = Vector3.zero;
            _loadNode.transform.localEulerAngles = Vector3.zero;
            _loadNode.transform.localScale = Vector3.one;
            _loadNode.SetActive(false);
        }
        /// <summary>
        /// Set the ar emoji shadow.
        /// </summary>
        /// <param name="castingMode">The casting mode.</param>
        /// <param name="isReceiveShadow">if set to <c>true</c> [is receive shadow].</param>
        public void SetAREmojiShadow(ShadowCastingMode castingMode, bool isReceiveShadow)
        {
            if (isLoading || _loadNode == null || aremojiMeshComposer == null)
            {
                Debug.Log("AREmojiShadow can be set after AREmoji is loaded!!");
                return;
            }
            aremojiMeshComposer.SetAREmojiShadow(castingMode, isReceiveShadow);
        }

        /// <summary>
        /// Sets the face animation clip.
        /// </summary>
        public void SetFaceAnimationClip()
        {
            //FaceAniLoader mFaceAni = new FaceAniLoader(loadNode, aremojiMeshComposer.blendShapeMeshSet);
            //AnimationClip mFaceAnimation = mFaceAni.CreateAnimationClip(Application.streamingAssetsPath + "/FaceAniFiles///F001_31_Hi.json");
            //Animation faceAnimation = loadNode.AddComponent<Animation>();
            //faceAnimation.AddClip(mFaceAnimation, "BlendAnimation");
            //faceAnimation.clip = mFaceAnimation;
            //faceAnimation.Play();
            //var clipName = "Assets/FaceIdle.anim";
            //AssetDatabase.CreateAsset(mFaceAnimation, clipName);
            //AssetDatabase.SaveAssets();
        }

    }
}
