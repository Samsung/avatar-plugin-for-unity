using System.Collections.Generic;
using UnityEngine;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    public class SimulationBoneGroup
    {
        /// <summary>
        /// The bone list
        /// </summary>
        public List<SimulationBone> boneList = new List<SimulationBone>();
        /// <summary>
        /// The bone group dist
        /// </summary>
        public float boneGroupDist;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationBoneGroup"/> class.
        /// </summary>
        /// <param name="boneList">The bone list.</param>
        /// <param name="stiffness">The stiffness.</param>
        /// <param name="damping">The damping.</param>
        /// <param name="elesticity">The elesticity.</param>
        /// <param name="mass">The mass.</param>
        /// <param name="gravity">The gravity.</param>
        /// <param name="colliderRadius">The collider radius.</param>
        public SimulationBoneGroup(List<SimulationBone> boneList, float stiffness, float damping, float elesticity, float mass, Vector3 gravity, float colliderRadius)
        {
            this.boneList = boneList;
            InitBones(stiffness, damping, elesticity, mass, gravity, colliderRadius);
        }

        /// <summary>
        /// Initializes the bones.
        /// </summary>
        /// <param name="stiffness">The stiffness.</param>
        /// <param name="damping">The damping.</param>
        /// <param name="elesticity">The elesticity.</param>
        /// <param name="mass">The mass.</param>
        /// <param name="gravity">The gravity.</param>
        /// <param name="colliderRadius">The collider radius.</param>
        public void InitBones(float stiffness, float damping, float elesticity, float mass, Vector3 gravity, float colliderRadius)
        {
            SimulationBone[] boneArray = boneList.ToArray();
            for (int i = 0; i < boneList.Count; i++)
            {
                if (boneArray[i].isKnematic)
                    continue;
                boneGroupDist += boneArray[i].orgDistance;
                boneArray[i].SetParms(stiffness, damping, elesticity, mass, gravity, colliderRadius);
            }
        }

        /// <summary>
        /// Initializes the simulation transform.
        /// </summary>
        /// <param name="includePhysicsParms">if set to <c>true</c> [include physics parms].</param>
        public void InitSimulationTransform(bool includePhysicsParms)
        {
            foreach (var bone in boneList)
            {
                if (bone.isKnematic)
                    continue;
                bone.InitTransform(includePhysicsParms);
            }
        }

        /// <summary>
        /// Updates the specified time step.
        /// </summary>
        /// <param name="timeStep">The time step.</param>
        public void Update(float timeStep)
        {
            float dist = 0.0f;


            foreach (var bone in boneList)
            {
                if (bone.isKnematic)
                    continue;
                 bone.Apply(timeStep);
                dist += bone.dist;
            }
            if (boneGroupDist * 2.0f < dist)
            {
                InitSimulationTransform(true);
            }
        }

    }
}