using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Microsoft.Win32;

public class PreferencesWindow : EditorWindow
{
    [MenuItem("CrewBoom/Preferences")]
    public static void ShowWindow()
    {
        var window = GetWindow<PreferencesWindow>();
        window.titleContent = new GUIContent("CrewBoom Preferences");
        window.maxSize = new Vector2(1200.0f, 255.0f);
        window.minSize = new Vector2(600.0f, window.maxSize.y);
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUIUtility.labelWidth = 300;
        var header = new GUIStyle();
        header.fontSize = 16;
        header.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("General", header);
        EditorGUILayout.BeginVertical("box");
        var currentAuthor = Preferences.AuthorName;
        var author = EditorGUILayout.TextField("Author Name", currentAuthor);
        if (author != currentAuthor)
            Preferences.AuthorName = author;
        EditorGUILayout.HelpBox("Your author name will be prefixed to character bundle filenames.", MessageType.Info);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        var currentCopyBundles = Preferences.CopyBundles;
        var copyBundles = EditorGUILayout.Toggle("Copy Character Bundles on Build", currentCopyBundles);
        if (copyBundles != currentCopyBundles)
            Preferences.CopyBundles = copyBundles;
        GUI.enabled = copyBundles;
        EditorGUILayout.BeginHorizontal();
        var currentTargetBundlePath = Preferences.TargetBundlePath;
        var targetBundlePath = EditorGUILayout.TextField(currentTargetBundlePath);
        if (targetBundlePath != currentTargetBundlePath)
            Preferences.TargetBundlePath = targetBundlePath;
        if (GUILayout.Button("Browse"))
        {
            var folder = EditorUtility.OpenFolderPanel("Browse folder", Preferences.TargetBundlePath, "");
            if (!string.IsNullOrEmpty(folder))
                Preferences.TargetBundlePath = folder;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("If the directory exists, character bundles will be automatically copied to this location on build.", MessageType.Info);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        var currentOpenFileExplorer = Preferences.OpenFileExplorerOnBuild;
        var openFileExplorer = EditorGUILayout.Toggle("Open File Explorer on Build", currentOpenFileExplorer);
        if (openFileExplorer != currentOpenFileExplorer)
            Preferences.OpenFileExplorerOnBuild = openFileExplorer;
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField($"LD CrewBoom Version: {CrewBoomVersion.Version}");
        if (GUILayout.Button("Update"))
            UpdateUtility.UpdateCrewBoom();
    }
}
