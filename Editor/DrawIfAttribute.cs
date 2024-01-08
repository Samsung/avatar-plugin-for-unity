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
#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)
using UnityEngine;
using System;

namespace AvatarPluginForUnity.Editor
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="UnityEngine.PropertyAttribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public class DrawIfAttribute : PropertyAttribute
    {
        #region Fields
        /// <summary>
        /// Gets the name of the compared property.
        /// </summary>
        /// <value>
        /// The name of the compared property.
        /// </value>
        public string comparedPropertyName { get; private set; }
        /// <summary>
        /// Gets the compared value.
        /// </summary>
        /// <value>
        /// The compared value.
        /// </value>
        public object comparedValue { get; private set; }
        /// <summary>
        /// Gets the type of the disabling.
        /// </summary>
        /// <value>
        /// The type of the disabling.
        /// </value>
        public DisablingType disablingType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public enum DisablingType
        {
            Draw = 1,
            /// <summary>
            /// The draw exclude
            /// </summary>
            DrawExclude = 2

        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawIfAttribute"/> class.
        /// </summary>
        /// <param name="comparedPropertyName">Name of the compared property.</param>
        /// <param name="comparedValue">The compared value.</param>
        /// <param name="disablingType">Type of the disabling.</param>
        public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.Draw)
        {
            this.comparedPropertyName = comparedPropertyName;
            this.comparedValue = comparedValue;
            this.disablingType = disablingType;
        }
    }
}
#endif