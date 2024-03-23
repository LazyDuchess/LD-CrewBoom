using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System;

public class UpdateUtility
{
    public static bool Busy = false;
    private enum VersionCompareResult
    {
        Higher,
        Equal,
        Lower,
        Error
    }
    private const string VersionRequestURL = "https://api.github.com/repos/LazyDuchess/LD-CrewBoom/releases/latest";
    private const string DownloadURL = "https://github.com/LazyDuchess/LD-CrewBoom/releases/download/{0}/{1}";
    private const string TempDirectory = "UpdateTemp";
    private const string AssetFoldersWarning = @"Personal assets not in the following folders may be deleted:

Assets/Characters
Assets/User";
    public static IEnumerator UpdateCrewBoom()
    {
        if (Busy)
            yield break;
        Busy = true;
        try
        {
            var request = UnityWebRequest.Get(VersionRequestURL);
            Log("Fetching latest version...");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var body = request.downloadHandler.text;
                var release = JsonUtility.FromJson<GithubRelease>(body);
                var latestVersion = release.tag_name;
                var comparison = CompareVersionToCurrent(latestVersion);
                if (comparison == VersionCompareResult.Higher)
                {
                    var updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"Update to {latestVersion}?\n\n{AssetFoldersWarning}", "Yes", "Cancel");
                    if (!updateConfirmation)
                        yield break;
                }
                else
                {
                    var updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"You are already on the newest version. Repair?\n\n{AssetFoldersWarning}", "Yes", "Cancel");
                    if (!updateConfirmation)
                        yield break;
                }
                GithubAsset zipAsset = null;
                foreach(var asset in release.assets)
                {
                    if (asset.name.ToLowerInvariant().StartsWith("crewboom.editor"))
                    {
                        zipAsset = asset;
                        break;
                    }    
                }
                if (zipAsset == null)
                {
                    EditorUtility.DisplayDialog("Update CrewBoom", "Failed to fetch zip file in latest release.", "OK");
                    yield break;
                }
                var zipUrl = string.Format(DownloadURL, release.tag_name, zipAsset.name);
                request = UnityWebRequest.Get(zipUrl);
                Log("Downloading latest version...");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var zipbody = request.downloadHandler.data;
                    var stream = new MemoryStream(zipbody);
                    var zip = new ZipArchive(stream);
                    if (Directory.Exists(TempDirectory))
                    {
                        Directory.Delete(TempDirectory, true);
                        Directory.CreateDirectory(TempDirectory);
                    }
                    zip.ExtractToDirectory(TempDirectory);
                    ApplyUpdate();
                    Log("Updated!");
                }
                else
                {
                    EditorUtility.DisplayDialog("Update CrewBoom", "Failed to download latest version.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Update CrewBoom", "Failed to fetch latest version.", "OK");
            }
        }
        finally
        {
            Busy = false;
        }
    }

    private static void Log(string text)
    {
        Debug.Log(text);
    }

    private static void ApplyUpdate()
    {
        var updateAssetsDirectory = Path.Combine(TempDirectory, "Assets");
        var updateCharactersDirectory = Path.Combine(updateAssetsDirectory, "Characters");
        var updateDynamicBonesDirectory = Path.Combine(updateAssetsDirectory, "Dynamic Bones");

        if (Directory.Exists(updateCharactersDirectory))
            Directory.Delete(updateCharactersDirectory, true);

        // In case people un-stub the scripts.
        var hasDynamicBones = false;
        var localAssetDirectories = Directory.GetDirectories("Assets");
        foreach(var localDirectory in localAssetDirectories)
        {
            var directoryName = Path.GetFileName(localDirectory);
            if (directoryName.ToLowerInvariant().Trim() == "characters")
                continue;
            if (directoryName.ToLowerInvariant().Trim() == "user")
                continue;
            if (directoryName.ToLowerInvariant().Trim() == "dynamic bones")
            {
                hasDynamicBones = true;
                continue;
            }
            Directory.Delete(localDirectory, true);
        }

        if (Directory.Exists(updateDynamicBonesDirectory) && hasDynamicBones)
            Directory.Delete(updateDynamicBonesDirectory, true);

        var updateAssetDirectories = Directory.GetDirectories(updateAssetsDirectory);
        foreach(var updateDirectory in updateAssetDirectories)
        {
            var directoryName = Path.GetFileName(updateDirectory);
            Directory.Move(updateDirectory, Path.Combine("Assets", directoryName));
        }
        Directory.Delete(TempDirectory, true);
        AssetDatabase.Refresh();
    }

    private static VersionCompareResult CompareVersionToCurrent(string version)
    {
        var compareVersion = GetVersionNumbers(version);
        var currentVersion = GetVersionNumbers(CrewBoomVersion.Version);
        if (compareVersion[0] > currentVersion[0])
            return VersionCompareResult.Higher;
        if (compareVersion[0] < currentVersion[0])
            return VersionCompareResult.Lower;
        if (compareVersion[0] == currentVersion[0])
        {
            if (compareVersion[1] > currentVersion[1])
                return VersionCompareResult.Higher;
            if (compareVersion[1] < currentVersion[1])
                return VersionCompareResult.Lower;
            if (compareVersion[1] == currentVersion[1])
            {
                if (compareVersion[2] > currentVersion[2])
                    return VersionCompareResult.Higher;
                if (compareVersion[2] < currentVersion[2])
                    return VersionCompareResult.Lower;
                if (compareVersion[2] == currentVersion[2])
                    return VersionCompareResult.Equal;
            }
        }
        return VersionCompareResult.Error;
    }

    private static int[] GetVersionNumbers(string version)
    {
        var versionArray = version.Split('.');
        var finalArray = new int[]{ int.Parse(versionArray[0]), int.Parse(versionArray[1]), int.Parse(versionArray[2]) };
        return finalArray;
    }
}