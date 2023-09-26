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
using System.Linq;
using UnityEngine;

namespace AvatarPluginForUnity
{
    [Serializable]
    public class AREmojiInfo
    {
        public string gltfURL = "";
        public string previewURL = "";
    }

    public class EmojiProviderEditor : EmojiProvider
    {
        private Dictionary<string, AREmojiInfo> _mockupArEmojis = new Dictionary<string, AREmojiInfo>();
        private GameObject _gameObject;

        public EmojiProviderEditor()
        {

        }

        public bool IsEmojiProviderSupported()
        {
            return true;
        }

        public void StartGetEmojiList(string gameObjectCallback, string methodCallback)
        {
            _gameObject = GameObject.Find(gameObjectCallback);
            _gameObject.SendMessage(methodCallback, "");
        }

        public string[] GetEmojiPackageList()
        {
            return _mockupArEmojis.Keys.ToArray();
        }

        public void StartGetEmojiPreview(string emojiPackage, string gameObjectCallback, string methodCallback)
        {

        }

        public string GetEmojiPreviewURI(string emojiPackage)
        {
            return "";
        }

        public void StartGetEmoji(string emojiPackage, string gameObjectCallback, string methodCallback)
        {
            _gameObject = GameObject.Find(gameObjectCallback);
            _gameObject.SendMessage(methodCallback, emojiPackage);
        }

        public string GetEmojiURI(string emojiPackage)
        {
            return _mockupArEmojis[emojiPackage].gltfURL;
        }

        public void CallEditor(string emojiPackage, string category, string gameObjectCallback, string methodCallback)
        {
            _gameObject = GameObject.Find(gameObjectCallback);
            _gameObject.SendMessage(methodCallback, "");
        }

        public void SetAvatarTextureResize(string emojiPackage, string resolution, string gameObjectCallback, string methodCallback)
        {
            _gameObject = GameObject.Find(gameObjectCallback);
            _gameObject.SendMessage(methodCallback, "");
        }
        public string[] GetAvatarList(string gameObjectCallback, string methodCallback)
        {
            return _mockupArEmojis.Keys.ToArray();
        }
        public void GetAvatarThumbnailUri(string emojiPackage, string gameObjectCallback, string methodCallback)
        {

        }
        public void GetAvatarZipUri(string emojiPackage, string gameObjectCallback, string methodCallback)
        {

        }
    }
}