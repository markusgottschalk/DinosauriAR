using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool Explored;
    public bool BonesAvailable { get; private set; }
    //public BlockMaterial Material { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Explored = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
