using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SubAssetTools
{
    [MenuItem("Assets/Delete SubAsset(s)")]
    public static void DeletePotentialSubAssets()
    {
        foreach (Object obj in Selection.objects)
        {
            AssetDatabase.RemoveObjectFromAsset(obj);
        }
        AssetDatabase.SaveAssets();
    }
    [MenuItem("Assets/Delete SubAsset(s)", true)]
    public static bool ValidateSubAsset()
    {
        foreach (Object obj in Selection.objects)
        {
            if (!AssetDatabase.IsSubAsset(obj))
            {
                return false;
            }
        }
        return true;
    }
}
