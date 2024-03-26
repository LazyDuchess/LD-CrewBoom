using CrewBoomMono;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PreviewCharacter : MonoBehaviour
{
    public Animator Animator;
    private const float MaxBlinkTimer = 6f;
    private SkinnedMeshRenderer _blinkRenderer = null;
    private CharacterDefinition _definition;
    private float _blinkTimer = MaxBlinkTimer;

    public void Initialize(AnimatorController controller)
    {
        Animator = GetComponent<Animator>();
        Animator.runtimeAnimatorController = controller;
        _definition = GetComponent<CharacterDefinition>();
        SetOutfit(0);
    }

    public void SetTrigger(string trigger)
    {
        Animator.SetTrigger(trigger);
    }

    private void Update()
    {
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer <= 0f)
        {
            StartCoroutine(Blink());
            _blinkTimer = MaxBlinkTimer;
        }
    }

    private IEnumerator Blink()
    {
        if (_definition.CanBlink == false) yield break;
        if (_blinkRenderer.sharedMesh.blendShapeCount == 0) yield break;
        _blinkRenderer.SetBlendShapeWeight(0, 100f);
        yield return new WaitForSeconds(0.05f);
        _blinkRenderer.SetBlendShapeWeight(0, 0f);
    }

    public void SetOutfit(int outfitIndex)
    {
        _blinkRenderer = null;
        var outfit = _definition.Outfits[outfitIndex];
        for(var rendererIter = 0;rendererIter < _definition.Renderers.Length; rendererIter++)
        {
            var renderer = _definition.Renderers[rendererIter];
            var rendererEnabled = outfit.EnabledRenderers[rendererIter];
            renderer.gameObject.SetActive(rendererEnabled);
            if (rendererEnabled)
            {
                if (_blinkRenderer == null)
                    _blinkRenderer = renderer;
                renderer.sharedMaterials = outfit.MaterialContainers[rendererIter].Materials;
            }
        }
    }
}
