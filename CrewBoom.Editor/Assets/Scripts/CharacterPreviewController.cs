#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewController : MonoBehaviour
{
    public Transform PropHolder;
    public GameObject Phone;
    public GameObject SprayCan;
    public AnimatorController PreviewController;
    [HideInInspector]
    public GameObject CharacterPrefab = null;
    [HideInInspector]
    public PreviewCharacter Character = null;

    public static CharacterPreviewController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        var character = Instantiate(CharacterPrefab);
        Character = character.AddComponent<PreviewCharacter>();
        Character.Initialize(PreviewController);
    }

    public void TriggerAnimation(string trigger)
    {
        Character.SetTrigger(trigger);
    }

    public void SetOutfit(int outfit)
    {
        Character.SetOutfit(outfit);
    }

    public void SetSpeed(Slider slider)
    {
        Time.timeScale = slider.value;
    }

    public void SetSprayCan(Toggle toggle)
    {
        Character.SetSprayCan(toggle.isOn);
    }

    public void SetPhone(Toggle toggle)
    {
        Character.SetPhone(toggle.isOn);
    }

    public void ExitPreview()
    {
        UnityEditor.EditorApplication.ExitPlaymode();
    }
}
#endif