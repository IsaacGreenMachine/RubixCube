using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // keeps track of this cube's material (for coloring cube sides)
    public Material myMat;
    // keeps track of cube's current [x, y, z] position and location in manager.cs's cubes[] array
    public int[] arrayPos;
    // Start is called before the first frame update
    void Start()
    {
        // setting arrayPos to empty 1d array of size 3
        arrayPos = new int[3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
