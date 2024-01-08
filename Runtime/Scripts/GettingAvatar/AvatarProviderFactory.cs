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
using UnityEngine;

namespace AvatarPluginForUnity
{
    public interface AvatarProvider
    {
        void StartService(GameObject gameObject, string callBackName);
        string[] GetAvatarList(GameObject gameObject, string callBackName);
        void GetThumbnailPath(string packageName, GameObject gameObject, string callBackName);
        void GetAvatarPath(string packageName, GameObject gameObject, string callBackName);
        void GetTextureResizedAvatarPath(string packageName, string size, GameObject gameObject, string callBackName);
        void CallEditor(string packageName, GameObject gameObject, string callBackName);
    }

    /**
     * Used to get an implementation of AvatarProvider.
      */
    public static class AvatarProviderFactory
    {
        private static AvatarProvider _avatarProvider;

        public static AvatarProvider GetAvatarProvider()
        {
            bool checkInstanced = false;
            if (_avatarProvider == null)
            {

                if (Application.platform == RuntimePlatform.Android)
                {
                    _avatarProvider = new AvatarProviderAndroid(out checkInstanced);
                }
                else
                {
                    _avatarProvider = new AvatarProviderEditor();
                    checkInstanced = true;
                }
            }
            if (!checkInstanced)
                _avatarProvider = null;
            return _avatarProvider;
        }
    }
}