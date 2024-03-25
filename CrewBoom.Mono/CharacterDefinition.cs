using UnityEngine;

namespace CrewBoomMono
{
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    [AddComponentMenu("Crew Boom/Character Definition")]
    public class CharacterDefinition : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool OverrideBundleFilename = false;
        public string BundleFilename = "";
        public static Mesh SprayCanMesh;
        public static Material SprayCanMaterial;
#endif
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

#if UNITY_EDITOR
        private void OnRenderObject()
        {
            var propR = FindRecursive(transform, "propr");
            if (propR != null && SprayCanMesh != null && SprayCanMaterial != null)
            {
                SprayCanMaterial.SetPass(0);
                Graphics.DrawMeshNow(SprayCanMesh, propR.localToWorldMatrix);
                SprayCanMaterial.SetPass(1);
                Graphics.DrawMeshNow(SprayCanMesh, propR.localToWorldMatrix);
            }
        }

        private static Transform FindRecursive(Transform transform, string name)
        {
            if (transform == null) return null;

            if (transform.name == name)
            {
                return transform;
            }

            Transform next = null;
            foreach (Transform child in transform)
            {
                next = FindRecursive(child, name);
                if (next)
                {
                    break;
                }
            }
            return next;
        }
#endif
    }

    public class CharacterConfig
    {
        public string CharacterToReplace = "None";
    }
}