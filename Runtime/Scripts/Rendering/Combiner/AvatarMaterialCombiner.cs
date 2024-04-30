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
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Object;
using static AvatarPluginForUnity.AvatarTextureCombiner;
using static AvatarPluginForUnity.MaterialCombineOption;
using GLTFast;

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    internal class AvatarMaterialCombiner
    {
        /// <summary>
        /// Combines the material.
        /// </summary>
        /// <param name="meshCombineOption">The material sources.</param>
        /// <param name="resolutionRatio">The resolution ratio.</param>
        /// <returns></returns>
        public static (Dictionary<Material, TextureInfoBase>, Material) CombineMaterial(List<Material> materialSources, MaterialCombineOption.TextureResolutionRatio resolutionRatio = MaterialCombineOption.TextureResolutionRatio.One)
        {
            var (textureInfoBaseSet,
                materialTextureInfoDic) = MakeTextureInfoBases(materialSources, resolutionRatio);

            var (combinedmainTexture,
                combinedNormalTexture,
                combinedAo_metallicRoughnessTexture) = Process(textureInfoBaseSet);

            return (materialTextureInfoDic, MakeCombinedMaterial(combinedmainTexture,
                combinedNormalTexture,
                combinedAo_metallicRoughnessTexture));
        }
        /// <summary>
        /// Makes the texture information bases.
        /// </summary>
        /// <param name="materialSources">The material sources.</param>
        /// <param name="resolutionRatio">The resolution ratio.</param>
        /// <returns></returns>
        private static (List<TextureInfoBase>, Dictionary<Material, TextureInfoBase>) MakeTextureInfoBases(List<Material> materialSources, MaterialCombineOption.TextureResolutionRatio resolutionRatio)
        {
            Dictionary<(Texture, (int, int)), Texture2D> textureDic = new Dictionary<(Texture, (int, int)), Texture2D>();
            Dictionary<(int, int), Texture2D> emptyTextureDic = new Dictionary<(int, int), Texture2D>();

            List<TextureInfoBase> textureInfoBaseSet = new List<TextureInfoBase>();
            Dictionary<Material, TextureInfoBase> materialTextureInfoDic = new Dictionary<Material, TextureInfoBase>();

            foreach (var materialSource in materialSources)
            {
                Texture mainTexture = materialSource.GetTexture("baseColorTexture");
                Texture normalTexture = materialSource.GetTexture("normalTexture");
                Texture Ao_metallicRoughnessTexture = materialSource.GetTexture("metallicRoughnessTexture");
                if (Ao_metallicRoughnessTexture == null)
                    Ao_metallicRoughnessTexture = materialSource.GetTexture("occlusionTexture");

                Vector2Int size = new Vector2Int(mainTexture.width / (int)resolutionRatio, mainTexture.height / (int)resolutionRatio);
                TextureInfoBase textureInfoBase = new TextureInfoBase();
                textureInfoBase.size = size;

                //Set mainTexture
                if (!textureDic.ContainsKey((mainTexture, (size.x, size.y))))
                    textureDic[(mainTexture, (size.x, size.y))] = GetResizedTexture2D(mainTexture, size, TextureFormat.RGBA32);
                textureInfoBase.mainTexture2D = textureDic[(mainTexture, (size.x, size.y))];

                //Set normalTexture
                if (normalTexture == null)
                {
                    if(emptyTextureDic.ContainsKey((size.x, size.y)))
                        emptyTextureDic[(size.x, size.y)] = MakeEmptyTexture(size);
                    Texture2D emptyTexture = emptyTextureDic[(size.x, size.y)];
                    normalTexture = textureDic[(emptyTexture, (size.x, size.y))] = emptyTexture;
                }
                else if (!textureDic.ContainsKey((normalTexture, (size.x, size.y))))
                    textureDic[(normalTexture, (size.x, size.y))] = GetResizedTexture2D(normalTexture, size);
                textureInfoBase.normalTexture2D = textureDic[(normalTexture, (size.x, size.y))];

                //Set Ao_metallicRoughnessTexture
                if (Ao_metallicRoughnessTexture == null)
                {
                    if (emptyTextureDic.ContainsKey((size.x, size.y)))
                        emptyTextureDic[(size.x, size.y)] = MakeEmptyTexture(size);
                    Texture2D emptyTexture = emptyTextureDic[(size.x, size.y)];
                    Ao_metallicRoughnessTexture = textureDic[(emptyTexture, (size.x, size.y))] = emptyTexture;
                }
                else if (!textureDic.ContainsKey((Ao_metallicRoughnessTexture, (size.x, size.y))))
                    textureDic[(Ao_metallicRoughnessTexture, (size.x, size.y))] = GetResizedTexture2D(Ao_metallicRoughnessTexture, size);
                textureInfoBase.Ao_metallicRoughnessTexture2D = textureDic[(Ao_metallicRoughnessTexture, (size.x, size.y))];

                textureInfoBaseSet.Add(textureInfoBase);

                materialTextureInfoDic[materialSource] = textureInfoBase;
            }

            return (textureInfoBaseSet, materialTextureInfoDic);
        }

        /// <summary>
        /// Makes the combined material.
        /// </summary>
        /// <param name="combinedmainTexture">The combinedmain texture.</param>
        /// <param name="combinedNormalTexture">The combined normal texture.</param>
        /// <param name="combinedAo_metallicRoughnessTexture">The combined ao metallic roughness texture.</param>
        /// <returns></returns>
        private static Material MakeCombinedMaterial(Texture2D combinedmainTexture,
            Texture2D combinedNormalTexture,
            Texture2D combinedAo_metallicRoughnessTexture)
        {

            var isURP = RenderPipelineUtils.RenderPipeline.Equals(GLTFast.RenderPipeline.Universal);

            Material combinedMat = null;
            if (isURP)
                combinedMat = Instantiate(Resources.Load("avatar-urp-combine-material", typeof(Material)) as Material);
            else
            {
                combinedMat = new Material(Shader.Find("glTF/PbrMetallicRoughness"));

                combinedMat.EnableKeyword("_ALPHATEST_ON");
                combinedMat.SetInt(Shader.PropertyToID("_ZWrite"), 1);
                combinedMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                combinedMat.renderQueue = (int)RenderQueue.AlphaTest;

                combinedMat.SetOverrideTag("RenderType", "TransparentCutout");
                combinedMat.SetInt(Shader.PropertyToID("_SrcBlend"), (int)BlendMode.One);
                combinedMat.SetInt(Shader.PropertyToID("_DstBlend"), (int)BlendMode.Zero);
                combinedMat.DisableKeyword("_ALPHABLEND_ON");

                combinedMat.SetInt(Shader.PropertyToID("_ZWrite"), 1);
                combinedMat.SetFloat(Shader.PropertyToID("_CullMode"), (int)CullMode.Off);

                combinedMat.EnableKeyword("_METALLICGLOSSMAP");
                combinedMat.EnableKeyword("_NORMALMAP");
                combinedMat.EnableKeyword("_OCCLUSION");

            }
            combinedmainTexture.Apply();
            combinedNormalTexture.Apply();
            combinedAo_metallicRoughnessTexture.Apply();
            combinedMat.SetTexture("baseColorTexture", combinedmainTexture);
            combinedMat.SetTexture("normalTexture", combinedNormalTexture);
            combinedMat.SetTexture("metallicRoughnessTexture", combinedAo_metallicRoughnessTexture);
            combinedMat.SetTexture("occlusionTexture", combinedAo_metallicRoughnessTexture);

            return combinedMat;
        }

        /// <summary>
        /// Materials the combine verification.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <param name="strictCheck">if set to <c>true</c> [strict check].</param>
        /// <returns></returns>
        public static bool MaterialCombineVerification(Material material, bool strictCheck = false)
        {

            Texture mainTexture = null;
            Texture normalTexture = null;
            Texture Ao_metallicRoughnessTexture = null;


            mainTexture = material.GetTexture("baseColorTexture");
            normalTexture = material.GetTexture("normalTexture");
            Ao_metallicRoughnessTexture = material.GetTexture("metallicRoughnessTexture");
            if (Ao_metallicRoughnessTexture == null)
                Ao_metallicRoughnessTexture = material.GetTexture("occlusionTexture");
            

            if (mainTexture == null)
            {
                Debug.Log("Materials that cannot be combined!!! There is no mainTexture!!");
                return false;
            }

            if (!strictCheck)
                return true;

            else if (!isPowerofTwo(mainTexture.width) || !isPowerofTwo(mainTexture.height))
            {
                Debug.Log("Materials that cannot be combined!!! Neither width nor height, or either one, satisfies POT(Power Of Two).!!");
                return false;
            }


            if (!(normalTexture == null) && (!isPowerofTwo(normalTexture.width) || !isPowerofTwo(normalTexture.height)))
            {
                Debug.Log("Materials that cannot be combined!!! Neither width nor height, or either one, satisfies POT(Power Of Two).!!");
                return false;
            }

            if (!(Ao_metallicRoughnessTexture == null) && (!isPowerofTwo(Ao_metallicRoughnessTexture.width) || !isPowerofTwo(Ao_metallicRoughnessTexture.height)))
            {
                Debug.Log("Materials that cannot be combined!!! Neither width nor height, or either one, satisfies POT(Power Of Two).!!");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Copies the new material.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns></returns>
        public static Material CopyNewMaterial(Material material, TextureResolutionRatio textureResolutionRatio)
        {
            int resolutionRatio = (int)textureResolutionRatio;

            Material newMaterial = Instantiate(material);
            Texture mainTexture = null;
            Texture normalTexture = null;
            Texture Ao_metallicRoughnessTexture = null;

            mainTexture = material.GetTexture("baseColorTexture");
            normalTexture = material.GetTexture("normalTexture");
            Ao_metallicRoughnessTexture = material.GetTexture("metallicRoughnessTexture");
            if (Ao_metallicRoughnessTexture == null)
                Ao_metallicRoughnessTexture = material.GetTexture("occlusionTexture");
            
            mainTexture = mainTexture!=null?GetResizedTexture2D(mainTexture, new Vector2Int(mainTexture.width / resolutionRatio, mainTexture.height / resolutionRatio), TextureFormat.RGBA32) : null;
            normalTexture = normalTexture != null ? GetResizedTexture2D(normalTexture, new Vector2Int(normalTexture.width / resolutionRatio, normalTexture.height / resolutionRatio)) : null;
            Ao_metallicRoughnessTexture = Ao_metallicRoughnessTexture != null ? GetResizedTexture2D(Ao_metallicRoughnessTexture, new Vector2Int(Ao_metallicRoughnessTexture.width / resolutionRatio, Ao_metallicRoughnessTexture.height / resolutionRatio)) : null;

            if (mainTexture != null)
                newMaterial.SetTexture("baseColorTexture", mainTexture);
            if (normalTexture != null)
                newMaterial.SetTexture("normalTexture", normalTexture);
            if (Ao_metallicRoughnessTexture != null)
            {
                newMaterial.SetTexture("metallicRoughnessTexture", Ao_metallicRoughnessTexture);
                newMaterial.SetTexture("occlusionTexture", Ao_metallicRoughnessTexture);
            }
            
            return newMaterial;
        }
    }
}



