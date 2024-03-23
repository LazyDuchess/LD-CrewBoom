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
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
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
    }
}
