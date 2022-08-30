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
using System.Collections.Generic;

namespace BravoBuild.Source
{
    [InlineEditor]
    [HideReferenceObjectPicker]
    public class BravoItchUpload : OdinEditorWindow
    {
        [TitleGroup("Itch Upload Settings")]
        public BravoItchSettings itchIOSettings;
        [TitleGroup("Itch Upload Settings"), ListDrawerSettings(HideAddButton = true, IsReadOnly = true)]
        public BravoItchUploadGameSettings[] itchioGameSettingsArray;

        private static int bravoItchSettingsInProject;
        private static int bravoItchGameSettingsInProject;

        [MenuItem("Upload/Itch.io")]
        public static void OpenWindow()
        {
            BravoItchUpload window = GetWindow<BravoItchUpload>();

            string[] uploadsettings = AssetDatabase.FindAssets("t:BravoItchSettings");

            bravoItchSettingsInProject = uploadsettings.Length;
            if (bravoItchSettingsInProject > 0)
            {
                string firstSettingsPath = AssetDatabase.GUIDToAssetPath(uploadsettings[0]);
                window.itchIOSettings = AssetDatabase.LoadAssetAtPath<BravoItchSettings>(firstSettingsPath);
            }
            if(bravoItchSettingsInProject > 1) Debug.LogError("There should be only one Itch Settings in the project");
            window.RefreshGameSettingsArray();
            window.Show();
        }

        [Button]
        public void Upload()
        {
            foreach (var gameSettings in itchioGameSettingsArray)
            {
                if (gameSettings.Enabled)
                {
                    ButlerPush(gameSettings);
                }
            }
        }

        
        public void ButlerPush(BravoItchUploadGameSettings settings)
        {
            int major = LMS.Version.Version.Instance.GameVersion.Major;
            int minor = LMS.Version.Version.Instance.GameVersion.Minor;
            int build = LMS.Version.Version.Instance.GameVersion.Build;
            
            BuildInformation buildInformation = BravoBuildLog.Instance.GetBuildWithVersion(settings.VersionToUpload);
            string strCmdText = $"/k echo \"{settings.GameName} TO ITCH.IO\" & cd /d \"{itchIOSettings.ButlerPath}\" & butler.exe push \"{buildInformation.GetFolderPath()}\" {settings.GetButtlerFullCommand()} --userversion {major}.{minor}.{build} & butler.exe status {settings.GetButtlerFullCommand()}";

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = strCmdText;
            process.Start();
            process.WaitForExit();
        }

        public void RefreshGameSettingsArray()
        {
            string[] gameSettings = AssetDatabase.FindAssets("t:BravoItchUploadGameSettings");
            bravoItchGameSettingsInProject = gameSettings.Length;
            if (bravoItchGameSettingsInProject > 0)
            {
                if (itchioGameSettingsArray == null)
                {
                    itchioGameSettingsArray = new BravoItchUploadGameSettings[gameSettings.Length];
                }
                for (int i = 0; i < gameSettings.Length; i++)
                {
                    itchioGameSettingsArray[i] = AssetDatabase.LoadAssetAtPath<BravoItchUploadGameSettings>(AssetDatabase.GUIDToAssetPath(gameSettings[i]));
                }
            }
        }
    }
}

#endif
