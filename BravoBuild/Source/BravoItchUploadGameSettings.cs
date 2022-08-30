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
    [InlineEditor]
    [HideReferenceObjectPicker]
    public class BravoItchUploadGameSettings : SerializedScriptableObject
    {
        [TitleGroup("Itch Settings")]
        [ToggleGroup("Itch Settings/Enabled", "$Label")]
        public bool Enabled = false;
        public string Label => GetButtlerFullCommand();
        [TitleGroup("Itch Settings")]
        public string User;
        [TitleGroup("Itch Settings")]
        public string GameName;
        [TitleGroup("Itch Settings")]
        public ItchIOPlatform Platform;
        [TitleGroup("Itch Settings")]
        [EnumToggleButtons()]
        public BuildType ItchIOBuildType;

        [TitleGroup("Itch Settings/Version To Upload")]
        // [GUIColor("SteamBuildTypeVersionBuildTypeCompare")]
        [HorizontalGroup("Itch Settings/Version To Upload/Version Info"), HideLabel]
        [ValueDropdown("@BravoBuildLog.Instance.GetListVersions()")] //TODO Check if this version has a folder okay
        public string VersionToUpload;
        [HorizontalGroup("Itch Settings/Version To Upload/Version Info"), ShowInInspector, ReadOnly]
        public string VersionBuildType
        {
            get
            {
                BuildInformation buildInfo = BravoBuildLog.Instance.GetBuildWithVersion(VersionToUpload);
                if (buildInfo != null)
                {
                    return BravoBuildUtils.ToFriendlyString(buildInfo.BuildType);
                }
                else
                {
                    return "-";
                }
            }
        }

        public string GetButtlerFullCommand() => $"{this.User}/{this.GameName}:{this.Platform}-{this.ItchIOBuildType}";
    }

    public enum ItchIOPlatform
    {
        win64,
        linux,
        mac,
        android
    }
}
#endif