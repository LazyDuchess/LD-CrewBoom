using CrewBoomMono;
using UnityEditor;
using UnityEngine;

public class CustomCharacterIdentifierMenuItem
{
    [MenuItem("CrewBoom/Get GUID for character bundle", false, -90)]
    public static void GetBundleID()
    {
        string file = EditorUtility.OpenFilePanel("Select a character bundle", CustomCharacterBundleBuilder.BUNDLE_OUTPUT_FOLDER, "");

        AssetBundle bundle = AssetBundle.LoadFromFile(file);
        if (bundle == null)
        {
            EditorUtility.DisplayDialog("GUID Identifier", "Selected file was not an asset bundle.", "OK");
            return;
        }

        GameObject[] allObjects = bundle.LoadAllAssets<GameObject>();
        CharacterDefinition characterDefinition;
        foreach (GameObject obj in allObjects)
        {
            characterDefinition = obj.GetComponent<CharacterDefinition>();
            if (characterDefinition != null)
            {
                CustomCharacterIdentifierWindow.Show(characterDefinition.Id, characterDefinition.CharacterName);
                bundle.Unload(true);
                return;
            }
        }

        EditorUtility.DisplayDialog("GUID Identifier", "Selected asset bundle was not a character bundle.", "OK");
        bundle.Unload(true);
    }
}
