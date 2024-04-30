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

namespace AvatarPluginForUnity
{
    /// <summary>
    /// Defines the <see cref="LoadDefines" />.
    /// </summary>
    public class LoadDefines
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
            /// The Legacy
            /// </summary>
            Legacy,
            /// <summary>
            /// The None
            /// </summary>
            None,
        }
    }
}