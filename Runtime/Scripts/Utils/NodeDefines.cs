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
    public class NodeDefines
    {
        /// <summary>
        /// Defines the HEAD_GRP.
        /// </summary>
        public static string LOAD_NODE = "LoadNode";
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
        /// Defines the BODY.
        /// </summary>
        public static string BODY = "BODY";
        /// <summary>
        /// The female body
        /// </summary>
        public static string FemaleBody = "asian_adult_female_GRP";
        /// <summary>
        /// The male body
        /// </summary>
        public static string MaleBody = "asian_adult_male_GRP";
        /// <summary>
        /// The junior body
        /// </summary>
        public static string JuniorBody = "asian_junior_male_GRP";
        /// <summary>
        /// Defines the INCLUDED_HEADONLY_PARENTS.
        /// </summary>
        public static List<string> INCLUDED_HEADONLY_PARENTS = new List<string>() { "head_GRP", "hair_GRP", "eyebrow_GRP", "eyelash_GRP", "audio_GRP", "ear_deco_l_GRP", "ear_deco_r_GRP",
            "headwear_GRP", "mask_GRP", "eye_patch_GRP","earrings_GRP","lip_deco_GRP","nose_deco_GRP","glasses_GRP" };
    }
}

