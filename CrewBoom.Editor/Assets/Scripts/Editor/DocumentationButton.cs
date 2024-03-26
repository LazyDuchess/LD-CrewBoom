using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class DocumentationButton
{
    [MenuItem("CrewBoom/Documentation", false, 100)]
    private static void OpenDocumentation()
    {
        Application.OpenURL("https://github.com/LazyDuchess/LD-CrewBoom/wiki");
    }
}