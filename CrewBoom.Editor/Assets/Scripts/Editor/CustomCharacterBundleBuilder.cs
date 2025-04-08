using CrewBoomMono;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class CustomCharacterBundleBuilder
{
    public const string BUNDLE_OUTPUT_FOLDER = "CharacterBundles";

    public static string GetBundleFilename(CharacterDefinition character)
    {
        if (character.OverrideBundleFilename && !string.IsNullOrWhiteSpace(character.BundleFilename))
            return SanitizeFilename(character.BundleFilename);
        var authorName = Preferences.AuthorName;
        if (string.IsNullOrWhiteSpace(authorName))
            return SanitizeFilename(character.CharacterName);
        return SanitizeFilename($"{authorName}.{character.CharacterName}");
    }

    private static string SanitizeFilename(string filename)
    {
        var invalidFilenameChars = Path.GetInvalidFileNameChars();
        foreach(var invalidChar in invalidFilenameChars)
        {
            filename = filename.Replace(invalidChar.ToString(), "");
        }
        return filename;
    }

    public static string GetFinalBundleFilename(CharacterDefinition character)
    {
        return $"{GetBundleFilename(character)}.cbb";
    }

    private static void ValidateGameShaders(CharacterDefinition definition)
    {
        var outfitRenderers = definition.GetComponentsInChildren<CharacterOutfitRenderer>();
        foreach(var outfitRenderer in outfitRenderers)
        {
            if (outfitRenderer == null) continue;
            if (outfitRenderer.Materials == null) continue;
            for(var i = 0; i < outfitRenderer.UseShaderForMaterial.Length; i++)
            {
                var mat = outfitRenderer.Materials[i];
                outfitRenderer.UseShaderForMaterial[i] = false;
                if (mat == null) continue;
                if (mat.shader == null) continue;
                outfitRenderer.UseShaderForMaterial[i] = ShaderUtility.IsGameShader(outfitRenderer.Materials[i].shader);
                EditorUtility.SetDirty(outfitRenderer);
            }
        }
    }

    public static void BuildBundle(GameObject prefab)
    {
        if (!EditorUtility.IsPersistent(prefab))
        {
            EditorUtility.DisplayDialog("Custom Character Bundle Builder", "A custom character can only be built from the prefab it originates from, but the prefab was not on disk.", "OK");
            return;
        }

        CharacterDefinition definition = prefab.GetComponent<CharacterDefinition>();
        if (definition == null)
        {
            EditorUtility.DisplayDialog("Custom Character Bundle Builder", $"{AssetDatabase.GetAssetPath(prefab)} is not a CharacterDefinition prefab.\nTry re-opening your character prefab to re-calibrate.", "OK");
            return;
        }

        var id = definition.Id;

        if (string.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString();

        definition.Id = id;
        EditorUtility.SetDirty(definition);
        ValidateGameShaders(definition);
        AssetDatabase.SaveAssets();

        List<AssetBundleBuild> assetBundleDefinitionList = new();

        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = GetFinalBundleFilename(definition);

        string pathToAsset = AssetDatabase.GetAssetPath(prefab);
        string fileName = Path.GetFileName(pathToAsset);
        string folderPath = pathToAsset.Remove(pathToAsset.Length - fileName.Length);
        if (RecursiveGetAllAssetsInDirectory(folderPath, definition, out List<string> assets, out List<string> characterFiles))
        {
            build.assetNames = assets.ToArray();
        }
        else
        {
            StringBuilder multiplePrefabsLog = new StringBuilder();
            multiplePrefabsLog.AppendLine("There were multiple prefabs found in the directory (or sub-directories) of the character you are trying to build.");
            multiplePrefabsLog.AppendLine("Make sure you put each character in their own directory with only the assets for that character in it.");
            multiplePrefabsLog.AppendLine();
            multiplePrefabsLog.AppendLine("The offending prefabs were:");
            foreach (string file in characterFiles)
            {
                multiplePrefabsLog.AppendLine(Path.GetFileNameWithoutExtension(file));
            }

            EditorUtility.DisplayDialog("Custom Character Bundle Builder", multiplePrefabsLog.ToString(), "OK");
            return;
        }

        assetBundleDefinitionList.Add(build);

        if (!Directory.Exists(BUNDLE_OUTPUT_FOLDER))
        {
            Directory.CreateDirectory(BUNDLE_OUTPUT_FOLDER);
        }
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(BUNDLE_OUTPUT_FOLDER, assetBundleDefinitionList.ToArray(), BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);

        if (manifest != null)
        {
            foreach (var bundleName in manifest.GetAllAssetBundles())
            {
                string projectRelativePath = Path.Combine(BUNDLE_OUTPUT_FOLDER, bundleName);
                string fileWithoutExtension = Path.GetFileNameWithoutExtension(projectRelativePath);
                string pathWithoutExtension = Path.Combine(BUNDLE_OUTPUT_FOLDER, fileWithoutExtension);
                string bundleManifestPath = projectRelativePath + ".manifest";
                string jsonPath = pathWithoutExtension + ".json";
                string txtPath = pathWithoutExtension + ".txt";
                if (File.Exists(bundleManifestPath))
                {
                    File.Delete(bundleManifestPath);
                }

                CharacterConfig config = new CharacterConfig()
                {
                    CharacterToReplace = nameof(BrcCharacter.None)
                };
                File.WriteAllText(jsonPath, JsonUtility.ToJson(config, true));
                File.WriteAllText(txtPath, id);

                Debug.Log($"Size of AssetBundle {projectRelativePath} is {new FileInfo(projectRelativePath).Length * 0.0009765625} KB");

                Debug.Log($"Built character bundle for {definition.CharacterName} with GUID {id}");
                if (Preferences.OpenFileExplorerOnBuild)
                    EditorUtility.RevealInFinder(projectRelativePath);

                if (Preferences.CopyBundles)
                {
                    var targetDirectory = Preferences.TargetBundlePath;
                    var targetBundle = Path.Combine(targetDirectory, Path.GetFileName(projectRelativePath));
                    if (Directory.Exists(targetDirectory) && !string.IsNullOrWhiteSpace(targetDirectory))
                    {
                        try
                        {
                            File.Copy(projectRelativePath, targetBundle, true);
                            File.Copy(jsonPath, Path.Combine(targetDirectory, Path.GetFileName(jsonPath)), true);
                            File.Copy(txtPath, Path.Combine(targetDirectory, Path.GetFileName(txtPath)), true);
                            if (Preferences.OpenFileExplorerOnBuild)
                                EditorUtility.RevealInFinder(targetBundle);
                        }
                        catch (IOException e)
                        {
                            Debug.LogWarning($"Failed to copy character bundle to {targetDirectory}!{Environment.NewLine}{e}");
                        }
                    }
                }
            }

            string manifestPath = Path.Combine(BUNDLE_OUTPUT_FOLDER, BUNDLE_OUTPUT_FOLDER);
            if (File.Exists(manifestPath))
            {
                File.Delete(manifestPath);
                File.Delete(manifestPath + ".manifest");
            }
        }
        else
        {
            Debug.Log("Build failed, see Console and Editor log for details");
        }
    }

    public static bool RecursiveGetAllAssetsInDirectory(string path, CharacterDefinition definition, out List<string> assets, out List<string> characterFiles)
    {
        bool onlyBuiltCharacter = true;
        characterFiles = new();

        assets = new();
        foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(file) != ".meta" &&
                Path.GetExtension(file) != ".cs" &&  // Scripts are not supported in AssetBundles
                Path.GetExtension(file) != ".unity") // Scenes cannot be mixed with other file types in a bundle
            {
                if (Path.GetExtension(file) == ".prefab")
                {
                    CharacterDefinition otherDefinition = AssetDatabase.LoadAssetAtPath<GameObject>(file).GetComponent<CharacterDefinition>();
                    if (otherDefinition != null && otherDefinition != definition)
                    {
                        onlyBuiltCharacter = false;
                        characterFiles.Add(file);
                    }
                }
                assets.Add(file);
            }
        }

        return onlyBuiltCharacter;
    }
}