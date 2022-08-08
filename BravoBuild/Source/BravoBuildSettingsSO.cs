using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;


public class BravoBuildSettingsSO : SerializedScriptableObject
{
    [OnValueChanged("ChangeName")]
    public BuildTarget buildTarget;
    [OnValueChanged("ChangeName")]
    public BuildOptions buildOptions;
    public string buildNotes;

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

        BravoBuild.BuildGame(buildOptions, buildTarget, listOfScenes.ToArray());
    }
    
    private void ChangeName()
    {
        string assetPath = AssetDatabase.GetAssetPath(this.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, BravoBuild.ToFriendlyString(buildTarget) + "_" + BravoBuild.ToFriendlyString(buildOptions));
        AssetDatabase.SaveAssets();
    }

}

#endif