#if UNITY_EDITOR
using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using UnityEditor;

namespace BravoBuild.Source
{
    [HideMonoScript, InlineEditor]
    public class BravoSteamUploadGameSettings : SerializedScriptableObject
    {
        [TitleGroup("Steam Settings")]
        [ToggleGroup("Steam Settings/Enabled", "$Label")]
        public bool Enabled = false;
        public string Label { get { return $"{this.AppID.ToString()} -> {this.DepotID.ToString()}"; } }
        [ToggleGroup("Steam Settings/PreviewBuild")]
        public bool PreviewBuild;
        [TitleGroup("Steam Settings/Build Type")]
        [EnumToggleButtons()]
        public BuildType SteamIDBuildType;
        [TitleGroup("App Settings")]
        public int AppID;
        [TitleGroup("App Settings")]
        [Sirenix.OdinInspector.FilePath]
        public string AppVDFPath;
        [TitleGroup("Depot Settings")]
        public int DepotID;
        [TitleGroup("Depot Settings")]
        [Sirenix.OdinInspector.FilePath]
        public string DepotVDFPath;

        [TitleGroup("Version To Upload")]
        [GUIColor("SteamBuildTypeVersionBuildTypeCompare")]
        [HorizontalGroup("Version To Upload/Version Info"), HideLabel]
        [ValueDropdown("@BravoBuildLog.Instance.GetListVersions()")] //TODO Check if this version has a folder okay
        public string VersionToUpload;
        [HorizontalGroup("Version To Upload/Version Info"), ShowInInspector, ReadOnly]
        public string VersionBuildType
        {
            get
            {
                BuildInformation buildInfo = BravoBuildLog.Instance.GetBuildWithVersion(VersionToUpload);
                if(buildInfo != null)
                {
                    return BravoBuildUtils.ToFriendlyString(buildInfo.BuildType);
                }
                else
                {
                    return "-";
                }                
            }
        }

        public Color SteamBuildTypeVersionBuildTypeCompare()
        {
            if(BravoBuildLog.Instance.GetBuildWithVersion(VersionToUpload) == null) return Color.red;
            if (BravoBuildLog.Instance.GetBuildWithVersion(VersionToUpload).BuildType == SteamIDBuildType) return Color.green;
            else return Color.red;
        }

        private static string rootVDFFolder = "/Plugins/BravoBuild/VdfFiles";
        private static string sampleAppVDFPath
        {
            get
            {
                return rootVDFFolder + "/sample_app_1000.vdf";
            }
        }

        private static string sampleDepotVDFPath
        {
            get
            {
                return rootVDFFolder + "/sample_depot_1001.vdf";
            }
        }

        private string appVDFFolder
        {
            get
            {
                return rootVDFFolder + $"/{AppID}";
            }
        }

        public override string ToString()
        {
            return $"AppID {AppID} \n AppVDFPath {AppVDFPath} \n DepotID {DepotID} DepotVDFPath {DepotVDFPath}\n";
        }

        public void UpdateVDFs()
        {
            BuildInformation buildInformation = BravoBuildLog.Instance.GetBuildWithVersion(VersionToUpload);
            if (buildInformation.BuildType != SteamIDBuildType)
            {
                Debug.LogError($"This steam upload require a build of type {BravoBuildUtils.ToFriendlyString(SteamIDBuildType)} this build is of type {buildInformation.BuildType}");
                return;
            }
            //
            string appVdfFile = System.IO.File.ReadAllText(AppVDFPath);
            dynamic appvdfObject = VdfConvert.Deserialize(appVdfFile);
            //
            int major = LMS.Version.Version.Instance.GameVersion.Major;
            int minor = LMS.Version.Version.Instance.GameVersion.Minor;
            int build = LMS.Version.Version.Instance.GameVersion.Build;
            //
            appvdfObject.Value.desc = $"V{major}.{minor}.{build}";
            appvdfObject.Value.buildoutput = Application.dataPath + appVDFFolder;
            appvdfObject.Value.preview = PreviewBuild ? 1 : 0;

            VObject depots = new VObject
        {
            new VProperty(DepotID.ToString(), new VValue(DepotVDFPath))
        };
            appvdfObject.Value.depots = depots;
            System.IO.File.WriteAllText(AppVDFPath, appvdfObject.ToString());
            // // // // // // 
            string depotVdfFile = System.IO.File.ReadAllText(DepotVDFPath);

            string chooseBuildFolderPath = buildInformation.GetFolderPath();
            dynamic vdfObjectDepot = VdfConvert.Deserialize(depotVdfFile);

            vdfObjectDepot.Value.contentroot = chooseBuildFolderPath;

            System.IO.File.WriteAllText(DepotVDFPath, vdfObjectDepot.ToString());
        }

