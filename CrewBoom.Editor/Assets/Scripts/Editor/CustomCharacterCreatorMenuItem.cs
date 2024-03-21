
using CrewBoomMono;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CustomCharacterCreatorMenuItem
{
    private const string MATERIAL_FILTER = "t:Material";

    private static readonly string[] GraffitiNamePossibilities =
    {
        "graffiti",
        "graff",
        "tag"
    };

    [MenuItem("Assets/Crew Boom/Create character from model", priority = 10)]
    public static void CreateCustomCharacterFromAsset()
    {
        Object model = Selection.activeObject;
        string modelFilePath = AssetDatabase.GetAssetPath(model);
        string modelFileName = Path.GetFileName(modelFilePath);
        string modelDirectory = modelFilePath.Remove(modelFilePath.Length - modelFileName.Length);

        GameObject prefab = CreateOrFetchPrefab(model, Path.Combine(modelDirectory, $"{model.name}_character.prefab"));
        CharacterDefinition characterDefinition = prefab.AddComponent<CharacterDefinition>();
        characterDefinition.CharacterName = model.name;
        CreateOrFetchOutfitsAndGraffiti(modelDirectory, ref characterDefinition);

        AssetDatabase.SaveAssets();

        PrefabStage stage = PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(prefab));
        if (stage)
        {
            Selection.activeObject = stage.prefabContentsRoot;
        }
    }
    [MenuItem("Assets/Crew Boom/Create character from model", true)]
    public static bool ValidateCreateCustomCharacter()
    {
        return Selection.activeObject != null &&
            PrefabUtility.GetPrefabAssetType(Selection.activeObject) == PrefabAssetType.Model;
    }

    private static GameObject CreateOrFetchPrefab(Object model, string prefabPath)
    {
        GameObject prefab;
        if (File.Exists(prefabPath))
        {
            prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
        }
        else
        {
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(model) as GameObject;
            prefab = PrefabUtility.SaveAsPrefabAsset(newPrefab, prefabPath);
            AssetDatabase.SaveAssets();

            Object.DestroyImmediate(newPrefab);
        }

        return prefab;
    }

    private static void CreateOrFetchOutfitsAndGraffiti(string path, ref CharacterDefinition characterDefinition)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string[] assetIds = AssetDatabase.FindAssets(MATERIAL_FILTER, new string[] { path });
        foreach (string assetId in assetIds)
        {
            Material material = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetId), typeof(Material)) as Material;

            foreach (string possibility in GraffitiNamePossibilities)
            {
                if (material.name.ToLower().Contains(possibility))
                {
                    characterDefinition.Graffiti = material;
                    characterDefinition.GraffitiName = material.name;
                    return;
                }
            }
        }
    }
}
