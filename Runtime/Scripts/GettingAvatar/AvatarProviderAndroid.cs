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
    public class AvatarProviderAndroid : AvatarProvider
    {
        private AndroidJavaObject instance;
        private string[] packageNames;
        public AvatarProviderAndroid(out bool checkInstanced)
        {
            SetInstance();
            checkInstanced = CheckInstanced();
        }

        private void SetInstance()
        {
            var pluginClass = new AndroidJavaClass("samsung.com.fileprovider.FileProviderForUnity");
            instance = pluginClass.CallStatic<AndroidJavaObject>("instance");
        }
        private bool CheckInstanced()
        {
            return instance == null ? false : true;
        }
        public void StartService(GameObject gameObject,string callBackName)
        {
            instance.Call("startService", gameObject.name, callBackName);
        }
        public string[] GetAvatarList(GameObject gameObject, string callBackName)
        {
            packageNames = instance.Call<string[]>("getAvatarList", gameObject.name, callBackName);
            return packageNames != null? packageNames:new string[0];
        }

        public void GetThumbnailPath(string packageName, GameObject gameObject, string callBackName)
        {
            instance.Call("getThumbnailPath", packageName, gameObject.name, callBackName);
        }

        public void GetAvatarPath(string packageName, GameObject gameObject, string callBackName)
        {
            instance.Call("getGltfPath", packageName, gameObject.name, callBackName);
        }

        public void GetTextureResizedAvatarPath(string packageName, string size, GameObject gameObject, string callBackName)
        {
            instance.Call("setTextureResize", packageName, size, gameObject.name, callBackName);
        }


        public void CallEditor(string packageName, GameObject gameObject, string callBackName)
        {
            instance.Call("callEditor", packageName, gameObject.name, callBackName);
        }


    }
}