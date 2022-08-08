using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

//TODO Size
//TODO Was published ? 
[System.Serializable]
public class BuildInformation
{
    [GUIColor("@BravoBuildUtils.VerifyFileExists(Path)")]

    [ReadOnly]
    public string Path;


    [HideInInspector, OdinSerialize]
    public DateTime Date;

    [ReadOnly]
    [LabelText("Date"), ShowInInspector]
    public string DateString
    {
        get
        {
            return Date.ToShortDateString();
        }
    }
    [ReadOnly]
    public string Version;

    public BuildInformation(string path, DateTime date, string version)
    {
        Path = path;
        Date = date;
        Version = version;
    }

    public bool CheckPath()
    {
        return File.Exists(Path);
    }

    [ShowIfGroup("ButtonGroup", Condition = "@CheckPath()")]
    [ButtonGroup("ButtonGroup/Buttons")]
    [Button]
    public void OpenFolder()
    {        
        Process.Start(@$"{GetFolderPath()}");
    }

    [ButtonGroup("ButtonGroup/Buttons")]
    [Button]
    public void PlayBuild()
    {
        Process.Start(@$"{Path}");
    }

    public string GetFolderPath()
    {
        int pos = Path.LastIndexOf('/');
        string folderPath = Path.Substring(0, pos);
        return folderPath;
    }
}