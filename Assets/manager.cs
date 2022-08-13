using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using np = Numpy;
using System.Linq;
public class Manager : MonoBehaviour
{
    // cube prefab that is used to spawn all cubes
    public GameObject cubePrefab;
    // X x Y x Z array that is used to represent the state of the rubix cube
    public GameObject[,,] rubixCube;
    // material prefab used to spawn all cube materials
    public Material cubeMaterialBase;
    // dimensions of the rubix cube
    public int[] cubeDims;
    // speed the cube can rotate
    public float rotateSpeed;
    // flag for if cube is currently rotating
    public bool currentlyRotating;
    // a queue to keep track of all queued up moves
    public Queue<string> MoveQueue;
    // solved state of the rubixc cube
    public GameObject[,,] solvedState;
    // list of all cube gameobjects
    public List<GameObject> allCubes;
    // list of all possible moves
    public List<string> moveList;
    // how much to scramble cube at start
    public int ScrambleAmt;
    /* 
    left : 0, *, *
    m : 1, *, *
    right : n, *, *

    up : *, n, *
    e : *, 1, *
    down : *, 0, *

    front : * , * , 0
    s : *, *, 1
    back : *, *, 2
    */

    void Start()
    {
        // spawn rubix cube, solved state, and a list of all cubes spawned
        var spawnOut = SpawnCube(cubeDims[0], cubeDims[1], cubeDims[2]);
        rubixCube = spawnOut.Item1;
        solvedState = spawnOut.Item2;
        allCubes = spawnOut.Item3;
        // setting up queue of next moves
        MoveQueue = new Queue<string>();
        // setting up movelist
        // "m" is front middle vertical, "e" is front middle horizontal, "s" is middle wrapper between front and back
        string[] strArr = { "l", "li", "m", "mi", "r", "ri", "d", "di", "e", "ei", "u", "ui", "f", "fi", "s", "si", "b", "bi"};
        moveList.AddRange(strArr);

        // making sure initial state of cube is "solved"
        print(CheckSolved());

        /*
        foreach( string s in moveList)
        {
            Rotate(s);
        }
        */
        Scramble(ScrambleAmt);
    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Rotate("l"));
        }
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Rotate("li"));
        }
        */
        if (!currentlyRotating && MoveQueue.Count > 0)
        {
            Rotate(MoveQueue.Dequeue());
        }
    }

    void Rotate(string rot)
    {
        if (currentlyRotating)
            MoveQueue.Enqueue(rot);
        else
        {
            // setting up spin direction
            Vector3 spinDirection = Vector3.zero;
            if (rot == "l" || rot == "ri" || rot == "m")
                spinDirection = Vector3.left;
            if (rot == "li" || rot == "r" || rot == "mi")
                spinDirection = Vector3.right;
            if (rot == "u" || rot == "di" || rot == "ei")
                spinDirection = Vector3.up;
            if (rot == "d" || rot == "ui" || rot == "e")
                spinDirection = Vector3.down;
            if (rot == "f" || rot == "bi" || rot == "s")
                spinDirection = Vector3.forward;
            if (rot == "fi" || rot == "b" || rot == "si")
                spinDirection = Vector3.back;

            if (rot == "l" || rot == "li")
                StartCoroutine(RotateL(spinDirection));
            if (rot == "m" || rot == "mi")
                StartCoroutine(RotateM(spinDirection));
            if (rot == "r" || rot == "ri")
                StartCoroutine(RotateR(spinDirection));
            if (rot == "d" || rot == "di")
                StartCoroutine(RotateD(spinDirection));
            if (rot == "e" || rot == "ei")
                StartCoroutine(RotateE(spinDirection));
            if (rot == "u" || rot == "ui")
                StartCoroutine(RotateU(spinDirection));
            if (rot == "f" || rot == "fi")
                StartCoroutine(RotateF(spinDirection));
            if (rot == "s" || rot == "si")
                StartCoroutine(RotateS(spinDirection));
            if (rot == "b" || rot == "bi")
                StartCoroutine(RotateB(spinDirection));
        }
    }

    bool CheckSolved()
    {
        int x, y, z;
        for (x = 0; x < cubeDims[0]; x++)
        {
            for (y = 0; y < cubeDims[1]; y++)
            {
                for (z = 0; z < cubeDims[2]; z++)
                {
                    if (rubixCube[x, y, z] != solvedState[x, y, z])
                        return false;
                }
            }
        }
        return true;

    }

    void UpdateArray()
    {
        foreach(GameObject cube in allCubes)
        {
            cube.GetComponent<Cube>().arrayPos = new int[] { ((int)cube.transform.position.x), ((int)cube.transform.position.y), ((int)cube.transform.position.z)};
            int[] ap = cube.GetComponent<Cube>().arrayPos;
            rubixCube[ap[0] + 1, ap[1] + 1, ap[2] + 1] = cube;
        }
    }

    System.Tuple<GameObject[,,], GameObject[,,], List<GameObject>> SpawnCube(int dim0, int dim1, int dim2)
    {
        // iterators for 3x3x3 array
        int x, y, z;
        GameObject[,,] rubixCube = new GameObject[cubeDims[0], cubeDims[1], cubeDims[2]];
        GameObject[,,] solvedState = new GameObject[cubeDims[0], cubeDims[1], cubeDims[2]];
        List<GameObject> allCubes = new();
        for (x = 0; x < dim0; x++)
        {
            for (y = 0; y < dim1; y++)
            {
                for (z = 0; z < dim2; z++)
                {
                    // spawning cube at position (x, y, z) centered around 0, 0, 0
                    rubixCube[x, y, z] = Instantiate(cubePrefab, new Vector3(x - 1, y - 1, z - 1), Quaternion.identity);
                    solvedState[x, y, z] = rubixCube[x, y, z];
                    allCubes.Add(rubixCube[x, y, z]);
                    // creating a copy of material to give to cube
                    Material thisMat = new(cubeMaterialBase);
                    // setting cube script's "myMat" variable to this material
                    rubixCube[x, y, z].GetComponent<Cube>().myMat = thisMat;
                    // setting cube's "arrayPos" variable to its [x, y, z] coordinate
                    rubixCube[x, y, z].GetComponent<Cube>().arrayPos = new int[] { x, y, z };
                    // setting cube's material to newly created material (currently all black)
                    rubixCube[x, y, z].GetComponent<MeshRenderer>().sharedMaterial = thisMat;

                    // if cube is on the left side
                    if (x == 0)
                        thisMat.SetColor("_Left", Color.red);
                    // if cube is on right side
                    if (x == dim0 - 1)
                        thisMat.SetColor("_Right", new Color(1, 0.64f, 0));
                    // if cube is on bottom
                    if (y == 0)
                        thisMat.SetColor("_Bottom", Color.yellow);
                    // if cube is on top
                    if (y == dim1 - 1)
                        thisMat.SetColor("_Top", Color.white);
                    // if cube is on back
                    if (z == 0)
                        thisMat.SetColor("_Back", Color.blue);
                    // if cube is on front
                    if (z == dim2 - 1)
                        thisMat.SetColor("_Front", Color.green);
                }
            }
        }
        return System.Tuple.Create(rubixCube, solvedState, allCubes);
    }

    IEnumerator RotateL(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[0, a, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[0, a, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[0, a, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateM(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[1, a, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[1, a, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[1, a, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateR(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[2, a, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[2, a, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[2, a, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateD(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, 0, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, 0, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, 0, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateE(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, 1, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, 1, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, 1, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateU(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, 2, b].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, 2, b].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, 2, b].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateF(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, b, 0].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, b, 0].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, b, 0].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateS(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, b, 1].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, b, 1].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, b, 1].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    IEnumerator RotateB(Vector3 spinDirection)
    {
        float rotatedAmount = 0;
        float rotate;
        currentlyRotating = true;
        while (rotatedAmount < 90)
        {
            if (rotatedAmount + (Time.deltaTime * rotateSpeed) > 90)
                rotate = (90 - rotatedAmount);
            else
                rotate = Time.deltaTime * rotateSpeed;
            rotatedAmount += rotate;
            int a, b;
            for (a = 0; a < cubeDims[1]; a++)
            {
                for (b = 0; b < cubeDims[2]; b++)
                {
                    rubixCube[a, b, 2].transform.RotateAround(Vector3.zero, spinDirection, rotate);
                    if (rotatedAmount == 90)
                    {
                        Vector3 pos = rubixCube[a, b, 2].transform.position;
                        // rounding cube position to int rather than float
                        rubixCube[a, b, 2].transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        UpdateArray();
        print(CheckSolved());
    }

    void Scramble(int moves)
    {
        for (int i = 0; i < moves + 1; i++)
        {
            Rotate(moveList[Random.Range(0, moveList.Count)]);
        }
    }
}

