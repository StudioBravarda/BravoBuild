
using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using LMS.Version;
using System.Text;
// using Gameloop.Vdf;
// using Gameloop.Vdf.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.Callbacks;

public class BravoBuild : OdinMenuEditorWindow, IPreprocessBuildWithReport
{    
    public static BravoBuildLog buildLog;
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

    public static void BuildGame(BuildOptions options, BuildTarget target, string[] scenes)
    {
        string basePath = System.IO.Directory.GetParent(Application.dataPath) + "/Build";
        if (!System.IO.Directory.Exists(basePath))
        {
            System.IO.Directory.CreateDirectory(basePath);
        }

        int major = LMS.Version.Version.Instance.GameVersion.Major;
        int minor = LMS.Version.Version.Instance.GameVersion.Minor;
        int build = LMS.Version.Version.Instance.GameVersion.Build;

        string productName = Application.productName.Replace(' ', '_');
        //TODO This should be a Enum extension function
        string targetEnumFriendlyString = Enum.GetName(target.GetType(), target);
        string buildFolder = $"/{productName}_{targetEnumFriendlyString}_" + $"{major}_{minor}_{build + 1}";
        //
        var folderInfo = System.IO.Directory.CreateDirectory(basePath + buildFolder);

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
            BravoBuild window = GetWindow<BravoBuild>();
            BravoBuildLog.Instance.AddBuildLog(report.summary.outputPath, DateTime.Now, version);
        }
        else
        {
            LMS.Version.Version.Instance.GameVersion.Build -= 1;
        }
        System.Console.WriteLine(reportString.ToString());
        //
        Debug.Log(reportString.ToString());
    }

    //TODO Remove this to from this class
    public static string ToFriendlyString(Enum code)
    {
        return Enum.GetName(code.GetType(), code);
    }
}
#endif

