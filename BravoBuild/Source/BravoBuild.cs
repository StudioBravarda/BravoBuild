
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using LMS.Version;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.Callbacks;

namespace BravoBuild.Source
{
    public class BravoBuild : OdinMenuEditorWindow, IPreprocessBuildWithReport
    {
        [MenuItem("Build/Window")]
        public static void OpenWindow()
        {
            BravoBuild window = GetWindow<BravoBuild>();
            window.Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false);
            tree.AddAllAssetsAtPath("Build Settings", "Assets/Plugins/BravoBuild/BuildSettings", typeof(BravoBuildSettingsSO), true, true);
            tree.AddAssetAtPath("Build Log", "Assets/Plugins/BravoBuild/BuildLog.asset", typeof(BravoBuildLog));

            return tree;
        }

        public int callbackOrder { get { return 1; } }

        public void OnPreprocessBuild(BuildReport report)
        {

            // throw new NotImplementedException();
        }

        public static void BuildGame(BuildOptions options, BuildTarget target, string[] scenes, BuildType type)
        {

            //
            string basePath = Directory.GetParent(Application.dataPath) + "/Build";
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            int major = LMS.Version.Version.Instance.GameVersion.Major;
            int minor = LMS.Version.Version.Instance.GameVersion.Minor;
            int build = LMS.Version.Version.Instance.GameVersion.Build;

            string productName = Application.productName.Replace(' ', '_');
            //TODO This should be a Enum extension function
            string targetEnumFriendlyString = Enum.GetName(target.GetType(), target);
            string buildFolder = $"/{productName}_{targetEnumFriendlyString}_" + $"{major}_{minor}_{build + 1}";
            //
            var folderInfo = Directory.CreateDirectory(basePath + buildFolder);
            if (type != BuildType.Release)
            {
                productName += $"_{BravoBuildUtils.ToFriendlyString(type)}"; // This will turn ProductName to ProductName_Alpha or ProductName_Demo
            }
            string exePath = folderInfo.FullName + $"/{productName}.exe";

            var report = BuildPipeline.BuildPlayer(scenes, exePath, target, options);
            string version = LMS.Version.Version.GetGameVersion(VersionDeliniator.Dot);
            StringBuilder reportString = new StringBuilder($"{productName} Report\n");
            reportString.AppendLine($"Target Platform: {targetEnumFriendlyString}");
            reportString.AppendLine($"Result: {report.summary.result}");
            reportString.AppendLine($"Size (Mb): {report.summary.totalSize * 0.000001f}");
            reportString.AppendLine($"Build Options: {options.ToString()}");
            reportString.AppendLine($"Version: {version}");
            reportString.AppendLine($"Build Started @: {report.summary.buildStartedAt.ToString("G")}");
            reportString.AppendLine($"Build Ended @: {report.summary.buildEndedAt.ToString("G")}");

            if (report.summary.result == BuildResult.Succeeded)
            {
                //TODO Open directory
                BravoBuildLog.Instance.AddBuildLog(report.summary.outputPath, DateTime.Now, version, type, report.summary.totalSize * 0.000001f);
            }
            else
            {
                LMS.Version.Version.Instance.GameVersion.Build -= 1;
            }
            Console.WriteLine(reportString.ToString());
            //
            Debug.Log(reportString.ToString());
        }

        
    }
}
#endif

