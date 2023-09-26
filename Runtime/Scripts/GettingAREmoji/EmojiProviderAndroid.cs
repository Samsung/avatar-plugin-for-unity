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
    public class EmojiProviderAndroid : EmojiProvider
    {
        private readonly AndroidJavaObject _androidJavaObject;

        private int _isFileProviderSupported = -1;

        public EmojiProviderAndroid()
        {
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        public bool IsEmojiProviderSupported()
        {
            if (_isFileProviderSupported == -1)
            {
                _isFileProviderSupported = _androidJavaObject.Call<bool>("isFileProviderSupported") ? 1 : 0;
            }

            return _isFileProviderSupported == 1;
        }

        public void StartGetEmojiList(string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("startGetListActivity", gameObjectCallback, methodCallback);
        }

        public string[] GetEmojiPackageList()
        {
            return _androidJavaObject.Call<string[]>("getModelPackageList");
        }

        public void StartGetEmojiPreview(string emojiPackage, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("startGetPreviewActivity", emojiPackage, gameObjectCallback, methodCallback);
        }

        public string GetEmojiPreviewURI(string emojiPackage)
        {
            return _androidJavaObject.Call<string>("getModelPreviewFileURI", emojiPackage);
        }

        public void StartGetEmoji(string emojiPackage, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("startGetFileActivity", emojiPackage, gameObjectCallback, methodCallback);
        }

        public string GetEmojiURI(string emojiPackage)
        {
            return _androidJavaObject.Call<string>("getModelPackageFileURI", emojiPackage);
        }

        public void CallEditor(string emojiPackage, string category, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("callEditor", emojiPackage, category, gameObjectCallback, methodCallback);
        }

        public void SetAvatarTextureResize(string emojiPackage, string resolution, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("setTextureResize", emojiPackage, resolution, gameObjectCallback, methodCallback);
        }

        public string[] GetAvatarList(string gameObjectCallback, string methodCallback)
        {
            return _androidJavaObject.Call<string[]>("getAvatarList", gameObjectCallback, methodCallback);
        }
        public void GetAvatarThumbnailUri(string emojiPackage, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("getAvatarThumbnailUri", emojiPackage, gameObjectCallback, methodCallback);
        }
        public void GetAvatarZipUri(string emojiPackage, string gameObjectCallback, string methodCallback)
        {
            _androidJavaObject.Call("getAvatarZipUri", emojiPackage, gameObjectCallback, methodCallback);
        }
    }
}