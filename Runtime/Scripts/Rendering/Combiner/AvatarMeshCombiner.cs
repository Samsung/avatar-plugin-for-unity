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
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Object;
using static AvatarPluginForUnity.AvatarMaterialCombiner;
using static AvatarPluginForUnity.MaterialCombineOption;
using UnityEngine.XR;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AvatarMeshCombiner : IDisposable
    {

        /// <summary>
        /// The texture loss rate
        /// </summary>
        private static float TEXTURE_LOSS_RATE = 0.25f;
        /// <summary>
        /// The bounds
        /// </summary>
        private Bounds bounds;
        /// <summary>
        /// The bindpose list
        /// </summary>
        private List<Matrix4x4> allBindposeList = new List<Matrix4x4>();
        /// <summary>
        /// The bone list
        /// </summary>
        private List<Transform> allBoneList = new List<Transform>();
        /// <summary>
        /// The blend frames dic
        /// </summary>
        Dictionary<(string, Mesh), BlendShapeData> blendFramesDic = new Dictionary<(string, Mesh), BlendShapeData>();
        /// <summary>
        /// The combine flags
        /// </summary>
        private CombineFlags combineFlags;
        /// <summary>
        /// The material combine option
        /// </summary>
        private MaterialCombineOption materialCombineOption;
        /// <summary>
        /// The mesh combine descriptors set
        /// </summary>
        private List<List<MeshCombineDescriptor>> meshCombineDescriptorsSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvatarMeshCombiner" /> class.
        /// </summary>
        /// <param name="combineFlags">The combine flags.</param>
        /// <param name="materialCombineOption">The material combine option.</param>
        public AvatarMeshCombiner(CombineFlags combineFlags, MaterialCombineOption materialCombineOption)
        {
            this.combineFlags = combineFlags;
            this.materialCombineOption = materialCombineOption;
        }
        /// <summary>
        /// Combines the specified targetMeshes.
        /// </summary>
        /// <param name="targetMeshes">The targetMeshes.</param>
        /// <param name="combinedMesh">The combined mesh.</param>
        /// <param name="combineFlags">The combine flags.</param>
        /// <param name="materialCombineOption">The material combine option.</param>
        public static void CombineSkinnedMesh(SkinnedMeshRenderer[] targetMeshes, SkinnedMeshRenderer combinedMesh, CombineFlags combineFlags, MaterialCombineOption materialCombineOption)
        {
            if (targetMeshes == null || targetMeshes.Length == 0 || (combineFlags.HasFlag(CombineFlags.UseMaterialCombine) && !MaterialCombineCheck(targetMeshes)))
                return;


            using (AvatarMeshCombiner combiner = new AvatarMeshCombiner(combineFlags, materialCombineOption))
            {
                combiner.ApplySkinnedMeshs(targetMeshes);
                combiner.Combine(combinedMesh);
            }


            if (combineFlags.HasFlag(CombineFlags.RemoveTargetMeshes))
                RemoveTargetMeshs(targetMeshes);
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Adds the skinned meshs.
        /// </summary>
        /// <param name="targetMeshes">The target meshes.</param>
        private void ApplySkinnedMeshs(SkinnedMeshRenderer[] targetMeshes)
        {
            Matrix4x4 referenceTransform = Matrix4x4.identity;
            Dictionary<Transform, List<Matrix4x4>> bonePoseListDic = new Dictionary<Transform, List<Matrix4x4>>();
            Dictionary<(Transform, Matrix4x4), int> boneIdxDic = new Dictionary<(Transform, Matrix4x4), int>();


            HashSet<int> boneCheck = new HashSet<int>();
            List<Matrix4x4> bindposeList = new List<Matrix4x4>();
            Dictionary<int, int> newBoneIdxDic = new Dictionary<int, int>();

            this.meshCombineDescriptorsSet = InitMeshCombineDescriptor(targetMeshes, combineFlags, materialCombineOption);
            Dictionary<SkinnedMeshRenderer, (BoneWeight[], Matrix4x4, Mesh)> meshCombineDic = new Dictionary<SkinnedMeshRenderer, (BoneWeight[], Matrix4x4, Mesh)>();
            List<MeshCombineDescriptor> meshCombineDescriptors = meshCombineDescriptorsSet.SelectMany(item => item).ToList();
            foreach (var meshCombineDescriptor in meshCombineDescriptors)
            {
                SkinnedMeshRenderer parentMesh = meshCombineDescriptor.parentMesh;

                if (!meshCombineDic.ContainsKey(parentMesh))
                {

                    bindposeList.Clear();
                    newBoneIdxDic.Clear();
                    boneCheck.Clear();

                    var orgMesh = parentMesh.sharedMesh;

                    if (meshCombineDic.Count == 0)
                        bounds = parentMesh.bounds;
                    else
                        bounds.Encapsulate(parentMesh.bounds);
                    var mesh = Instantiate(orgMesh);
                    var bones = parentMesh.bones;

                    mesh.GetBindposes(bindposeList);

                    var resultTransform = Matrix4x4.identity;
                    var inverseReTransform = Matrix4x4.identity;
                    int rootIndex = 0;
                    if (bindposeList.Count > 0)
                    {
                        if (bones.Contains(parentMesh.rootBone))
                            rootIndex = Array.IndexOf(bones, parentMesh.rootBone);
                        if (meshCombineDic.Count == 0)
                            referenceTransform = bindposeList[rootIndex];
                        else
                        {
                            resultTransform = referenceTransform.inverse * bindposeList[rootIndex];
                            inverseReTransform = resultTransform.inverse;
                        }
                    }

                    int capacityBones = allBoneList.Count + bones.Length;
                    if (allBoneList.Capacity < capacityBones) allBoneList.Capacity = capacityBones;

                    int capacityBindposes = allBindposeList.Count + bindposeList.Count;
                    if (allBindposeList.Capacity < capacityBindposes) allBindposeList.Capacity = capacityBindposes;

                    var boneWeightList = mesh.boneWeights;
                    foreach (var weight in boneWeightList)
                    {
                        if (weight.weight0 > 0)
                            boneCheck.Add(weight.boneIndex0);
                        if (weight.weight1 > 0)
                            boneCheck.Add(weight.boneIndex1);
                        if (weight.weight2 > 0)
                            boneCheck.Add(weight.boneIndex2);
                        if (weight.weight3 > 0)
                            boneCheck.Add(weight.boneIndex3);
                    }

                    for (int i = 0; i < bones.Length; i++)
                    {
                        if (!boneCheck.Contains(i)) continue;
                        Matrix4x4 bindpose = bindposeList[i] * inverseReTransform;
                        bool isApproximated = false;
                        List<Matrix4x4> boneBindPoseList;
                        if (bonePoseListDic.TryGetValue(bones[i], out boneBindPoseList))
                        {
                            foreach (var boneBindPose in boneBindPoseList)
                            {
                                for (int j = 0; j < 16; j++)
                                {
                                    isApproximated = true;
                                    if (!Mathf.Approximately(bindpose[j], boneBindPose[j]))
                                    {
                                        isApproximated = false;
                                        break;
                                    }
                                }
                                if (isApproximated)
                                {
                                    newBoneIdxDic[i] = boneIdxDic[(bones[i], boneBindPose)];
                                    break;
                                }
                            }
                        }
                        if (!isApproximated)
                        {
                            newBoneIdxDic[i] = boneIdxDic[(bones[i], bindpose)] = allBoneList.Count;
                            allBoneList.Add(bones[i]);
                            allBindposeList.Add(bindpose);
                            if (!bonePoseListDic.ContainsKey(bones[i]))
                                bonePoseListDic[bones[i]] = new List<Matrix4x4>();

                            bonePoseListDic[bones[i]].Add(bindpose);
                        }
                    }

                    for (int i = 0, idx; i < boneWeightList.Length; i++)
                    {
                        BoneWeight boneWeight = boneWeightList[i];

                        if (boneWeightList[i].weight0 > 0 && newBoneIdxDic.TryGetValue(boneWeight.boneIndex0, out idx))
                            boneWeight.boneIndex0 = idx;
                        else
                            boneWeight.weight0 = boneWeight.boneIndex0 = 0;
                        if (boneWeightList[i].weight1 > 0 && newBoneIdxDic.TryGetValue(boneWeight.boneIndex1, out idx))
                            boneWeight.boneIndex1 = idx;
                        else
                            boneWeight.weight1 = boneWeight.boneIndex1 = 0;
                        if (boneWeightList[i].weight2 > 0 && newBoneIdxDic.TryGetValue(boneWeight.boneIndex2, out idx))
                            boneWeight.boneIndex2 = idx;
                        else
                            boneWeight.weight2 = boneWeight.boneIndex2 = 0;
                        if (boneWeightList[i].weight3 > 0 && newBoneIdxDic.TryGetValue(boneWeight.boneIndex3, out idx))
                            boneWeight.boneIndex3 = idx;
                        else
                            boneWeight.weight3 = boneWeight.boneIndex3 = 0;
                        boneWeightList[i] = boneWeight;
                    }
                    meshCombineDic[parentMesh] = (boneWeightList, resultTransform, mesh);
                }

                var _boneWeightList = meshCombineDic[parentMesh].Item1;
                var _resultTransform = meshCombineDic[parentMesh].Item2;
                var _mesh = meshCombineDic[parentMesh].Item3;
                var copyMaterial = combineFlags.HasFlag(CombineFlags.UseMaterialCombine) ? false : combineFlags.HasFlag(CombineFlags.RemoveTargetMeshes) ? true : false;

                CombineInstance combineInstance = meshCombineDescriptor.subMesCombineInstance;
                Material material = parentMesh.materials[combineInstance.subMeshIndex];
                SubMeshDescriptor subMeshDescriptor = combineInstance.mesh.GetSubMesh(combineInstance.subMeshIndex);
                combineInstance.transform = _resultTransform;
                meshCombineDescriptor.subMesCombineInstance = combineInstance;
                meshCombineDescriptor.material = copyMaterial ? CopyNewMaterial(material, materialCombineOption.textureResolutionRatio) : material;
                meshCombineDescriptor.boneWeights = new ArraySegment<BoneWeight>(_boneWeightList, subMeshDescriptor.firstVertex, subMeshDescriptor.vertexCount);

            }
        }

        /// <summary>
        /// Combines the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        private Mesh Combine(SkinnedMeshRenderer destination)
        {

            List<MeshCombineDescriptor> allCombineDescriptors = meshCombineDescriptorsSet.SelectMany(item => item).ToList();


            Dictionary<string, BlendShapeData> resultBlendShapeDic = new Dictionary<string, BlendShapeData>();
            List<Material> resultMaterial = new List<Material>();


            var combineInstanceList = allCombineDescriptors.Select(x => x.subMesCombineInstance).ToList();
            var bindPoseArray = allBindposeList.ToArray();
            var name = destination.name;
            var combinedNewMesh = new Mesh { name = name };
            int vertexCount = 0;

            foreach (var combine in combineInstanceList)
            {
                vertexCount += combine.mesh.GetSubMesh(combine.subMeshIndex).vertexCount;
                if (vertexCount > ushort.MaxValue)
                {
                    combinedNewMesh.indexFormat = IndexFormat.UInt32;
                    break;
                }
            }

            if (combineFlags.HasFlag(CombineFlags.UseMaterialCombine))
            {
                if (!materialCombineOption.textureAtlasOptimization || (materialCombineOption.textureAtlasOptimization && meshCombineDescriptorsSet.Count == 1))
                {
                    combinedNewMesh.CombineMeshes(combineInstanceList.ToArray(), true, true);
                    Material combinedMat = GetCombinedMaterial(combinedNewMesh, allCombineDescriptors);
                    resultMaterial.Add(combinedMat);
                }
                else
                {
                    List<CombineInstance> optimizedInstanceList = new List<CombineInstance>();
                    List<Mesh> optimizedMeshList = new List<Mesh>();

                    foreach (var meshCombineDescriptors in meshCombineDescriptorsSet)
                    {
                        List<CombineInstance> combineInstanceGroup = meshCombineDescriptors.Select(x => x.subMesCombineInstance).ToList();
                        var optimizedMesh = new Mesh();
                        optimizedMesh.CombineMeshes(combineInstanceGroup.ToArray(), true, true);
                        Material combinedMat = GetCombinedMaterial(optimizedMesh, meshCombineDescriptors);

                        optimizedMeshList.Add(optimizedMesh);
                        resultMaterial.Add(combinedMat);
                        optimizedInstanceList.Add(new CombineInstance { mesh = optimizedMesh, subMeshIndex = 0 });
                    }
                    combinedNewMesh.CombineMeshes(optimizedInstanceList.ToArray(), false, false);
                    foreach (var optimizedMesh in optimizedMeshList)
                        DestroyImmediate(optimizedMesh);
                    Resources.UnloadUnusedAssets();
                }
            }
            else
            {
                combinedNewMesh.CombineMeshes(combineInstanceList.ToArray(), false, true);
                resultMaterial.AddRange(allCombineDescriptors.Select(x => x.material));
            }

            combinedNewMesh.boneWeights = allCombineDescriptors.Select(x => x.boneWeights).Where(x => x != null).SelectMany(x => x).ToArray();
            combinedNewMesh.bindposes = bindPoseArray;

            if (!combineFlags.HasFlag(CombineFlags.RemoveBlendshapes))
            {
                int offset = 0;
                foreach (var combine in combineInstanceList)
                {
                    Mesh mesh = combine.mesh;
                    SubMeshDescriptor descriptor = mesh.GetSubMesh(combine.subMeshIndex);

                    for (int shapeIndex = 0; shapeIndex < mesh.blendShapeCount; shapeIndex++)
                    {
                        var shapeName = mesh.GetBlendShapeName(shapeIndex);

                        BlendShapeData blendShape;
                        if (!blendFramesDic.TryGetValue((shapeName, mesh), out blendShape))
                        {
                            blendFramesDic[(shapeName, mesh)] = blendShape = new BlendShapeData();
                            blendShape.deltaVertices = new Vector3[mesh.vertexCount];
                            blendShape.deltaNormals = new Vector3[mesh.vertexCount];
                            blendShape.deltaTangents = new Vector3[mesh.vertexCount];
                            mesh.GetBlendShapeFrameVertices(shapeIndex, 0, blendShape.deltaVertices, blendShape.deltaNormals, blendShape.deltaTangents);
                        }

                        BlendShapeData resultBlendShape;
                        if (!resultBlendShapeDic.TryGetValue(shapeName, out resultBlendShape))
                        {
                            resultBlendShapeDic[shapeName] = resultBlendShape = new BlendShapeData();
                            resultBlendShape.deltaVertices = new Vector3[combinedNewMesh.vertexCount];
                            resultBlendShape.deltaNormals = new Vector3[combinedNewMesh.vertexCount];
                            resultBlendShape.deltaTangents = new Vector3[combinedNewMesh.vertexCount];
                        }

                        Array.Copy(blendShape.deltaVertices, descriptor.firstVertex, resultBlendShape.deltaVertices, offset, descriptor.vertexCount);
                        Array.Copy(blendShape.deltaNormals, descriptor.firstVertex, resultBlendShape.deltaNormals, offset, descriptor.vertexCount);
                        Array.Copy(blendShape.deltaTangents, descriptor.firstVertex, resultBlendShape.deltaTangents, offset, descriptor.vertexCount);
                        if (!(combine.transform == null || combine.transform == Matrix4x4.identity))
                        {
                            ApplyTransform(resultBlendShape.deltaVertices, combine.transform, offset, descriptor.vertexCount);
                            ApplyTransform(resultBlendShape.deltaNormals, combine.transform, offset, descriptor.vertexCount);
                            ApplyTransform(resultBlendShape.deltaTangents, combine.transform, offset, descriptor.vertexCount);
                        }
                    }
                    offset += descriptor.vertexCount;

                }
                foreach (var pair in resultBlendShapeDic)
                    combinedNewMesh.AddBlendShapeFrame(pair.Key, 1.0f, pair.Value.deltaVertices, pair.Value.deltaNormals, pair.Value.deltaTangents);
            }

            //combinedNewMesh.RecalculateNormals();
            //combinedNewMesh.RecalculateTangents();
            combinedNewMesh.RecalculateBounds();
            combinedNewMesh.UploadMeshData(false);
            var rootBoneInverse = destination.rootBone.worldToLocalMatrix;
            bounds.center = rootBoneInverse.MultiplyPoint(bounds.center);
            bounds.extents = rootBoneInverse.MultiplyVector(bounds.extents);
            destination.localBounds = bounds;
            destination.sharedMesh = combinedNewMesh;
            destination.bones = allBoneList.ToArray();
            destination.sharedMaterials = resultMaterial.ToArray();
            destination.shadowCastingMode = ShadowCastingMode.Off;
            destination.receiveShadows = false;
            destination.updateWhenOffscreen = true;
            return combinedNewMesh;
        }
        /// <summary>
        /// Gets the combined material.
        /// </summary>
        /// <param name="combinedNewMesh">The combined new mesh.</param>
        /// <param name="combineDescriptorGroup">The combine descriptor group.</param>
        /// <returns></returns>
        private Material GetCombinedMaterial(Mesh combinedNewMesh, List<MeshCombineDescriptor> combineDescriptorGroup)
        {
            List<Material> materialGroup = combineDescriptorGroup.Select(x => x.material).ToList();
            var (materialTextureInfoDic, combinedMaterial) = CombineMaterial(materialGroup, materialCombineOption.textureResolutionRatio);

            int _offset = 0;
            Vector2[] newRatioUV = new Vector2[combinedNewMesh.uv.Length];
            foreach (var combineDescriptor in combineDescriptorGroup)
            {
                CombineInstance combine = combineDescriptor.subMesCombineInstance;
                TextureInfoBase textureInfoBase = materialTextureInfoDic[combineDescriptor.material];
                SubMeshDescriptor subMesh = combine.mesh.GetSubMesh(combine.subMeshIndex);
                int uvLength = subMesh.vertexCount;
                int firstVertex = subMesh.firstVertex;
                Vector2[] meshuv = combine.mesh.uv;
                Array.Fill(newRatioUV, textureInfoBase.offsetRatio, _offset, uvLength);
                for (int i = _offset, j = firstVertex; i < _offset + uvLength; i++, j++)
                {
                    meshuv[j].x = Mathf.Repeat(meshuv[j].x, 1.0f);
                    meshuv[j].y = Mathf.Repeat(meshuv[j].y, 1.0f);
                    newRatioUV[i] += meshuv[j] * textureInfoBase.sizeRatio;
                }
                _offset += uvLength;
            }

            combinedNewMesh.uv = newRatioUV;
            return combinedMaterial;
        }

        /// <summary>
        /// Initializes the mesh combine descriptor.
        /// </summary>
        /// <param name="targetMeshes">The target meshes.</param>
        /// <param name="combineFlags">The combine flags.</param>
        /// <param name="materialCombineOption">The material combine option.</param>
        /// <returns></returns>
        private static List<List<MeshCombineDescriptor>> InitMeshCombineDescriptor(SkinnedMeshRenderer[] targetMeshes, CombineFlags combineFlags, MaterialCombineOption materialCombineOption)
        {

            List<List<MeshCombineDescriptor>> meshCombineDescriptorsSet;
            List<MeshCombineDescriptor> meshCombineDescriptors = MakeMeshCombineDescriptors(targetMeshes);

            if (combineFlags.HasFlag(CombineFlags.UseMaterialCombine) && materialCombineOption.textureAtlasOptimization)
                meshCombineDescriptorsSet = GetOptimizedMeshCombineDescriptorsSet(meshCombineDescriptors);
            else
                meshCombineDescriptorsSet = new List<List<MeshCombineDescriptor>>() { meshCombineDescriptors };
            return meshCombineDescriptorsSet;
        }
        /// <summary>
        /// Gets the optimized mesh combine descriptors set.
        /// </summary>
        /// <param name="meshCombineDescriptors">The mesh combine descriptors.</param>
        /// <returns></returns>
        private static List<List<MeshCombineDescriptor>> GetOptimizedMeshCombineDescriptorsSet(List<MeshCombineDescriptor> meshCombineDescriptors)
        {
            int sumSize = 0;
            HashSet<Texture> textureHash = new HashSet<Texture>();
            meshCombineDescriptors = meshCombineDescriptors.OrderByDescending(o => {
                int size = (int)(o.textureSize.x * o.textureSize.y);
                if (!textureHash.Contains(o.mainTexture))
                {
                    sumSize += size;
                    textureHash.Add(o.mainTexture);
                }
                return size;
            }).ToList();

            if (AvatarTextureCombiner.isPowerofTwo(sumSize))
                return new List<List<MeshCombineDescriptor>>() { meshCombineDescriptors };
            else
            {
                List<List<MeshCombineDescriptor>> meshCombineDescriptorsSet = new List<List<MeshCombineDescriptor>>();
                Stack<int> sizeStack = new Stack<int>();
                for (int resultSize = 1; resultSize < sumSize; resultSize *= 2)
                    sizeStack.Push(resultSize);
                int groupSize = 0;
                int targetSize = sizeStack.Pop();

                int textureLoss = (int)(sumSize * TEXTURE_LOSS_RATE);
                bool allow = (targetSize * 2 - sumSize) < textureLoss;

                List<MeshCombineDescriptor> atalsGroup = new List<MeshCombineDescriptor>();
                Dictionary<Texture, List<MeshCombineDescriptor>> atalsGroupDic = new Dictionary<Texture, List<MeshCombineDescriptor>>();
                foreach (var meshCombineDescriptor in meshCombineDescriptors)
                {

                    if (atalsGroupDic.ContainsKey(meshCombineDescriptor.mainTexture))
                    {
                        atalsGroupDic[meshCombineDescriptor.mainTexture].Add(meshCombineDescriptor);
                        continue;
                    }
                    else
                    {
                        atalsGroupDic[meshCombineDescriptor.mainTexture] = atalsGroup;
                        groupSize += (int)(meshCombineDescriptor.textureSize.x * meshCombineDescriptor.textureSize.y);
                        atalsGroup.Add(meshCombineDescriptor);
                    }

                    if (groupSize == targetSize && !allow)
                    {
                        sumSize -= groupSize;
                        if (sumSize == 0) break;

                        meshCombineDescriptorsSet.Add(atalsGroup);
                        atalsGroup = new List<MeshCombineDescriptor>();

                        while (sizeStack.Peek() > sumSize)
                            sizeStack.Pop();
                        if (sizeStack.Peek() * 2 - sumSize < textureLoss)
                            allow = true;
                        else
                        {
                            targetSize = sizeStack.Pop();
                            groupSize = 0;
                        }
                    }
                }
                if (atalsGroup.Count != 0)
                    meshCombineDescriptorsSet.Add(atalsGroup);

                return meshCombineDescriptorsSet;
            }
        }

        /// <summary>
        /// Makes the mesh combine descriptors.
        /// </summary>
        /// <param name="targetMeshes">The target meshes.</param>
        /// <returns></returns>
        private static List<MeshCombineDescriptor> MakeMeshCombineDescriptors(SkinnedMeshRenderer[] targetMeshes)
        {
            List<MeshCombineDescriptor> meshCombineDescriptors = new List<MeshCombineDescriptor>();
            foreach (var sm in targetMeshes)
                meshCombineDescriptors.AddRange(MeshCombineDescriptor.GetMeshCombineDescriptorsFromSubmeshes(sm));

            return meshCombineDescriptors;
        }

        /// <summary>
        /// Materials the combine check.
        /// </summary>
        /// <param name="targetMeshes">The target meshes.</param>
        /// <returns></returns>
        private static bool MaterialCombineCheck(SkinnedMeshRenderer[] targetMeshes)
        {
            foreach (var sm in targetMeshes)
                foreach (var materia in sm.materials)
                    if (!MaterialCombineVerification(materia))
                        return false;
            return true;
        }
        /// <summary>
        /// Transforms the each.
        /// </summary>
        /// <param name="deltas">The deltas.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        private static void ApplyTransform(Vector3[] deltas, Matrix4x4 transform, int offset, int count)
        {
            if (offset < 0 || deltas.Length < offset || deltas.Length < offset + count)
                return;

            for (var i = offset; i < offset + count; i++)
                deltas[i] = transform.MultiplyVector(deltas[i]);
        }
        /// <summary>
        /// Deletes the meshs.
        /// </summary>
        /// <param name="targetMeshes">The target meshes.</param>
        private static void RemoveTargetMeshs(SkinnedMeshRenderer[] targetMeshes)
        {
            foreach (Renderer targetMesh in targetMeshes)
            {
                if (targetMesh != null || targetMesh.gameObject != null)
                    DestroyImmediate(targetMesh.gameObject);
            }
        }
        /// <summary>
        /// Instantiates the renderer parms.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="textureResolutionRatio">The texture resolution ratio.</param>
        /// <returns></returns>
        public static Renderer InstantiateRendererParms(Renderer renderer, TextureResolutionRatio textureResolutionRatio)
        {
            if (renderer is MeshRenderer)
            {
                Mesh mesh = renderer.gameObject.GetComponent<MeshFilter>().mesh;
                renderer.gameObject.GetComponent<MeshFilter>().mesh = Instantiate(mesh);
            }
            else if (renderer is SkinnedMeshRenderer smr)
                smr.sharedMesh = Instantiate(smr.sharedMesh);

            renderer.material = CopyNewMaterial(renderer.material, textureResolutionRatio);
            return renderer;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            allBindposeList = null;
            allBoneList = null;
            blendFramesDic = null;
            Resources.UnloadUnusedAssets();
        }
    }
}