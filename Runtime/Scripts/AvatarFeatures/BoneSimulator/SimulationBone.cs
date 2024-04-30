using System.Collections.Generic;
using UnityEngine;
using static AvatarPluginForUnity.AvatarBoneConstructor;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class SimulationBone : MonoBehaviour
    {

        /// <summary>
        /// The stiffness
        /// </summary>
        private float stiffness;
        /// <summary>
        /// The damping
        /// </summary>
        private float damping;
        /// <summary>
        /// The elesticity
        /// </summary>
        private float elesticity;
        /// <summary>
        /// The shakiness
        /// </summary>
        private float shakiness = 0.0f;
        /// <summary>
        /// The mass
        /// </summary>
        private float mass;
        /// <summary>
        /// The gravity
        /// </summary>
        private Vector3 gravity;
        /// <summary>
        /// The minimum toleranced dist
        /// </summary>
        private float minTolerancedDist;
        /// <summary>
        /// The maximum toleranced dist
        /// </summary>
        private float maxTolerancedDist;
        /// <summary>
        /// The org local position
        /// </summary>
        private Vector3 orgLocalPosition;
        /// <summary>
        /// The org local euler angles
        /// </summary>
        private Vector3 orgLocalEulerAngles;
        /// <summary>
        /// The org distance
        /// </summary>
        public float orgDistance;
        /// <summary>
        /// The dist
        /// </summary>
        public float dist;
        /// <summary>
        /// The is knematic
        /// </summary>
        public bool isKnematic;
        /// <summary>
        /// The velocity
        /// </summary>
        private Vector3 velocity = Vector3.zero;
        /// <summary>
        /// The spring force
        /// </summary>
        private Vector3 springForce = Vector3.zero;
        /// <summary>
        /// The damping force
        /// </summary>
        private Vector3 dampingForce = Vector3.zero;
        /// <summary>
        /// The position
        /// </summary>
        public Vector3 position = Vector3.zero;
        // Name, Center, Radius, Height, Dir 
        /// <summary>
        /// The avatar collider template
        /// </summary>
        public static Dictionary<BodyType, List<(string, Vector3, float, float, int)>> AvatarColliderTemplate = new Dictionary<BodyType, List<(string, Vector3, float, float, int)>>
        {
            [BodyType.Female] = new List<(string, Vector3, float, float, int)> {
                ("spine1_JNT", new Vector3(0f, 0.02f, 0.016f), 0.12f, 0.3f, 1),
                ("spine2_JNT", new Vector3(0f, 0.05f, 0.018f), 0.14f, 0.3f, 1),
                ("neck_JNT", new Vector3(0f, 0.056f, 0f), 0.06f, 0.19f, 1),
                ("l_arm_JNT", new Vector3(-0.11f, 0f, 0f), 0.05f, 0.32f, 0),
                ("l_forearm_JNT", new Vector3(-0.1f, 0f, 0f), 0.042f, 0.24f, 0),
                ("r_arm_JNT", new Vector3(0.11f, 0f, 0f), 0.05f, 0.32f, 0),
                ("r_forearm_JNT", new Vector3(0.1f, 0f, 0f), 0.042f, 0.24f, 0)
            },
            [BodyType.Male] = new List<(string, Vector3, float, float, int)> {
                ("spine1_JNT", new Vector3(0f, 0.02f, 0.016f), 0.12f, 0.3f, 1),
                ("spine2_JNT", new Vector3(0f, 0.05f, 0.01f), 0.15f, 0.3f, 1),
                ("neck_JNT", new Vector3(0f, 0.056f, 0.01f), 0.06f, 0.19f, 1),
                ("l_arm_JNT", new Vector3(-0.11f, 0f, 0f), 0.055f, 0.32f, 0),
                ("l_forearm_JNT", new Vector3(-0.1f, 0f, 0f), 0.042f, 0.24f, 0),
                ("r_arm_JNT", new Vector3(0.11f, 0f, 0f), 0.055f, 0.32f, 0),
                ("r_forearm_JNT", new Vector3(0.1f, 0f, 0f), 0.042f, 0.24f, 0)
            },
            [BodyType.Junior] = new List<(string, Vector3, float, float, int)> {
                ("spine1_JNT", new Vector3(0f, 0.0f, 0.016f), 0.1f, 0.1f, 1),
                ("spine2_JNT", new Vector3(0f, 0.015f, 0.01f), 0.09f, 0.1f, 1),
                ("neck_JNT", new Vector3(0f, 0.03f, 0.007f), 0.04f, 0.09f, 1),
                ("l_arm_JNT", new Vector3(-0.045f, 0f, 0f), 0.025f, 0.18f, 0),
                ("l_forearm_JNT", new Vector3(-0.061f, 0f, 0f), 0.022f, 0.15f, 0),
                ("r_arm_JNT", new Vector3(0.045f, 0f, 0f), 0.025f, 0.18f, 0),
                ("r_forearm_JNT", new Vector3(0.061f, 0f, 0f), 0.022f, 0.15f, 0)
            }
        };
        /// <summary>
        /// The use collider
        /// </summary>
        private bool useCollider = false;
        /// <summary>
        /// The collider targets
        /// </summary>
        private HashSet<Collider> colliderTargets = new HashSet<Collider>();
        /// <summary>
        /// The collider radius
        /// </summary>
        private float colliderRadius;
        /// <summary>
        /// Initializes the bone.
        /// </summary>
        /// <param name="isKnematic">if set to <c>true</c> [is knematic].</param>
        /// <param name="colliderTargets">The collider targets.</param>
        public void InitBone(bool isKnematic, HashSet<Collider> colliderTargets = null)
        {
            this.position = this.transform.position;

            Quaternion lookAt = Quaternion.identity;    
            lookAt.SetLookRotation((-transform.localPosition).normalized);
            this.orgLocalEulerAngles = lookAt.eulerAngles - transform.localEulerAngles;
            this.orgLocalPosition = this.transform.localPosition;
            this.orgDistance = Vector3.Distance(Vector3.zero, transform.localPosition);
            this.isKnematic = isKnematic;

            if (colliderTargets != null)
            {
                this.useCollider = true;
                this.colliderTargets = colliderTargets;
            }
        }

        /// <summary>
        /// Sets the parms.
        /// </summary>
        /// <param name="stiffness">The stiffness.</param>
        /// <param name="damping">The damping.</param>
        /// <param name="elesticity">The elesticity.</param>
        /// <param name="mass">The mass.</param>
        /// <param name="gravity">The gravity.</param>
        /// <param name="colliderRadius">The collider radius.</param>
        public void SetParms(float stiffness, float damping, float elesticity, float mass, Vector3 gravity, float colliderRadius)
        {
            this.stiffness = stiffness;
            this.damping = damping;
            this.maxTolerancedDist = this.orgDistance + this.orgDistance * elesticity;
            this.minTolerancedDist = this.orgDistance - this.orgDistance * elesticity;
            this.mass = mass;
            this.gravity = gravity;
            this.colliderRadius = colliderRadius;
        }

        /// <summary>
        /// Sets the shakiness.
        /// </summary>
        /// <param name="shakiness">The shakiness.</param>
        public void SetShakiness(float shakiness)
        {
            this.shakiness = shakiness;
        }

        /// <summary>
        /// Applies the specified time step.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void Apply(float timeStep = 0.0f)
        {
            springForce = -stiffness * (position - transform.parent.TransformPoint(orgLocalPosition));
            dampingForce = damping * velocity;

            var force = (springForce - dampingForce) + (mass * gravity);
            var acceleration = force / mass;
            velocity = velocity + acceleration * timeStep;
            position += (velocity * timeStep);

            if (useCollider)
            {
                Vector3 correctionPoint = Vector3.zero;
                int collisionedCount = 0;
                foreach (var collider in colliderTargets)
                {
                    Vector3 closestPoint = collider.ClosestPoint(position);
                    if (closestPoint.x == position.x && closestPoint.y == position.y && closestPoint.z == position.z)
                    {
                        collisionedCount++;
                        closestPoint = transform.parent.TransformPoint(orgLocalPosition);
                        correctionPoint += closestPoint;
                    }
                    else if (Vector3.Distance(position, closestPoint) < colliderRadius)
                    {
                        collisionedCount++;
                        Vector3 normal = (position - closestPoint).normalized;
                        closestPoint += (normal * colliderRadius);
                        correctionPoint += closestPoint;
                    }
                }
                if (collisionedCount != 0)
                    position = correctionPoint / collisionedCount;
            }

            Vector3 localPos = transform.parent.InverseTransformPoint(position);
            dist = Vector3.Distance(Vector3.zero, localPos);

            if (!IsToleranced(dist))
            {
                float t = dist < orgDistance ? minTolerancedDist / dist : maxTolerancedDist / dist;
                position = transform.parent.TransformPoint(Vector3.LerpUnclamped(Vector3.zero, localPos, t));
            }

            if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
                return;

            transform.position = position;

            if (shakiness!=0.0f) {
                Quaternion lerpQut = Quaternion.FromToRotation(Vector3.forward, -transform.localPosition.normalized);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, lerpQut, shakiness);
            }
        }

        /// <summary>
        /// Determines whether the specified dist is toleranced.
        /// </summary>
        /// <param name="dist">The dist.</param>
        /// <returns>
        ///   <c>true</c> if the specified dist is toleranced; otherwise, <c>false</c>.
        /// </returns>
        private bool IsToleranced(float dist)
        {
            if (minTolerancedDist <= dist && dist <= maxTolerancedDist)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes the transform.
        /// </summary>
        /// <param name="includePhysicsParms">if set to <c>true</c> [include physics parms].</param>
        public void InitTransform(bool includePhysicsParms)
        {
            if (includePhysicsParms)
            {
                velocity = Vector3.zero;
                springForce = Vector3.zero;
                dampingForce = Vector3.zero;
            }

            transform.localPosition = orgLocalPosition;
            position = transform.parent.TransformPoint(orgLocalPosition);
            dist = 0.0f;

        }
    }
}