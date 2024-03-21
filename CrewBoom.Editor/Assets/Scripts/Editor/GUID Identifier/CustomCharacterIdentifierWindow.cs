using UnityEditor;
using UnityEngine;

public class CustomCharacterIdentifierWindow : EditorWindow
{
    private string _guid;
    private string _name;
    private GUIStyle _propertyWindowStyle;

    public static void Show(string guid, string characterName)
    {
        CustomCharacterIdentifierWindow window = CreateWindow<CustomCharacterIdentifierWindow>("GUID Identifier");

        window._guid = guid;
        window._name = characterName;
        window.InitializeStyle();

        window.maxSize = new Vector2(350.0f, 115.0f);
        window.minSize = window.maxSize;

        window.Show();
    }

    private void InitializeStyle()
    {
        _propertyWindowStyle = new GUIStyle("GroupBox");
        _propertyWindowStyle.fontStyle = FontStyle.Bold;
        _propertyWindowStyle.fontSize = 14;
        _propertyWindowStyle.alignment = TextAnchor.UpperCenter;
        Vector2 offset = _propertyWindowStyle.contentOffset;
        offset.y = -16f;
        _propertyWindowStyle.contentOffset = offset;
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("Character ID", _propertyWindowStyle);
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField($"For character '{_name}'", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.TextField(_guid);

            EditorGUILayout.Space();

            if (GUILayout.Button("OK"))
            {
                Close();
            }
        }
        GUILayout.EndVertical();
    }
}
