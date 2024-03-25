using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Animations;
using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    public AnimatorController PreviewController;
    [HideInInspector]
    public GameObject CharacterPrefab = null;
    [HideInInspector]
    public GameObject Character = null;

    private Animator _animator = null;
    public static CharacterPreviewController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        Character = Instantiate(CharacterPrefab);
        InitializePreviewCharacter();
    }

    private void InitializePreviewCharacter()
    {
        _animator = Character.GetComponent<Animator>();
        _animator.runtimeAnimatorController = PreviewController;
    }

    public void TriggerAnimation(string trigger)
    {
        _animator.SetTrigger(trigger);
    }
}