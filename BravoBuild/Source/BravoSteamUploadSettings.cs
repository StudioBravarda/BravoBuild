using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This scriptable object should not be uploaded to git EVER
/// </summary>
[InlineEditor]
[HideReferenceObjectPicker]
public class BravoSteamUploadSettings : SerializedScriptableObject
{
    [FolderPath(AbsolutePath = true), GUIColor("@BravoBuildUtils.VerifyFileExists(GetSteamCMDPath())")]
    public string SteamSDKPath;
    [BoxGroup("Steam Login")]
    public string SteamLogin;
    [BoxGroup("Steam Login")]
    public string SteamPassword;

    public override string ToString()
    {
        return $"SteamCMDPath {SteamSDKPath} \n\n Login:{SteamLogin} Pass:{SteamPassword}\n";
    }
    public bool CheckComplete() => !string.IsNullOrEmpty(SteamSDKPath) && !string.IsNullOrEmpty(SteamLogin) && !string.IsNullOrEmpty(SteamPassword);
    public string GetSteamCMDPath() => SteamSDKPath + "/tools/ContentBuilder/builder/steamcmd.exe";
    public bool CheckSDKCorrect() 
    {
        return File.Exists(SteamSDKPath);
    }
}
