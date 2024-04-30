using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using AvatarPluginForUnity.Editor;
#endif
using static AvatarPluginForUnity.AvatarBoneConstructor;
using static AvatarPluginForUnity.AvatarComponent;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class ApplyDynamicHair : MonoBehaviour
    {
        /// <summary>
        /// The target FPS
        /// </summary>
        private static int TARGET_FPS = 60;
        /// <summary>
        /// The earring size limit
        /// </summary>
        private static float EARRING_SIZE_LIMIT = 0.05f;
        /// <summary>
        /// The limit SPF
        /// </summary>
        private float limitSPF = 1.0f / ((float)TARGET_FPS);
        /// <summary>
        /// The avatar component
        /// </summary>
        private AvatarComponent avatarComponent = null;
        /// <summary>
        /// The collider
        /// </summary>
        [SerializeField]
        private bool Collider = true;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// The collider radius
        /// </summary>
        [DrawIf("Collider", true, DrawIfAttribute.DisablingType.Draw)]
#endif
        [SerializeField]
        [Range(0.001f, 1.0f)]
        public float ColliderRadius = 0.014f;
        /// <summary>
        /// The stiffness
        /// </summary>
        [SerializeField]
        [Range(0.0f, 1000.0f)]
        public float Stiffness = 350;
        /// <summary>
        /// The damping
        /// </summary>
        [SerializeField]
        [Range(0.0f, 100.0f)]
        public float Damping = 15.0f;
        /// <summary>
        /// The elesticity
        /// </summary>
        [SerializeField]
        [Range(0.0f, 1.0f)]
        public float Elesticity = 0.05f;
        /// <summary>
        /// The mass
        /// </summary>
        [SerializeField]
        [Range(0.05f, 10.0f)]
        public float Mass = 0.7f;
        /// <summary>
        /// The gravity
        /// </summary>
        [SerializeField]
        public Vector3 Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        /// <summary>
        /// The is update
        /// </summary>
        private bool isUpdate = false;
        /// <summary>
        /// The hair simulation groups
        /// </summary>
        private List<SimulationBoneGroup> hairSimulationGroups = new List<SimulationBoneGroup>();
        /// <summary>
        /// The head center JNT
        /// </summary>
        private Transform headCenter_JNT;
        /// <summary>
        /// The use accessory simulation
        /// </summary>
        private bool useAccessorySimulation = false;
        /// <summary>
        /// The accessory simulation groups
        /// </summary>
        private List<SimulationBoneGroup> accessory_SimulationGroups = new List<SimulationBoneGroup>();
        /// <summary>
        /// The r earring accessory JNT
        /// </summary>
        private Transform r_earring_accessory_JNT = null;
        /// <summary>
        /// The l earring accessory JNT
        /// </summary>
        private Transform l_earring_accessory_JNT = null;
        /// <summary>
        /// The r earring child
        /// </summary>
        private Transform r_earring_Target = null;
        /// <summary>
        /// The l earring child
        /// </summary>
        private Transform l_earring_Target = null;
        /// <summary>
        /// The collider targets
        /// </summary>
        private List<Collider> colliderTargets = new List<Collider>();
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start()
        {
            avatarComponent = GetComponent<AvatarComponent>();
        }
        /// <summary>
        /// Called when [validate].
        /// </summary>
        void OnValidate()
        {
            UpdateSimulationParms();
        }


        /// <summary>
        /// Updates the simulation parms.
        /// </summary>
        public void UpdateSimulationParms()
        {
            foreach (var hairSimulationGroup in hairSimulationGroups)
                hairSimulationGroup.InitBones(Stiffness, Damping, Elesticity, Mass, Gravity, ColliderRadius * headCenter_JNT.lossyScale.x);

            foreach (var accessory_SimulationGroup in accessory_SimulationGroups)
                accessory_SimulationGroup.InitBones(Stiffness, Damping, 0.0f, Mass, Gravity, ColliderRadius * headCenter_JNT.lossyScale.x);
        }


        /// <summary>
        /// Initializes the simulator.
        /// </summary>
        public void InitSimulator()
        {
            l_earring_Target = null;
            r_earring_Target = null;
            hairSimulationGroups.Clear();
            accessory_SimulationGroups.Clear();
            colliderTargets.Clear();
            if (Collider)
                MakeAvatarHeadCollider();

            headCenter_JNT = avatarComponent.FindAvatarTransformByName("headCenter_JNT");

            Transform earring_GEO = avatarComponent.FindAvatarTransformByName("earring_GEO");
            if (earring_GEO &&
                EARRING_SIZE_LIMIT * headCenter_JNT.lossyScale.x < earring_GEO.GetComponent<SkinnedMeshRenderer>().bounds.size.y)
                useAccessorySimulation = true;
        }

        /// <summary>
        /// Applies the simulator.
        /// </summary>
        public void ApplySimulator()
        {
            if (Collider)
                MakeAvatarBodyColliders(avatarComponent.GetBodyType());

            int childCount = headCenter_JNT.childCount;
            List<Transform> roots = new List<Transform>();
            for (int i = 0; i < childCount; i++)
                roots.Add(headCenter_JNT.GetChild(i));

            for (int i = 0; i < childCount; i++)
            {
                Transform[] allChild = roots[i].GetComponentsInChildren<Transform>();
                int childrenCount = allChild.Length;
                Transform parent = headCenter_JNT;

                List<SimulationBone> hairBones = new List<SimulationBone>();

                for (int j = 0; j < childrenCount; j++)
                {

                    Transform child = allChild[j];

                    Transform tunnedNode = MakeLocalRatationTunnedNode(child);
                    child.parent = tunnedNode;
                    child.localPosition = Vector3.zero;
                    if (child.childCount != 0)
                        child.GetChild(0).parent = tunnedNode;

                    bool isRoot = parent == headCenter_JNT;
                    SimulationBone simulationBone = tunnedNode.gameObject.AddComponent<SimulationBone>();
                    simulationBone.SetShakiness(0.03f);
                    if (Collider && !isRoot)
                        simulationBone.InitBone(isRoot, new HashSet<Collider>(colliderTargets));
                    else
                        simulationBone.InitBone(isRoot);

                    hairBones.Add(simulationBone);
                    parent = tunnedNode;
                }

                if (hairBones.Count != 0)
                {
                    SimulationBoneGroup hairSimulationGroup = new SimulationBoneGroup(hairBones, Stiffness, Damping, Elesticity, Mass, Gravity, ColliderRadius * headCenter_JNT.lossyScale.x);
                    hairSimulationGroups.Add(hairSimulationGroup);
                }
            }

            if (useAccessorySimulation)
            {
                r_earring_accessory_JNT = avatarComponent.FindAvatarTransformByName("r_earring_accessory_JNT");
                (r_earring_accessory_JNT, r_earring_Target) = MakeAccessoryBone(r_earring_accessory_JNT);

                l_earring_accessory_JNT = avatarComponent.FindAvatarTransformByName("l_earring_accessory_JNT");
                (l_earring_accessory_JNT, l_earring_Target) = MakeAccessoryBone(l_earring_accessory_JNT);
            }
            if (hairSimulationGroups.Count != 0 || accessory_SimulationGroups.Count != 0)
                isUpdate = true;
            else
                isUpdate = false;
        }



        /// <summary>
        /// Makes the accessory bone.
        /// </summary>
        /// <param name="earring_accessory_JNT">The earring accessory JNT.</param>
        /// <returns></returns>
        private (Transform, Transform) MakeAccessoryBone(Transform earring_accessory_JNT)
        {
            if (earring_accessory_JNT.childCount == 0)
                return (null, null);

            List<SimulationBone> accessoryBones = new List<SimulationBone>();
            Transform earring_Parent = new GameObject(earring_accessory_JNT.name + "_Parent").transform;
            earring_Parent.parent = earring_accessory_JNT.parent;
            earring_Parent.localScale = earring_accessory_JNT.localScale;
            earring_Parent.localPosition = earring_accessory_JNT.localPosition;

            SimulationBone earring_ParentBone = earring_Parent.gameObject.AddComponent<SimulationBone>();
            earring_ParentBone.InitBone(true);
            accessoryBones.Add(earring_ParentBone);

            Transform earring_Child = earring_accessory_JNT.GetChild(0);
            earring_Child.parent = earring_Parent;
            SimulationBone earring_ChildBone = earring_Child.gameObject.AddComponent<SimulationBone>();
            if (Collider)
                earring_ChildBone.InitBone(false, new HashSet<Collider>(colliderTargets));
            else
                earring_ChildBone.InitBone(false);
            accessoryBones.Add(earring_ChildBone);

            SimulationBoneGroup accessory_SimulationGroup = new SimulationBoneGroup(accessoryBones, Stiffness, Damping, 0.0f, Mass, Gravity, ColliderRadius * headCenter_JNT.lossyScale.x);
            accessory_SimulationGroups.Add(accessory_SimulationGroup);

            Transform tunnedNode = MakeLocalRatationTunnedNode(earring_accessory_JNT, earring_Child);
            earring_accessory_JNT.parent = tunnedNode;

            return (tunnedNode, earring_Child);
        }

        /// <summary>
        /// Makes the local ratation tunned node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        private Transform MakeLocalRatationTunnedNode(Transform node, Transform target = null)
        {
            Transform tunnedNode = new GameObject(node.name + "_tunned").transform;
            tunnedNode.parent = node.parent;
            tunnedNode.localScale = node.localScale;
            tunnedNode.localPosition = node.localPosition;
            Vector3 localPos = target != null ? tunnedNode.parent.InverseTransformPoint(target.position) : Vector3.zero;
            tunnedNode.localRotation = Quaternion.FromToRotation(Vector3.forward, (localPos - tunnedNode.localPosition).normalized);
            return tunnedNode;
        }

        /// <summary>
        /// Makes the avatar body colliders.
        /// </summary>
        /// <param name="bodyType">Type of the body.</param>
        private void MakeAvatarBodyColliders(BodyType bodyType)
        {
            foreach (var avatarColliderTemplate in SimulationBone.AvatarColliderTemplate[bodyType])
            {
                Transform target_JNT = avatarComponent.FindAvatarTransformByName(avatarColliderTemplate.Item1);
                CapsuleCollider capsuleCollider = target_JNT.gameObject.AddComponent<CapsuleCollider>();
                capsuleCollider.center = avatarColliderTemplate.Item2;
                capsuleCollider.radius = avatarColliderTemplate.Item3;
                capsuleCollider.height = avatarColliderTemplate.Item4;
                capsuleCollider.direction = avatarColliderTemplate.Item5;
                capsuleCollider.isTrigger = false;
                colliderTargets.Add(capsuleCollider);
            }
        }
        /// <summary>
        /// Makes the avatar head collider.
        /// </summary>
        private void MakeAvatarHeadCollider()
        {
            Transform head_COL = new GameObject("head_COL").transform;
            Transform head_GEO = avatarComponent.FindAvatarTransformByName("head_GEO");
            Mesh headMesh = new Mesh();
            head_GEO.GetComponent<SkinnedMeshRenderer>().BakeMesh(headMesh);
            head_COL.parent = head_GEO;
            head_COL.localPosition = Vector3.zero;
            head_COL.localEulerAngles = Vector3.zero;
            MeshCollider headMeshCollider = head_COL.gameObject.AddComponent<MeshCollider>();
            headMeshCollider.sharedMesh = headMesh;
            headMeshCollider.convex = true;
            headMeshCollider.isTrigger = false;
            head_COL.parent = avatarComponent.FindAvatarTransformByName("head_JNT");
            colliderTargets.Add(headMeshCollider);
        }
        /// <summary>
        /// Lates the update.
        /// </summary>
        private void LateUpdate()
        {
            if (!isUpdate || headCenter_JNT==null) return;
            float time = Time.deltaTime;
            int repeat = (int)Mathf.Ceil(time / limitSPF);
            float timeUnit = time / repeat;

            for(int i= 0; i< repeat; i++)
            {
                foreach (var hairSimulationGroup in hairSimulationGroups)
                {
                    hairSimulationGroup.Update(timeUnit);
                }
                foreach (var accessory_SimulationGroup in accessory_SimulationGroups)
                {
                    accessory_SimulationGroup.Update(timeUnit);
                }
            }

            if (r_earring_Target != null)
            {
                Vector3 localPos = r_earring_accessory_JNT.parent.InverseTransformPoint(r_earring_Target.position);
                Quaternion localRot = Quaternion.FromToRotation(Vector3.forward, localPos - r_earring_accessory_JNT.localPosition);
                r_earring_accessory_JNT.localRotation = Quaternion.Lerp(r_earring_accessory_JNT.localRotation, localRot, time * 10.0f);

            }
            if (l_earring_Target != null)
            {
                Vector3 localPos = l_earring_accessory_JNT.parent.InverseTransformPoint(l_earring_Target.position);
                Quaternion localRot = Quaternion.FromToRotation(Vector3.forward, localPos - l_earring_accessory_JNT.localPosition);
                l_earring_accessory_JNT.localRotation = Quaternion.Lerp(l_earring_accessory_JNT.localRotation, localRot, time * 10.0f);
            }

        }



        /// <summary>
        /// Testtts this instance.
        /// </summary>
        public void  testtt()
        {

            avatarComponent.loadGLTFWithFilePath("/Basemodel_female/model.gltf", true, 0);

        }

    }

}