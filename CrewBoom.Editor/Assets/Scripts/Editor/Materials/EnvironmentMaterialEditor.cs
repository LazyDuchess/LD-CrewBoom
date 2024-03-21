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

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
        ValidateNewShader(material);
    }

    private void ValidateNewShader(Material material)
    {
        var materials = new Material[] { material };
        var properties = MaterialEditor.GetMaterialProperties(materials);
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);
        ValidateTransparency(properties, material, (Transparency)transparencyProperty.floatValue);
        ValidateRenderQueue(properties, material, (Transparency)transparencyProperty.floatValue);
    }

    private void ValidateRenderQueue(MaterialProperty[] properties, Material material, Transparency transparency)
    {
        switch (transparency)
        {
            case Transparency.Opaque:
                material.renderQueue = 2000;
                break;

            case Transparency.Cutout:
                material.renderQueue = 2450;
                break;

            case Transparency.Transparent:
                material.renderQueue = 3000;
                break;
        }
    }

    private void ValidateTransparency(MaterialProperty[] properties, Material material, Transparency transparency)
    {
        var zWriteProperty = ShaderGUI.FindProperty("_ZWrite", properties);
        var blendSrcProperty = ShaderGUI.FindProperty("_BlendSrc", properties);
        var blendDestProperty = ShaderGUI.FindProperty("_BlendDst", properties);

        material.DisableKeyword("_TRANSPARENCY_OPAQUE");
        material.DisableKeyword("_TRANSPARENCY_CUTOUT");
        material.DisableKeyword("_TRANSPARENCY_TRANSPARENT");
        if (transparency == Transparency.Transparent)
        {
            blendSrcProperty.floatValue = 5f;
            blendDestProperty.floatValue = 10f;
        }
        else
        {
            blendSrcProperty.floatValue = 1f;
            blendDestProperty.floatValue = 0f;
        }
        zWriteProperty.floatValue = transparency == Transparency.Transparent ? 0 : 1;

        switch (transparency)
        {
            case Transparency.Opaque:
                material.EnableKeyword("_TRANSPARENCY_OPAQUE");
                break;

            case Transparency.Cutout:
                material.EnableKeyword("_TRANSPARENCY_CUTOUT");
                break;

            case Transparency.Transparent:
                material.EnableKeyword("_TRANSPARENCY_TRANSPARENT");
                break;
        }
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);

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
            transparencyProperty.floatValue = (float)transparencyMode;
            ValidateRenderQueue(properties, material, transparencyMode);
        }
        ValidateTransparency(properties, material, transparencyMode);
        if (transparencyMode == Transparency.Cutout)
        {
            materialEditor.ShaderProperty(cutoutProperty, "Alpha Cutout");
        }
        base.OnGUI(materialEditor, properties);
    }
}