using UnityEngine;
using UnityEditor;

// Custom Inspector for the BRC Character material.
public class CharacterMaterialEditor : ShaderGUI
{
    // Draw the inspector.
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        foreach (var property in properties)
        {
            if ((property.flags & MaterialProperty.PropFlags.HideInInspector) == MaterialProperty.PropFlags.HideInInspector)
                continue;
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                EditorGUILayout.BeginVertical("Box");
            }
            materialEditor.ShaderProperty(property, property.displayName);
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                var uvProperty = ShaderGUI.FindProperty($"{property.name}UV", properties);
                var scrollProperty = ShaderGUI.FindProperty($"{property.name}Scroll", properties);
                var uProperty = ShaderGUI.FindProperty($"{property.name}USpeed", properties);
                var vProperty = ShaderGUI.FindProperty($"{property.name}VSpeed", properties);
                materialEditor.ShaderProperty(uvProperty, uvProperty.displayName);
                materialEditor.ShaderProperty(scrollProperty, scrollProperty.displayName);
                if (scrollProperty.floatValue == 1f)
                {
                    EditorGUILayout.BeginHorizontal();
                    materialEditor.ShaderProperty(uProperty, uProperty.displayName);
                    materialEditor.ShaderProperty(vProperty, vProperty.displayName);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }
        materialEditor.RenderQueueField();
        //base.OnGUI(materialEditor, properties);
    }
}