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
    public interface EmojiProvider
    {
        bool IsEmojiProviderSupported();

        void StartGetEmojiList(string gameObjectCallback, string methodCallback);
        string[] GetEmojiPackageList();

        void StartGetEmojiPreview(string emojiPackage, string gameObjectCallback, string methodCallback);
        string GetEmojiPreviewURI(string emojiPackage);

        void StartGetEmoji(string emojiPackage, string gameObjectCallback, string methodCallback);
        string GetEmojiURI(string emojiPackage);

        void CallEditor(string emojiPackage, string category, string gameObjectCallback, string methodCallback);
        void SetAvatarTextureResize(string emojiPackage, string resolution, string gameObjectCallback, string methodCallback);

        string[] GetAvatarList(string gameObjectCallback, string methodCallback);
        void GetAvatarThumbnailUri(string emojiPackage, string gameObjectCallback, string methodCallback);
        void GetAvatarZipUri(string emojiPackage, string gameObjectCallback, string methodCallback);
    }

    /**
     * Used to get an implementation of EmojiProvider.
      */
    public static class EmojiProviderFactory
    {
        private static EmojiProvider _emojiProvider;

        public static EmojiProvider GetEmojiProvider()
        {
            if (_emojiProvider == null)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    _emojiProvider = new EmojiProviderAndroid();
                }
                else
                {
                    _emojiProvider = new EmojiProviderEditor();
                }
            }

            return _emojiProvider;
        }
    }
}