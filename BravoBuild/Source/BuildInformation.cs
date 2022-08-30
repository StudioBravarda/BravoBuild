using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace BravoBuild.Source
{
    //TODO Size
    [System.Serializable]
    public class BuildInformation
    {
        [GUIColor("@BravoBuildUtils.VerifyFileExists(Path)")]

        [ReadOnly]
        public string Path;


        [HideInInspector, OdinSerialize]
        public DateTime Date
        {
            get
            {
                return DateTime.Parse(DateString);
            }
        }

        [ReadOnly]
        [LabelText("Date"), ShowInInspector]
        public string DateString;
        public float Size;

        [ReadOnly]
        public BuildType BuildType;

        [ReadOnly]
        public string Version;

        public BuildInformation(string path, DateTime date, string version, BuildType buildType, float size)
        {
            Path = path;
            DateString = date.ToString("G");
            Version = version;
            BuildType = buildType;
            Size = size;
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
}