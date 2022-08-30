using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BravoBuild.Source
{
    [InlineEditor]
    [HideReferenceObjectPicker]
    public class BravoItchSettings : SerializedScriptableObject
    {
        [FolderPath(AbsolutePath = true), GUIColor("@BravoBuildUtils.VerifyFileExists(GetButlerPath())")]
        public string ButlerPath;

        public string GetButlerPath() => ButlerPath + "/butler.exe";
    }
}
