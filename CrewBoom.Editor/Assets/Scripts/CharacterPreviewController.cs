#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class PreviewDynamicBone
{
    private DynamicBone _dynamicBone;
    private Vector3 _force;

    public PreviewDynamicBone(DynamicBone dynamicBone)
    {
        _dynamicBone = dynamicBone;
        _force = dynamicBone.m_Force;
    }

    public void SetForce(Vector3 force)
    {
        _dynamicBone.m_Force = _force + force;
    }

    public static List<PreviewDynamicBone> GetFromParent(Transform tf)
    {
        var ls = new List<PreviewDynamicBone>();
        var dynaBones = tf.GetComponentsInChildren<DynamicBone>(true);
        foreach(var dynaBone in dynaBones)
            ls.Add(new PreviewDynamicBone(dynaBone));
        return ls;
    }
}

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
    private bool _paused = false;
    private float _timeScale = 1f;
    private List<PreviewDynamicBone> _dynamicBones;

    public static CharacterPreviewController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        var character = Instantiate(CharacterPrefab);
        Character = character.AddComponent<PreviewCharacter>();
        Character.Initialize(PreviewController);
        _dynamicBones = PreviewDynamicBone.GetFromParent(Character.transform);
    }
    
    public void SetPaused(Toggle toggle)
    {
        _paused = toggle.isOn;
        if (_paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = _timeScale;
    }

    public void PlaySoftBounceAnimation()
    {
        Character.PlaySoftBounceAnimation();
    }

    public void PlayFreestyleAnimation()
    {
        Character.PlayFreestyleAnimation();
    }

    public void PlayAnimation(string trigger)
    {
        Character.Animator.Play(trigger);
    }

    public void SetOutfit(int outfit)
    {
        Character.SetOutfit(outfit);
    }

    public void SetSpeed(Slider slider)
    {
        _timeScale = slider.value;
        if (_paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = slider.value;
    }

    public void SetMovementSpeed(Slider slider)
    {
        foreach(var dynamicBone in _dynamicBones)
        {
            dynamicBone.SetForce(new Vector3(0f, 0f, -slider.value));
        }
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

    public void Blink()
    {
        Character.StartCoroutine(Character.Blink());
    }
}
#endif