        public static void CreateNewGameSettings(int appID, int depotID)
        {
            //Creating directory that holds the vdf files for that particular appid
            string directoryPath = Application.dataPath + rootVDFFolder + $"/{appID}";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            //Creating default depot_appid.vdf file with information about the build
            //This is created first because the app vdf reference this file
            string depotVDFFilePath = directoryPath + $"/depot_{appID}.vdf";
            // Testing if the file already exists
            if (!File.Exists(depotVDFFilePath))
            {
                string depotVDFText = System.IO.File.ReadAllText(Application.dataPath + sampleDepotVDFPath);
                dynamic depotVdfObject = VdfConvert.Deserialize(depotVDFText);
                depotVdfObject.Value.DepotID = depotID.ToString();

                System.IO.File.WriteAllText(depotVDFFilePath, depotVdfObject.ToString());
            }
            else
            {
                Debug.Log($"This vdf file already exists {depotVDFFilePath}");
            }


            //Creating default app_appid.vdf file with information about the build
            string appVDFFilePath = directoryPath + $"/app_{appID}.vdf";
            if (!File.Exists(appVDFFilePath))
            {
                string appVDFText = System.IO.File.ReadAllText(Application.dataPath + sampleAppVDFPath);
                dynamic appVdfObject = VdfConvert.Deserialize(appVDFText);
                appVdfObject.Value.appid = appID.ToString();
                appVdfObject.Value.buildoutput = directoryPath;
                VObject depots = new VObject
            {
                new VProperty(depotID.ToString(), new VValue(depotVDFFilePath))
            };
                appVdfObject.Value.depots = depots;

                System.IO.File.WriteAllText(appVDFFilePath, appVdfObject.ToString());
            }
            else
            {
                Debug.Log($"This vdf file already exists {appVDFFilePath}");
            }


            // Testing if something went wrong
            if (!File.Exists(depotVDFFilePath) || !File.Exists(appVDFFilePath))
            {
                Debug.LogError("Something went worng when creating the vdf files needed");
            }

            // Creating the Scriptable Objects that hold information about vdfs and stuff
            ScriptableObject newGameSettingsSO = CreateInstance<BravoSteamUploadGameSettings>() as ScriptableObject;
            BravoSteamUploadGameSettings gameSettings = newGameSettingsSO as BravoSteamUploadGameSettings;
            gameSettings.AppID = appID;
            gameSettings.DepotID = depotID;
            gameSettings.AppVDFPath = appVDFFilePath;
            gameSettings.DepotVDFPath = depotVDFFilePath;
            string scriptableObjectPath = "Assets" + rootVDFFolder + $"/{appID}" + $"/{appID}_SteamGameSettings.asset";

            scriptableObjectPath = AssetDatabase.GenerateUniqueAssetPath(scriptableObjectPath);

            AssetDatabase.CreateAsset(newGameSettingsSO, scriptableObjectPath);
            AssetDatabase.Refresh();
            Selection.activeObject = newGameSettingsSO;

            Debug.Log($"Created Steam Game Upload Settings (and VDFs) \n@{scriptableObjectPath}");

            BravoSteamUpload window = EditorWindow.GetWindow<BravoSteamUpload>();
            window.RefreshGameSettingsArray();
            window.Repaint();
        }
    }
}

#endif