using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour
{
    // cube prefab that is used to spawn all cubes
    public GameObject cubePrefab;
    // 3x3x3 array that is used to hold all cubes
    public GameObject[,,] cubes;
    // material prefab used to spawn all cube materials
    public Material cubeMaterialBase;
    // Start is called before the first frame update
    void Start()
    {
        // create empty 3x3x3 array "cubes"
        cubes = new GameObject[3, 3, 3];
        // iterators for 3x3x3 array
        int x, y, z;
        for (x = 0; x < 3; x++)
        {
            for (y = 0; y < 3; y++)
            {
                for (z = 0; z < 3; z++)
                {
                    // spawning cube at position (x, y, z) centered around 0, 0, 0
                    cubes[x, y, z] = Instantiate(cubePrefab, new Vector3(x - 1, y - 1, z - 1), Quaternion.identity);
                    // creating a copy of material to give to cube
                    Material thisMat = new Material(cubeMaterialBase);
                    // setting cube script's "myMat" variable to this material
                    cubes[x, y, z].GetComponent<Cube>().myMat = thisMat;
                    // setting cube's "arrayPos" variable to its [x, y, z] coordinate
                    cubes[x, y, z].GetComponent<Cube>().arrayPos = new int[] {x, y, z};
                    // setting cube's material to newly created material (currently all black)
                    cubes[x, y, z].GetComponent<MeshRenderer>().sharedMaterial = thisMat;

                    // if cube is on the left side
                    if (x == 0)
                        thisMat.SetColor("_Left", Color.red);
                    // if cube is on right side
                    if (x == 2)
                        thisMat.SetColor("_Right", new Color(1, 0.64f, 0));
                    // if cube is on bottom
                    if (y == 0)
                        thisMat.SetColor("_Bottom", Color.yellow);
                    // if cube is on top
                    if (y == 2)
                        thisMat.SetColor("_Top", Color.white);
                    // if cube is on back
                    if (z == 0)
                        thisMat.SetColor("_Back", Color.blue);
                    // if cube is on front
                    if (z == 2)
                        thisMat.SetColor("_Front", Color.green);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
