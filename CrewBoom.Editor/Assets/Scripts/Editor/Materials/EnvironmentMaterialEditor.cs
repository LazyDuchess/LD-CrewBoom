using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Xml.Schema;

public class EnvironmentMaterialEditor : ShaderGUI
{
    private enum Transparency
    {
        Opaque,
        Cutout,
        Transparent
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var zWriteProperty = ShaderGUI.FindProperty("_ZWrite", properties);
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);
        
        var blendSrcProperty = ShaderGUI.FindProperty("_BlendSrc", properties);
        var blendDestProperty = ShaderGUI.FindProperty("_BlendDst", properties);

        var cutoutProperty = ShaderGUI.FindProperty("_CutOut", properties);

        var material = materialEditor.target as Material;

        var transparencyOptions = new string[]
        {
            "Opaque",
            "Cutout",
            "Transparent"
        };
        var transparencyMode = (Transparency)transparencyProperty.floatValue;
        EditorGUI.BeginChangeCheck();
        {
            transparencyMode = (Transparency)EditorGUILayout.Popup("Render Mode", (int)transparencyMode, transparencyOptions);
        }
        if (EditorGUI.EndChangeCheck())
        {
            material.DisableKeyword("_TRANSPARENCY_OPAQUE");
            material.DisableKeyword("_TRANSPARENCY_CUTOUT");
            material.DisableKeyword("_TRANSPARENCY_TRANSPARENT");
            transparencyProperty.floatValue = (float)transparencyMode;
            switch (transparencyMode)
            {
                case Transparency.Opaque:
                    material.EnableKeyword("_TRANSPARENCY_OPAQUE");
                    material.renderQueue = 2000;
                    break;

                case Transparency.Cutout:
                    material.EnableKeyword("_TRANSPARENCY_CUTOUT");
                    material.renderQueue = 2450;
                    break;

                case Transparency.Transparent:
                    material.EnableKeyword("_TRANSPARENCY_TRANSPARENT");
                    material.renderQueue = 3000;
                    break;
            }
        }
        if (transparencyMode == Transparency.Transparent)
        {
            blendSrcProperty.floatValue = 5f;
            blendDestProperty.floatValue = 10f;
        }
        else
        {
            blendSrcProperty.floatValue = 1f;
            blendDestProperty.floatValue = 0f;
        }
        zWriteProperty.floatValue = transparencyMode == Transparency.Transparent ? 0 : 1;
        if (transparencyMode == Transparency.Cutout)
        {
            materialEditor.ShaderProperty(cutoutProperty, "Alpha Cutout");
        }
        base.OnGUI(materialEditor, properties);
    }
}