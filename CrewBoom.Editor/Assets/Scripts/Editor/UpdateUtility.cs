using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class UpdateUtility
{
    public static void UpdateCrewBoom()
    {
        var updateConfirmation = EditorUtility.DisplayDialog("Update CrewBoom", "Are you sure? Any assets not in the Characters folder might be deleted!", "Yes", "Cancel");
        if (!updateConfirmation)
            return;
    }
}