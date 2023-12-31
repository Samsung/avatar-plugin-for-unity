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
using System;
using System.Collections.Generic;

namespace AvatarPluginForUnity
{
    [Serializable]
    public class PoseData
    {
        /// <summary>
        /// The female
        /// </summary>
        public List<BoneData> female;
        /// <summary>
        /// The male
        /// </summary>
        public List<BoneData> male;
        /// <summary>
        /// The junior
        /// </summary>
        public List<BoneData> junior;
    }
}
