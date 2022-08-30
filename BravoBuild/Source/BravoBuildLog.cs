using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

namespace BravoBuild.Source
{
    [InlineEditor, ReadOnly]
    public class BravoBuildLog : SerializedScriptableObject
    {
        public static BravoBuildLog Instance
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<BravoBuildLog>("Assets/Plugins/BravoBuild/BuildLog.asset");
            }
        }
        public List<BuildInformation> BuildLog = new List<BuildInformation>();

        public string GetLastBuildPath()
        {
            if (BuildLog.Count == 0)
            {
                Debug.LogError("This current build log is empty");
                return "";
            }
            else return BuildLog.Last().Path;
        }

        public void AddBuildLog(string path, DateTime time, string version, BuildType type, float size)
        {
            if (BuildLog == null) BuildLog = new List<BuildInformation>();

            BuildLog.Add(new BuildInformation(path, time, version, type, size));
            Debug.Log($"Build log added {version} @{time.ToString("G")}");
            EditorUtility.SetDirty(this);
        }

        public IEnumerable<string> GetListVersions()
        {
            List<string> newList = new List<string>();
            foreach (var log in BuildLog)
            {
                if (log.CheckPath()) newList.Add(log.Version);
            }
            return newList;
        }

        public BuildInformation GetBuildWithVersion(string versionString)
        {
            foreach (BuildInformation buildInfo in BuildLog)
            {
                if (buildInfo.Version == versionString) return buildInfo;
            }
            return null;
        }
    }
}
#endif