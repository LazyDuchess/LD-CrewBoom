using UnityEditor;
using UnityEngine;

public class DefaultAudioImportSettings : AssetPostprocessor
{
    // Optimized default settings for character audio clips. Makes loading very fast.
    void OnPreprocessAudio()
    {
        AudioImporter audioImporter = (AudioImporter)assetImporter;

        audioImporter.loadInBackground = true;
        audioImporter.preloadAudioData = false;

        var settings = audioImporter.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.Streaming;
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = 1f;

        audioImporter.defaultSampleSettings = settings;
    }
}
