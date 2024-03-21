using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CharacterRig
{
    private Avatar _avatar;

    //Root
    private Transform _root;

    //Humanoid bones
    private Transform _head, _chest, _spine, _leftHand, _rightHand, _leftFoot, _rightFoot, _leftLeg, _rightLeg;

    //Extras
    private Transform _boostPack, _boost, _propL, _propR;
    private Transform _bmxFrame, _bmxGear, _bmxHandlebars, _bmxWheelF, _bmxWheelR, _bmxPedalL, _bmxPedalR;
    private Transform _leftHandIK, _rightHandIK, _phoneDirectionRoot, _phoneDirection, _skateboard;

    public Transform LeftPropBone => _propL;
    public Transform RightPropBone => _propR;
    public Transform BoostBone => _boost;

    private static readonly Vector3 PROP_L_ANGLE = new Vector3(-47.5f, 60.0f, 25.0f);
    private static readonly Vector3 PROP_R_ANGLE = new Vector3(0.0f, 90.0f, 0.0f);
    private static readonly Vector3 BOOST_ANGLE = new Vector3(-90.0f, 0.0f, -90.0f);

    //Offsets
    public Transform _skateOffsetL, _skateOffsetR;

    public Transform LeftSkateBone => _skateOffsetL;
    public Transform RightSkateBone => _skateOffsetR;

    public bool PreviewProps { get; private set; } = true;
    public bool PreviewSkates { get; private set; } = true;
    public bool PreviewBoost { get; private set; }

    private GUIContent _missingBoneContent;
    private GUIContent _foundBoneContent;
    private GUIContent _unknownBoneContent;

    public void Setup(Avatar avatar)
    {
        _avatar = avatar;

        _missingBoneContent = EditorGUIUtility.IconContent("CollabConflict");
        _foundBoneContent = EditorGUIUtility.IconContent("CollabNew");
        _unknownBoneContent = EditorGUIUtility.IconContent("TestIgnored");
    }

    public bool Validate(Transform _prefabRoot, bool isEditable)
    {
        bool valid = true;

        if (_avatar == null || !_avatar.isValid)
        {
            EditorGUILayout.HelpBox("Avatar is missing or misconfigured.", MessageType.Error);
            return false;
        }
        if (!_avatar.isHuman)
        {
            EditorGUILayout.HelpBox("The avatar must be of type humanoid.", MessageType.Error);
            return false;
        }

        _root = _prefabRoot.Find("root");
        EditorGUILayout.ObjectField(BoneExistsLabel(_root, "Skeleton Root", "The transform where the skeleton starts\n(Usually the skeleton itself)"),
                                    _root,
                                    typeof(Transform),
                                    false);
        if (!_root)
        {
            EditorGUILayout.HelpBox("The skeleton root must be named 'root'", MessageType.None);
            valid = false;
        }

        EditorGUILayout.Space();

        if (!ValidateHumanoidRig(_root))
        {
            valid = false;
        }

        EditorGUILayout.Space();

        if (!ValidateExtraRig(_prefabRoot, _root))
        {
            valid = false;
            if (isEditable && _prefabRoot && _root)
            {
                FixExtraRig(_prefabRoot);
            }
        }

        return valid;
    }

    private GUIContent BoneExistsLabel(Transform bone, string label, string tooltip)
    {
        GUIContent content = bone ? _foundBoneContent : _missingBoneContent;
        content.text = ' ' + label;
        content.tooltip = tooltip;

        return content;
    }
    private GUIContent BoneExistsLabel(bool exists, string label, string tooltip)
    {
        GUIContent content = exists ? _foundBoneContent : _missingBoneContent;
        content.text = ' ' + label;
        content.tooltip = tooltip;

        return content;
    }
    private bool DrawMatchingBoneProperty(Transform bone, string targetName, string label, bool disabled = false)
    {
        bool matches = bone ? bone.name == targetName : false;
        GUIContent content = disabled ? _unknownBoneContent : (matches ? _foundBoneContent : _missingBoneContent);
        content.text = ' ' + label;
        content.tooltip = string.Empty;
        EditorGUILayout.ObjectField(content, bone, typeof(Transform), false);

        if (!matches && !disabled)
        {
            EditorGUILayout.HelpBox($"This bone must be (re)named '{targetName}'", MessageType.None);
        }

        return matches;
    }

    private void SetupHumanoidRig(Transform skeletonRoot)
    {
        HumanDescription avatarDescription = _avatar.humanDescription;
        foreach (var bone in avatarDescription.human)
        {
            switch (bone.humanName)
            {
                case "LeftHand":
                    _leftHand = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "RightHand":
                    _rightHand = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "LeftFoot":
                    _leftFoot = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "RightFoot":
                    _rightFoot = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "LeftLowerLeg":
                    _leftLeg = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "RightLowerLeg":
                    _rightLeg = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "Head":
                    _head = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "Chest":
                    _chest = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
                case "Spine":
                    _spine = skeletonRoot.FindRecursive(bone.boneName);
                    continue;
            }
        }
    }
    public bool ValidateHumanoidRig(Transform skeletonRoot)
    {
        bool hasRoot = skeletonRoot;
        if (!hasRoot)
        {
            GUI.enabled = false;
        }
        else
        {
            SetupHumanoidRig(skeletonRoot);
        }

        bool valid = true;

        EditorGUILayout.LabelField("Humanoid Bones", EditorStyles.boldLabel);

        if (!DrawMatchingBoneProperty(_head, "head", "Head", !hasRoot))
        {
            valid = false;
        }

        EditorGUILayout.Space();

        if (!DrawMatchingBoneProperty(_leftHand, "handl", "Left Hand", !hasRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_rightHand, "handr", "Right Hand", !hasRoot))
        {
            valid = false;
        }

        EditorGUILayout.Space();

        if (!DrawMatchingBoneProperty(_leftLeg, "leg2l", "Lower Left Leg", !hasRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_rightLeg, "leg2r", "Lower Right Leg", !hasRoot))
        {
            valid = false;
        }

        EditorGUILayout.Space();

        if (!DrawMatchingBoneProperty(_leftFoot, "footl", "Left Foot", !hasRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_rightFoot, "footr", "Right Foot", !hasRoot))
        {
            valid = false;
        }

        if (!hasRoot)
        {
            GUI.enabled = true;
        }

        return valid;
    }

    private void SetupExtraRig(Transform prefabRoot, Transform skeletonRoot)
    {
        _boostPack = skeletonRoot.FindRecursive("jetpack");
        _boost = _boostPack.FindRecursive("boost");
        _propL = _leftHand.FindRecursive("propl");
        _propR = _rightHand.FindRecursive("propr");

        _bmxFrame = prefabRoot.FindRecursive("bmxFrame");
        _bmxGear = _bmxFrame.FindRecursive("bmxGear");
        _bmxHandlebars = _bmxFrame.FindRecursive("bmxHandlebars");
        _bmxWheelF = _bmxHandlebars.FindRecursive("bmxWheelF");
        _bmxWheelR = _bmxFrame.FindRecursive("bmxWheelR");
        _bmxPedalL = _bmxGear.FindRecursive("bmxPedalL");
        _bmxPedalR = _bmxGear.FindRecursive("bmxPedalR");

        _leftHandIK = prefabRoot.FindRecursive("handlIK");
        _rightHandIK = prefabRoot.FindRecursive("handrIK");

        _phoneDirectionRoot = prefabRoot.FindRecursive("phoneDirectionRoot");
        _phoneDirection = _phoneDirectionRoot.FindRecursive("phoneDirection");

        _skateboard = prefabRoot.FindRecursive("skateboard");
    }
    public bool ValidateExtraRig(Transform prefabRoot, Transform skeletonRoot)
    {
        bool hasNoRoot = !prefabRoot || !skeletonRoot;
        if (hasNoRoot)
        {
            GUI.enabled = false;
        }
        else
        {
            SetupExtraRig(prefabRoot, skeletonRoot);
        }

        bool valid = true;

        EditorGUILayout.LabelField("Non-humanoid Bones", EditorStyles.boldLabel);

        PreviewProps = EditorGUILayout.Toggle("Preview Phone & Spraycan", PreviewProps);
        PreviewBoost = EditorGUILayout.Toggle("Preview Boost Effect", PreviewBoost);

        if (!DrawMatchingBoneProperty(_boostPack, "jetpack", "Jetpack", hasNoRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_boost, "boost", "Boost", hasNoRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_propL, "propl", "Left Prop", hasNoRoot))
        {
            valid = false;
        }
        if (!DrawMatchingBoneProperty(_propR, "propr", "Right Prop", hasNoRoot))
        {
            valid = false;
        }

        bool hasExtraBones = _bmxFrame && _bmxGear && _bmxHandlebars && _bmxWheelF && _bmxWheelR && _bmxPedalL && _bmxPedalR &&
                    _leftHandIK && _rightHandIK && _phoneDirectionRoot && _phoneDirection && _skateboard;
        GUIContent extraBonesContent = BoneExistsLabel(hasExtraBones,
                                                       "Extra Bones",
                                                       "Static bones that the game needs to animate properly.\nThey do not need to be created manually.");
        EditorGUILayout.LabelField(extraBonesContent);
        if (!hasExtraBones)
        {
            valid = false;
        }

        if (hasNoRoot)
        {
            GUI.enabled = true;
        }

        return valid;
    }
    public void FixExtraRig(Transform prefabRoot)
    {
        if (!_boostPack)
        {
            Transform boostPackParent = _chest ? _chest : _spine ? _spine.GetChild(0) : null;

            if (boostPackParent != null)
            {
                GameObject jetpackPos = CreateObject("jetpackPos", boostPackParent);
                GameObject jetpack = CreateObject("jetpack", jetpackPos.transform);

                _boostPack = jetpack.transform;
            }
        }
        if (!_boost && _boostPack)
        {
            GameObject boostPos = CreateObject("boostPos", _boostPack);
            GameObject boost = CreateObject("boost", boostPos.transform);

            _boost = boost.transform;
            _boost.rotation = Quaternion.Euler(BOOST_ANGLE);
        }

        if (!_propL)
        {
            _propL = CreateObject("propl", _leftHand).transform;
            _propL.rotation = Quaternion.Euler(PROP_L_ANGLE);
        }
        if (!_propR)
        {
            _propR = CreateObject("propr", _rightHand).transform;
            _propR.rotation = Quaternion.Euler(PROP_R_ANGLE);
        }

        if (!_bmxFrame)
        {
            _bmxFrame = CreateObject("bmxFrame", prefabRoot).transform;
        }
        if (!_bmxGear)
        {
            _bmxGear = CreateObject("bmxGear", _bmxFrame).transform;
        }
        if (!_bmxHandlebars)
        {
            _bmxHandlebars = CreateObject("bmxHandlebars", _bmxFrame).transform;
        }
        if (!_bmxWheelF)
        {
            _bmxWheelF = CreateObject("bmxWheelF", _bmxHandlebars).transform;
        }
        if (!_bmxWheelR)
        {
            _bmxWheelR = CreateObject("bmxWheelR", _bmxFrame).transform;
        }
        if (!_bmxPedalL)
        {
            _bmxPedalL = CreateObject("bmxPedalL", _bmxGear).transform;
        }
        if (!_bmxPedalR)
        {
            _bmxPedalR = CreateObject("bmxPedalR", _bmxGear).transform;
        }

        if (!_leftHandIK)
        {
            _leftHandIK = CreateObject("handlIK", prefabRoot).transform;
        }
        if (!_rightHandIK)
        {
            _rightHandIK = CreateObject("handrIK", prefabRoot).transform;
        }

        if (!_phoneDirectionRoot)
        {
            _phoneDirectionRoot = CreateObject("phoneDirectionRoot", prefabRoot).transform;
        }
        if (!_phoneDirection)
        {
            _phoneDirection = CreateObject("phoneDirection", _phoneDirectionRoot).transform;
        }

        if (!_skateboard)
        {
            _skateboard = CreateObject("skateboard", prefabRoot).transform;
        }
    }


    private void SetupSkateOffsets()
    {
        if (_leftFoot)
        {
            _skateOffsetL = _leftFoot.Find("skateOffsetL");
        }
        if (_rightFoot)
        {
            _skateOffsetR = _rightFoot.Find("skateOffsetR");
        }
    }
    public void ValidateSkateOffsets()
    {
        bool hasNoFeet = !_leftFoot || !_rightFoot;
        if (hasNoFeet)
        {
            GUI.enabled = false;
        }
        else
        {
            SetupSkateOffsets();
        }

        EditorGUILayout.LabelField("Inline Skates Offsets", EditorStyles.boldLabel);

        if (!_skateOffsetL || !_skateOffsetR)
        {
            if (GUILayout.Button("Add transforms"))
            {
                if (!_skateOffsetL)
                {
                    _skateOffsetL = CreateObject("skateOffsetL", _leftFoot).transform;
                    _skateOffsetL.rotation = Quaternion.Euler(-65.0f, 0.0f, 90.0f);
                }
                if (!_skateOffsetR)
                {
                    _skateOffsetR = CreateObject("skateOffsetR", _rightFoot).transform;
                    _skateOffsetR.rotation = Quaternion.Euler(-65.0f, 0.0f, 90.0f);
                }
            }
        }
        else if (_skateOffsetL && _skateOffsetR)
        {
            PreviewSkates = EditorGUILayout.Toggle("Preview Skates", PreviewSkates);
            EditorGUILayout.ObjectField("Left Offset", _skateOffsetL, typeof(Transform), false);
            EditorGUILayout.ObjectField("Right Offset", _skateOffsetR, typeof(Transform), false);
        }

        if (hasNoFeet)
        {
            GUI.enabled = true;
        }
    }

    private GameObject CreateObject(string name, Transform parent)
    {
        GameObject newBone = new GameObject(name);

        StageUtility.PlaceGameObjectInCurrentStage(newBone);
        GameObjectUtility.SetParentAndAlign(newBone, parent.gameObject);
        Undo.RegisterCreatedObjectUndo(newBone, $"Created {name}");

        return newBone;
    }
}