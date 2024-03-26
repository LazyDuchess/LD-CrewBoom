#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFromOutfitName : MonoBehaviour
{
    public int OutfitIndex = 0;
    private void Start()
    {
        GetComponent<Text>().text = CharacterPreviewController.Instance.Character.GetOutfitName(OutfitIndex);
    }
}
#endif