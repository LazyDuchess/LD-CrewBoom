using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CypherDanceButton : MonoBehaviour
{
    private void Awake()
    {
        if (!Directory.Exists("Assets/User/Freestyle"))
            gameObject.SetActive(false);
    }
}
