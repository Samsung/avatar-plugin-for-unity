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

namespace AvatarPluginForUnity
{
    /// <summary>
    /// Defines the <see cref="Constance" />.
    /// </summary>
    public class Constance
    {
        /// <summary>
        /// 
        /// </summary>
        public enum AssetLocation
        {
            /// <summary>
            /// The streaming asset
            /// </summary>
            StreamingAsset,
            /// <summary>
            /// The server
            /// </summary>
            Server,
            /// <summary>
            /// The else
            /// </summary>
            Else
        }

        /// <summary>
        /// 
        /// </summary>
        public enum LoadType
        {
            /// <summary>
            /// The URL
            /// </summary>
            Url,
            /// <summary>
            /// The stream
            /// </summary>
            Stream
        }

        /// <summary>
        /// 
        /// </summary>
        public enum AnimationType
        {
            /// <summary>
            /// The Humanoid
            /// </summary>
            Humanoid,
            /// <summary>
            /// The Generic
            /// </summary>
            Generic,
            /// <summary>
            /// The None
            /// </summary>
            None,
        }
        /// <summary>
        /// 
        /// </summary>
        public enum BodyType
        {
            /// <summary>
            /// The Female
            /// </summary>
            Female,
            /// <summary>
            /// The Male
            /// </summary>
            Male,
            /// <summary>
            /// The Junior
            /// </summary>
            Junior,
        }

        /// <summary>
        /// Defines the HEAD_GRP.
        /// </summary>
        public static string HEAD_GRP = "head_GRP";

        /// <summary>
        /// Defines the HEAD_GEO.
        /// </summary>
        public static string HEAD_GEO = "head_GEO";

        /// <summary>
        /// Defines the MODEL.
        /// </summary>
        public static string MODEL = "model";

        /// <summary>
        /// Defines the RIG_GRP.
        /// </summary>
        public static string RIG_GRP = "rig_GRP";

        /// <summary>
        /// Defines the HIP_JNT.
        /// </summary>
        public static string HIP_JNT = "hips_JNT";

        /// <summary>
        /// Defines the RootNode.
        /// </summary>
        public static string ROOT_NODE = "RootNode";

        /// <summary>
        /// Defines the model_GRP.
        /// </summary>
        public static string MODEL_GRP = "model_GRP";

        /// <summary>
        /// Defines the REPEAT.
        /// </summary>
        public static string REPEAT = "REPEAT";

        /// <summary>
        /// Defines the ONCE.
        /// </summary>
        public static string ONCE = "ONCE";

        /// <summary>
        /// Defines the HEAD.
        /// </summary>
        public static string HEAD = "HEAD";

        /// <summary>
        /// Defines the BODY.
        /// </summary>
        public static string BODY = "BODY";

        /// <summary>
        /// Defines the INCLUDED_HEADONLY_PARENTS.
        /// </summary>
        public static List<string> INCLUDED_HEADONLY_PARENTS = new List<string>() { "head_GRP", "hair_GRP", "eyebrow_GRP", "eyelash_GRP", "audio_GRP", "ear_deco_l_GRP", "ear_deco_r_GRP", 
            "headwear_GRP", "mask_GRP", "eye_patch_GRP","earrings_GRP","lip_deco_GRP","nose_deco_GRP","glasses_GRP" };

        /// <summary>
        /// Defines the AREMOJI_BASE_SCALE.
        /// </summary>
        public static float AREMOJI_BASE_SCALE = 0.009894463f;

        /// <summary>
        /// Defines the FEMALE_SCALE.
        /// </summary>
        public static float FEMALE_SCALE = 0.7677343064433373f;

        /// <summary>
        /// Defines the MALE_SCALE.
        /// </summary>
        public static float MALE_SCALE = 0.7677343064433373f;

        /// <summary>
        /// Defines the JUNIOR_SCALE.
        /// </summary>
        public static float JUNIOR_SCALE = 0.3585273878997415f;
    }

    public enum MeshCombineType
    {
        Head,
        Body,
    }

}