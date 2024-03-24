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

[InitializeOnLoad]
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
    public enum UpdateStatuses
    {
        NotChecked,
        Error,
        UpdateAvailable,
        UpToDate,
        Checking
    }
    public static UpdateStatuses UpdateStatus = UpdateStatuses.NotChecked;
    public static GithubRelease UpdateRelease;
    private const string VersionRequestURL = "https://api.github.com/repos/LazyDuchess/LD-CrewBoom/releases/latest";
    private const string DownloadURL = "https://github.com/LazyDuchess/LD-CrewBoom/releases/download/{0}/{1}";
    private const string TempDirectory = "UpdateTemp";
    private const string AssetFoldersWarning = @"Personal assets not in the following folders may be deleted:

Assets/Characters
Assets/User";

    static UpdateUtility()
    {
        if (!Preferences.AutoUpdate)
            return;
        EditorCoroutineUtility.StartCoroutineOwnerless(UpdateCrewBoom(true));
    }

    public static IEnumerator FetchLatestUpdate()
    {
        UpdateStatus = UpdateStatuses.Checking;
        try
        {
            var request = UnityWebRequest.Get(VersionRequestURL);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                var body = request.downloadHandler.text;
                var release = JsonUtility.FromJson<GithubRelease>(body);
                var latestVersion = release.tag_name;
                var comparison = CompareVersionToCurrent(latestVersion);
                UpdateRelease = release;
                if (comparison == VersionCompareResult.Higher)
                    UpdateStatus = UpdateStatuses.UpdateAvailable;
                else
                    UpdateStatus = UpdateStatuses.UpToDate;
            }
            else
                UpdateStatus = UpdateStatuses.Error;
        }
        finally
        {
            if (UpdateStatus == UpdateStatuses.Checking)
                UpdateStatus = UpdateStatuses.Error;
        }
    }

    // TODO: Clean up, this code is a mess cause I thought you couldn't call coroutines inside coroutines 💀.
    public static IEnumerator UpdateCrewBoom(bool autoUpdate)
    {
        if (Busy)
            yield break;
        Busy = true;
        try
        {
            yield return FetchLatestUpdate();
            switch (UpdateStatus)
            {
                case UpdateStatuses.NotChecked:
                    yield break;

                case UpdateStatuses.Checking:
                    yield break;

                case UpdateStatuses.Error:
                    if (autoUpdate)
                        yield break;
                    EditorUtility.DisplayDialog("Update CrewBoom", "Failed to fetch latest version.", "OK");
                    yield break;

                case UpdateStatuses.UpdateAvailable:
                    bool updateConfirmation;
                    if (autoUpdate)
                        updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"There is a new version of LD CrewBoom available ({UpdateRelease.tag_name}) Would you like to update?\n\n{AssetFoldersWarning}", "Yes", "No");
                    else
                        updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"Update to {UpdateRelease.tag_name}?\n\n{AssetFoldersWarning}", "Yes", "Cancel");
                    if (!updateConfirmation)
                    {
                        if (autoUpdate)
                            Preferences.AutoUpdate = false;
                        yield break;
                    }
                    break;

                case UpdateStatuses.UpToDate:
                    if (autoUpdate)
                        yield break;
                    var repairConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"You are already on the newest version. Repair?\n\n{AssetFoldersWarning}", "Yes", "Cancel");
                    if (!repairConfirmation)
                        yield break;
                    break;
            }
            if (UpdateStatus == UpdateStatuses.Error)
            {
                if (autoUpdate)
                    yield break;
                EditorUtility.DisplayDialog("Update CrewBoom", "Failed to fetch latest version.", "OK");
                yield break;
            }
              
            GithubAsset zipAsset = null;
            foreach(var asset in UpdateRelease.assets)
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
            var zipUrl = string.Format(DownloadURL, UpdateRelease.tag_name, zipAsset.name);
            var request = UnityWebRequest.Get(zipUrl);
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
            }
            else
            {
                EditorUtility.DisplayDialog("Update CrewBoom", "Failed to download latest version.", "OK");
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
        Log("CrewBoom Update applied!");
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