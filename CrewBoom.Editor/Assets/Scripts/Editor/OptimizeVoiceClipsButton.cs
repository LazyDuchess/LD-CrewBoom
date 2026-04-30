using CrewBoomMono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class OptimizeVoiceClipsButton
{
    [MenuItem("CrewBoom/Optimize Voice Clips", false, -90)]
    private static void OptimizeVoiceClips()
    {
        if (EditorUtility.DisplayDialog("Optimize Voice Clips", "Are you sure? This will replace the import settings for all the voice clips in the current character to optimized ones.", "Yes", "No"))
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            DoOptimizeVoiceClips();
        }
    }

    [MenuItem("CrewBoom/Optimize Voice Clips", true, -90)]
    private static bool OptimizeVoiceClipsValidate()
    {
        var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (currentPrefabStage == null) return false;
        if (currentPrefabStage.prefabContentsRoot.GetComponent<CharacterDefinition>() != null) return true;
        return false;
    }

    private static void DoOptimizeVoiceClips()
    {
        var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        var definition = currentPrefabStage.prefabContentsRoot.GetComponent<CharacterDefinition>();

        var voices = new List<AudioClip>();

        if (definition.VoiceBoostTrick != null)
            voices.AddRange(definition.VoiceBoostTrick);

        if (definition.VoiceCombo != null)
            voices.AddRange(definition.VoiceCombo);

        if (definition.VoiceDie != null)
            voices.AddRange(definition.VoiceDie);

        if (definition.VoiceDieFall != null)
            voices.AddRange(definition.VoiceDieFall);

        if (definition.VoiceGetHit != null)
            voices.AddRange(definition.VoiceGetHit);

        if (definition.VoiceJump != null)
            voices.AddRange(definition.VoiceJump);

        if (definition.VoiceTalk != null)
            voices.AddRange(definition.VoiceTalk);

        foreach(var voice in voices)
        {
            var assetPath = AssetDatabase.GetAssetPath(voice);
            var audioImporter = (AudioImporter)AssetImporter.GetAtPath(assetPath);

            audioImporter.loadInBackground = true;
            audioImporter.preloadAudioData = false;

            var settings = audioImporter.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.Streaming;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = 1f;

            audioImporter.defaultSampleSettings = settings;

            audioImporter.SaveAndReimport();
        }

        Debug.Log($"Optimized {voices.Count} AudioClip assets.");
    }
}