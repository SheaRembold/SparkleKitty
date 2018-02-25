﻿//-----------------------------------------------------------------------
// <copyright file="BuildHelper.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCoreInternal
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class BuildHelper : IPreprocessBuild
    {
        [SuppressMessage("UnityRules.UnityStyleRules", "US1000:FieldsMustBeUpperCamelCase",
         Justification = "Overriden property.")]
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            var isARCoreRequired = ARCoreProjectSettings.Instance.IsARCoreRequired;

            Debug.LogFormat("Building application with {0} ARCore support.",
                isARCoreRequired ? "REQUIRED" : "OPTIONAL");

            const string k_RequiredAARPath = "Assets/3rdParty/GoogleARCore/SDK/Plugins/google_ar_required.aar";
            const string k_OptionalAARPath = "Assets/3rdParty/GoogleARCore/SDK/Plugins/google_ar_optional.aar";
            PluginImporter arRequiredAAR = AssetImporter.GetAtPath(k_RequiredAARPath) as PluginImporter;
            PluginImporter arOptionalAAR = AssetImporter.GetAtPath(k_OptionalAARPath) as PluginImporter;

            if (arRequiredAAR == null || arOptionalAAR == null)
            {
                throw new UnityEditor.Build.BuildFailedException(
                    "Not finding google_ar_required.aar and google_ar_optional.aar files needed for ARCore support. " +
                    "Were they moved from the ARCore SDK?");
            }

            arRequiredAAR.SetCompatibleWithPlatform(BuildTarget.Android, isARCoreRequired);
            arOptionalAAR.SetCompatibleWithPlatform(BuildTarget.Android, !isARCoreRequired);
        }
    }
}
