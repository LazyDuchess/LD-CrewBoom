using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CharacterPreviewUtility
{
    private const string PreviewScenePath = "Assets/Scenes/Preview Scene.unity";

    public static void PreviewCharacter(GameObject character)
    {
        if (EditorApplication.isPlaying) return;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        var previewScene = EditorSceneManager.GetSceneByPath(PreviewScenePath);
        if (previewScene == null)
            previewScene = EditorSceneManager.OpenScene(PreviewScenePath, OpenSceneMode.Single);
        EditorSceneManager.SetActiveScene(previewScene);
        EditorApplication.EnterPlaymode();
        GameObject.FindObjectOfType<CharacterPreviewController>().CharacterPrefab = character;
        PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(character));
    }
}
