using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

[InitializeOnLoad]
public class ProjectSettingsFixUp
{
    static ProjectSettingsFixUp()
    {
        var audioPreset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/AudioImporter.preset");

        if (audioPreset == null) return;

        var type = audioPreset.GetPresetType();

        if (type.IsValidDefault())
        {
            var list = Preset.GetDefaultPresetsForType(type);

            if (list.Length == 0)
            {
                var ls = list.ToList();
                ls.Insert(0, new DefaultPreset(string.Empty, audioPreset));
                Preset.SetDefaultPresetsForType(type, ls.ToArray());
            }
        }
    }
}