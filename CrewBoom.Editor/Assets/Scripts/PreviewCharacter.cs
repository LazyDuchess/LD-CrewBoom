#if UNITY_EDITOR
using CrewBoomMono;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using System.Linq;

public class PreviewCharacter : MonoBehaviour
{
    public Animator Animator;
    private const float MaxBlinkTimer = 6f;
    private SkinnedMeshRenderer _blinkRenderer = null;
    private CharacterDefinition _definition;
    private float _blinkTimer = MaxBlinkTimer;
    private CharacterPreviewController _previewController = null;

    public void Initialize(AnimatorController controller)
    {
        _previewController = CharacterPreviewController.Instance;
        Animator = GetComponent<Animator>();
        Animator.runtimeAnimatorController = controller;
        _definition = GetComponent<CharacterDefinition>();
        SetOutfit(0);
    }

    public void SetSprayCan(bool set)
    {
        if (set)
        {
            Animator.SetLayerWeight(2, 1f);
            _previewController.SprayCan.transform.SetParent(GetPropRTransform(), false);
        }
        else
        {
            Animator.SetLayerWeight(2, 0f);
            _previewController.SprayCan.transform.SetParent(_previewController.PropHolder, false);
        }
    }

    public void SetPhone(bool set)
    {
        if (set)
        {
            Animator.SetLayerWeight(1, 1f);
            _previewController.Phone.transform.SetParent(GetPropLTransform(), false);
        }
        else
        {
            Animator.SetLayerWeight(1, 0f);
            _previewController.Phone.transform.SetParent(_previewController.PropHolder, false);
        }
    }

    private Transform GetPropLTransform()
    {
        var skeletonRoot = transform.Find("root");
        var rightHandHumanBone = Animator.avatar.humanDescription.human.FirstOrDefault(x => x.humanName == "LeftHand");
        var rightHandBone = skeletonRoot.FindRecursive(rightHandHumanBone.boneName);
        var propRBone = rightHandBone.FindRecursive("propl");
        return propRBone;
    }

    private Transform GetPropRTransform()
    {
        var skeletonRoot = transform.Find("root");
        var rightHandHumanBone = Animator.avatar.humanDescription.human.FirstOrDefault(x => x.humanName == "RightHand");
        var rightHandBone = skeletonRoot.FindRecursive(rightHandHumanBone.boneName);
        var propRBone = rightHandBone.FindRecursive("propr");
        return propRBone;
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
        CloseEyes();
        yield return new WaitForSeconds(0.1f);
        OpenEyes();
    }

    private void CloseEyes()
    {
        if (_definition.CanBlink == false) return;
        if (_blinkRenderer.sharedMesh.blendShapeCount == 0) return;
        _blinkRenderer.SetBlendShapeWeight(0, 100f);
    }

    private void OpenEyes()
    {
        if (_definition.CanBlink == false) return;
        if (_blinkRenderer.sharedMesh.blendShapeCount == 0) return;
        _blinkRenderer.SetBlendShapeWeight(0, 0f);
    }

    public void SetOutfit(int outfitIndex)
    {
        StopAllCoroutines();
        if (_blinkRenderer != null)
            OpenEyes();
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

    public string GetOutfitName(int outfitIndex)
    {
        var name = _definition.Outfits[outfitIndex].Name;
        if (string.IsNullOrEmpty(name))
        {
            switch (outfitIndex)
            {
                case 0:
                    return "Spring";
                case 1:
                    return "Summer";
                case 2:
                    return "Autumn";
                case 3:
                    return "Winter";
            }
        }
        return name;
    }
}
#endif