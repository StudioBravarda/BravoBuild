using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using LMS.Version;
#if UNITY_EDITOR
using UnityEditor;

namespace BravoBuild.Source
{

    public class BravoBuildSettingsSO : SerializedScriptableObject
    {
        [OnValueChanged("ChangeName")]
        public BuildTarget buildTarget;
        [OnValueChanged("ChangeName")]
        public BuildOptions buildOptions;
        public string buildNotes;
        [OnValueChanged("ChangeName")]
        public BuildType buildType;
        public List<SceneAsset> m_SceneAssets = new List<SceneAsset>();

        [Button]
        public void Build()
        {
            List<string> listOfScenes = new List<string>();
            foreach (SceneAsset scene in m_SceneAssets)
            {
                listOfScenes.Add(AssetDatabase.GetAssetOrScenePath(scene));
                Debug.Log(AssetDatabase.GetAssetOrScenePath(scene));
            }
            LMS.Version.Version.Instance.Initialize();
            BravoBuild.BuildGame(buildOptions, buildTarget, listOfScenes.ToArray(), buildType);
        }

        private void ChangeName()
        {
            string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
            AssetDatabase.RenameAsset(assetPath, BravoBuildUtils.ToFriendlyString(buildType) + "_" + BravoBuildUtils.ToFriendlyString(buildTarget) + "_" + BravoBuildUtils.ToFriendlyString(buildOptions));
            AssetDatabase.SaveAssets();
        }
    }
}

#endif