using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public static class ShaderUtility
{
    public static bool IsGameShader(Shader shader)
    {
        return shader.name == "LD CrewBoom/Game Character";
    }
}
