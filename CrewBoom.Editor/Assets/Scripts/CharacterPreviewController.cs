using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterPreviewController : MonoBehaviour
{
    [HideInInspector]
    public GameObject Character = null;
    public CharacterPreviewController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        var character = Instantiate(Character);
    }
}