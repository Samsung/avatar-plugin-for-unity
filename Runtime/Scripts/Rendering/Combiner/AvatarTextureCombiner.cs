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

namespace AvatarPluginForUnity
{
    /// <summary>
    /// 
    /// </summary>
    internal static class AvatarTextureCombiner
    {
        /// <summary>
        /// Processes the specified source texture information bases.
        /// </summary>
        /// <param name="sourceTextureInfoBases">The source texture information bases.</param>
        /// <param name="resolutionRatio">The resolution ratio.</param>
        /// <returns></returns>
        public static (Texture2D, Texture2D, Texture2D) Process(List<TextureInfoBase> sourceTextureInfoBases)
        {
            HashSet<Texture2D> textureDuplicatedCheck = new HashSet<Texture2D>();
            List<Texture2D> mainTextures = new List<Texture2D>();
            List<Texture2D> normalTextures = new List<Texture2D>();
            List<Texture2D> ao_metallicRoughnessTextures = new List<Texture2D>();

            int texturesSize = 0;
            foreach (var sourceTextureInfoBase in sourceTextureInfoBases)
            {
                if (!textureDuplicatedCheck.Contains(sourceTextureInfoBase.mainTexture2D))
                {
                    textureDuplicatedCheck.Add(sourceTextureInfoBase.mainTexture2D);
                    mainTextures.Add(sourceTextureInfoBase.mainTexture2D);
                    normalTextures.Add(sourceTextureInfoBase.normalTexture2D);
                    ao_metallicRoughnessTextures.Add(sourceTextureInfoBase.Ao_metallicRoughnessTexture2D);
                    texturesSize += (int)(sourceTextureInfoBase.size.x * sourceTextureInfoBase.size.y);
                }
            }
            int maximumAtlasSize = GetMaximumAtlasSize(texturesSize);
            var (combinedMainTexture, rects) = MakeMainAtlasTexture(mainTextures, maximumAtlasSize);
            Vector2Int resultSize = new Vector2Int(combinedMainTexture.width, combinedMainTexture.height);

            Texture2D combinedNormalTexture = MakeEmptyTexture(resultSize);
            Texture2D combinedAo_metallicRoughnessTexture = MakeEmptyTexture(resultSize);
            for (int i=0; i< rects.Length;i++ )
            {
                Vector2 position = rects[i].position * resultSize;
                Vector2 size = rects[i].size * resultSize;
                Graphics.CopyTexture(normalTextures[i], 0, 0, 0, 0, (int)size.x, (int)size.y, combinedNormalTexture, 0, 0, (int)position.x, (int)position.y);
                Graphics.CopyTexture(ao_metallicRoughnessTextures[i], 0, 0, 0, 0, (int)size.x, (int)size.y, combinedAo_metallicRoughnessTexture, 0, 0, (int)position.x, (int)position.y);
            }

            Vector2Int combinedSize = new Vector2Int(combinedMainTexture.width, combinedMainTexture.height);
            foreach (var sourceTextureInfoBase in sourceTextureInfoBases)
            {
                Rect rect = rects[mainTextures.IndexOf(sourceTextureInfoBase.mainTexture2D)];

                sourceTextureInfoBase.sizeRatio = new Vector2((float)sourceTextureInfoBase.size.x / combinedSize.x, (float)sourceTextureInfoBase.size.y / combinedSize.y);
                sourceTextureInfoBase.offsetRatio = new Vector2(rect.x, rect.y);
            }
            return (combinedMainTexture,
                combinedNormalTexture,
                combinedAo_metallicRoughnessTexture);
        }

        /// <summary>
        /// Makes the atlas texture.
        /// </summary>
        /// <param name="targetTextures">The target textures.</param>
        /// <param name="maximumAtlasSize">Maximum size of the atlas.</param>
        /// <param name="newFormat">The new format.</param>
        /// <returns></returns>
        private static (Texture2D,Rect[]) MakeMainAtlasTexture(List<Texture2D> targetTextures, int maximumAtlasSize)
        {
            Texture2D combinedTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false); ;
            Rect[] rects = combinedTexture.PackTextures(targetTextures.ToArray(), 0, maximumAtlasSize);
            return (combinedTexture,rects);
        }
        /// <summary>
        /// Gets the base texture.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="textureFormat">The texture format.</param>
        /// <returns></returns>
        public static Texture2D MakeEmptyTexture(Vector2Int size, TextureFormat textureFormat = TextureFormat.RGB24)
        {
           Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGB24, false);
            texture2D.SetPixel(0, 0, Color.black);
            texture2D.Apply();
            return GetResizedTexture2D(texture2D, size);
        }
        /// <summary>
        /// Gets the resized texture2 d.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="size">The size.</param>
        /// <param name="textureFormat">The texture format.</param>
        /// <returns></returns>
        public static Texture2D GetResizedTexture2D(Texture texture, Vector2Int size, TextureFormat textureFormat = TextureFormat.RGB24)
        {
            RenderTexture rt = new RenderTexture(size.x, size.y, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);
            Texture2D result = new Texture2D(size.x, size.y, textureFormat, false);
            result.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
            result.Apply();
            return result;
        }
        /// <summary>
        /// Gets the maximum size of the atlas.
        /// </summary>
        /// <param name="sumSize">Size of the sum.</param>
        /// <returns></returns>
        private static int GetMaximumAtlasSize(int sumSize)
        {
            Vector2 size = Vector2.one;
            bool toggle = true;
            while (true)
            {
                if (sumSize < size.x * size.y)
                    return (int)size.x;

                if (toggle)
                    size.x *= 2;
                else
                    size.y *= 2;
                toggle = !toggle;
            }
        }
        /// <summary>
        /// Changes the format.
        /// </summary>
        /// <param name="oldTexture">The old texture.</param>
        /// <param name="newFormat">The new format.</param>
        /// <returns></returns>
        private static Texture2D ChangeFormat(Texture2D oldTexture, TextureFormat newFormat)
        {
            Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
            newTex.SetPixels(oldTexture.GetPixels());
            newTex.Apply();
            return newTex;
        }
        /// <summary>
        /// Determines whether [is powerof two] [the specified n].
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>
        ///   <c>true</c> if [is powerof two] [the specified n]; otherwise, <c>false</c>.
        /// </returns>
        public static bool isPowerofTwo(int n)
        {
            if (n == 0)
                return false;
            while (n != 1)
            {
                if (n % 2 != 0)
                    return false;
                n = n / 2;
            }
            return true;
        }

    }

}