using UnityEngine;

namespace CrewBoomMono
{
    [AddComponentMenu("Crew Boom/Character Definition")]
    public class CharacterDefinition : MonoBehaviour
    {
        public string CharacterName = "New Custom Character";
        public BrcCharacter FreestyleAnimation = BrcCharacter.Red;
        public BrcCharacter BounceAnimation = BrcCharacter.Red;
        public BrcMovestyle DefaultMovestyle = BrcMovestyle.Skateboard;

        public SkinnedMeshRenderer[] Renderers;
        //This really sucks but I have to do it as Unity still doesn't support serialized types from a DLL
        //that isn't shipped with the game itself but loaded with Assembly.Load (which BepInEx probably does)
        //It also doesn't serialize multi-dimensional arrays so even that doesn't work
        public CharacterOutfit[] Outfits;

        public Material Graffiti;
        public string GraffitiName;
        public string GraffitiArtist;

        public AudioClip[] VoiceDie;
        public AudioClip[] VoiceDieFall;
        public AudioClip[] VoiceTalk;
        public AudioClip[] VoiceBoostTrick;
        public AudioClip[] VoiceCombo;
        public AudioClip[] VoiceGetHit;
        public AudioClip[] VoiceJump;

        public bool CanBlink;

        public string Id;
    }

    public class CharacterConfig
    {
        public string CharacterToReplace = "None";
    }
}