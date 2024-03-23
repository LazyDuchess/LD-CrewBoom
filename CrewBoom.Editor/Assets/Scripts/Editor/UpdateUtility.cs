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
    public static IEnumerator UpdateCrewBoom()
    {
        if (Busy)
            yield break;
        Busy = true;
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
                if (comparison == VersionCompareResult.Higher)
                {
                    var updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"Update to {latestVersion}? Any assets not in the Characters folder might be deleted!", "Yes", "Cancel");
                    if (!updateConfirmation)
                        yield break;
                }
                else
                {
                    var updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", $"You are already on the newest version. Repair? Any assets not in the Characters folder might be deleted!", "Yes", "Cancel");
                    if (!updateConfirmation)
                        yield break;
                }
                UpdateToZip(string.Format(DownloadURL, release.tag_name, release.assets[0].name));
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

    private static IEnumerator UpdateToZip(string zipUrl)
    {
        var request = UnityWebRequest.Get(zipUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var body = request.downloadHandler.data;
            var stream = new MemoryStream(body);
            var zip = new ZipArchive(stream);
        }
        else
        {
            EditorUtility.DisplayDialog("Update CrewBoom", "Failed to download latest version.", "OK");
        }
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