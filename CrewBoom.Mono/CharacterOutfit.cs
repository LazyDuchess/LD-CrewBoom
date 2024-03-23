using UnityEngine;

namespace CrewBoomMono
{
    public class CharacterOutfit : MonoBehaviour
    {
        public string Name;
        public bool[] EnabledRenderers;
        public CharacterOutfitRenderer[] MaterialContainers;
    }
    public class CharacterOutfitRenderer : MonoBehaviour
    {
        public Material[] Materials;
        public bool[] UseShaderForMaterial;
    }
}