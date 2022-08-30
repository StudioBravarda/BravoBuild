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
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace BravoBuild.Source
{
    public class BravoSteamUpload : OdinEditorWindow
    {
        [TitleGroup("Steam Upload Settings")]
        [ShowIf("@steamUploadSettings != null")]
        [InfoBox("There should be only ONE SteamUploadSettings in the project", InfoMessageType.Error, "_steamUploadSettingsWarning")]
        [InfoBox("Please complete the fields in the settings to continue", InfoMessageType.Error, "@!steamUploadSettings.CheckComplete()")]
        [HideLabel]
        public BravoSteamUploadSettings steamUploadSettings;
        private bool _steamUploadSettingsWarning = false;

        [TitleGroup("Steam Game Settings"), ListDrawerSettings(HideAddButton = true, IsReadOnly = true)]
        public BravoSteamUploadGameSettings[] bravoSteamUploadGameSettingsArray;
        [TitleGroup("Steam Game Settings")]
        private int _steamGameSettingsQtd
        {
            get
            {
                if (bravoSteamUploadGameSettingsArray != null)
                {
                    return bravoSteamUploadGameSettingsArray.Length;
                }
                else
                {
                    return 0;
                }
            }
        }

        private static string rootFolderPath;
        private static int bravoSteamUploadSettingsInProject;

        [MenuItem("Upload/Steam")]
        public static void OpenWindow()
        {
            BravoSteamUpload window = GetWindow<BravoSteamUpload>();
            //TODO There must be a better way of getting the path from this script
            // string guid = AssetDatabase.FindAssets("BravoSteamUpload")[0];
            // string path = AssetDatabase.GUIDToAssetPath(guid);   
            rootFolderPath = Application.dataPath + "/Plugins/BravoBuild";

            window.Show();
            //
            string[] uploadsettings = AssetDatabase.FindAssets("t:BravoSteamUploadSettings");
            bravoSteamUploadSettingsInProject = uploadsettings.Length;
            if (bravoSteamUploadSettingsInProject > 0)
            {
                string firstSettingsPath = AssetDatabase.GUIDToAssetPath(uploadsettings[0]);
                window.steamUploadSettings = AssetDatabase.LoadAssetAtPath<BravoSteamUploadSettings>(firstSettingsPath);
            }
            window._steamUploadSettingsWarning = bravoSteamUploadSettingsInProject > 1;
            ///////
            window.RefreshGameSettingsArray();
        }

        [TitleGroup("Steam Upload Settings")]
        [InfoBox("Create a Steam Upload Settings to use this window", InfoMessageType.Error, "@bravoSteamUploadSettingsInProject == 0")]
        [Button, ShowIf("@steamUploadSettings == null")]
        public static void BravoSteamUploadSettings()
        {
            ScriptableObject newSettings = CreateInstance<BravoSteamUploadSettings>() as ScriptableObject;


            string targetFolder = "Assets/Plugins/BravoBuild/SteamSettings/";
            string dest = targetFolder + "BravoSteamUploadSettings.asset";
            dest = AssetDatabase.GenerateUniqueAssetPath(dest);


            AssetDatabase.CreateAsset(newSettings, dest);
            AssetDatabase.Refresh();
            Selection.activeObject = newSettings;

            Debug.Log($"Created Steam Upload Settings");


            BravoSteamUpload window = GetWindow<BravoSteamUpload>();
            window.steamUploadSettings = AssetDatabase.LoadAssetAtPath<BravoSteamUploadSettings>(dest);
            window.Repaint();
        }

        [InfoBox("VDF will be created. VDF files are files used by steam to hold the data necessary for uploading builds")]
        [Button]
        public void CreateGameSettings(int appId, int depotID)
        {
            BravoSteamUploadGameSettings.CreateNewGameSettings(appId, depotID);
        }

        [Button()]
        [ShowIf("ShowUploadButton")]
        public void Upload()
        {
            foreach (var gameSettings in bravoSteamUploadGameSettingsArray)
            {
                if (gameSettings.Enabled)
                {
                    UploadToSteam(steamUploadSettings, gameSettings);
                }
            }
        }

        private void UploadToSteam(BravoSteamUploadSettings steamSettings, BravoSteamUploadGameSettings gameSettings)
        {
            gameSettings.UpdateVDFs();
            string strCmdText = $"+login {steamSettings.SteamLogin} {steamSettings.SteamPassword} +run_app_build {gameSettings.AppVDFPath}";
            string steamCmdPath = steamSettings.GetSteamCMDPath();

            if (!File.Exists(steamCmdPath))
            {
                Debug.LogError("SteamCMD File does not exist");
            }

            Process process = new Process();
            process.StartInfo.FileName = steamSettings.GetSteamCMDPath();
            process.StartInfo.Arguments = strCmdText;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError = false;
            process.EnableRaisingEvents = false;
            process.Start();
            process.WaitForExit();
            //
            OnSteamFinishUpload();
        }

        private void OnSteamFinishUpload()
        {
            Debug.Log("Steam Finished Uploaded");
        }

        public void RefreshGameSettingsArray()
        {
            string[] gameSettings = AssetDatabase.FindAssets("t:BravoSteamUploadGameSettings");
            if (bravoSteamUploadSettingsInProject > 0)
            {
                if (bravoSteamUploadGameSettingsArray == null)
                {
                    bravoSteamUploadGameSettingsArray = new BravoSteamUploadGameSettings[gameSettings.Length];
                }
                for (int i = 0; i < gameSettings.Length; i++)
                {
                    bravoSteamUploadGameSettingsArray[i] = AssetDatabase.LoadAssetAtPath<BravoSteamUploadGameSettings>(AssetDatabase.GUIDToAssetPath(gameSettings[i]));
                }
            }
        }

        private bool ShowUploadButton()
        {
            foreach (var bravoGameSettings in bravoSteamUploadGameSettingsArray)
            {
                if (bravoGameSettings.Enabled) return true;
            }
            return false;
        }
    }
}

#endif