using CrewBoomMono;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorPropPreviews
{
    private const string REFERENCE_PATH = "Assets/Reference Models/";
    private const string SKATES_MESH_L_PATH = REFERENCE_PATH + "skateLeft.obj";
    private const string SKATES_MESH_R_PATH = REFERENCE_PATH + "skateRight.obj";
    private const string SPRAYCAN_MESH_PATH = REFERENCE_PATH + "spraycanMesh.obj";
    private const string PHONE_MESH_PATH = REFERENCE_PATH + "phoneMesh.obj";
    private const string PREVIEW_MATERIAL_PATH = REFERENCE_PATH + "previewMaterial.mat";
    private const string SPRAYCAN_MATERIAL_PATH = REFERENCE_PATH + "spraycanMesh_0Mat.mat";
    private const string BOOST_MESH_PATH = REFERENCE_PATH + "boostMesh.obj";
    private const string BOOST_MATERIAL_PATH = REFERENCE_PATH + "boostMesh_0Mat.mat";
    static EditorPropPreviews()
    {
        CharacterDefinition.SprayCanMaterial = AssetDatabase.LoadAssetAtPath<Material>(SPRAYCAN_MATERIAL_PATH);
        var spraycanAssets = AssetDatabase.LoadAllAssetsAtPath(SPRAYCAN_MESH_PATH);
        foreach (var obj in spraycanAssets)
        {
            if (obj as Mesh != null)
            {
                CharacterDefinition.SprayCanMesh = obj as Mesh;
                break;
            }
        }
    }
}
