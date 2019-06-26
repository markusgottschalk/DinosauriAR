using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition
{
    public string Name { get; set; }
    public Image picture;
    public string InfoText;
    public bool active;

    public Expedition(string name)
    {
        Name = name;
        active = false;
    }
}
