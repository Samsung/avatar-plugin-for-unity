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
    public class BlendShapeAnimation
    {
        public string name { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public List<BlendShape> blendShapes { get; set; }
        public int shapesAmount { get; set; }
        public List<int> time { get; set; }
        public int frames { get; set; }

    }
}
