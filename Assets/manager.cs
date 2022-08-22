using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Queue<System.Tuple<string, int, int>> MoveQueue;
    // solved state of the rubixc cube
    public GameObject[,,] solvedState;
    // list of all cube gameobjects
    public List<GameObject> allCubes;
    // list of all possible moves
    public List<string> moveList;
    // how much to scramble cube at start
    public int ScrambleAmt;

    public Vector3 center;

    // parent object for all cubes
    public GameObject cubeParent;

    void Start()
    {
        System.Diagnostics.ProcessStartInfo start = new();
        start.FileName = Application.dataPath + "/c#test";

        cubePrefab = Resources.Load("CubePrefab") as GameObject;
        cubeMaterialBase = Resources.Load("cube") as Material;
        if (rotateSpeed <= 0)
            rotateSpeed = 500;
        if (ScrambleAmt <= 0)
            ScrambleAmt = 100;
        if (cubeDims.Length != 3 || cubeDims[0] == 0 || cubeDims[1] == 0 || cubeDims[2] == 0)
            cubeDims = new int[] { 3, 3, 3 };
        else
            cubeDims = new int[] { cubeDims[0], cubeDims[1], cubeDims[2] };

        cubeParent = new GameObject("Cube Parent");
        cubeParent.transform.position = new Vector3((cubeDims[0] - 1) / 2f, (cubeDims[1] - 1) / 2f, (cubeDims[2] - 1) / 2f);
        cubeParent.AddComponent<Rigidbody>();

        // spawn rubix cube, solved state, and a list of all cubes spawned
        var spawnOut = SpawnCube(cubeDims[0], cubeDims[1], cubeDims[2]);

        rubixCube = spawnOut.Item1;
        solvedState = spawnOut.Item2;
        allCubes = spawnOut.Item3;

        // setting up queue of next moves
        MoveQueue = new Queue<System.Tuple<string, int, int>>();

        // setting up movelist
        // "m" is front middle vertical, "e" is front middle horizontal, "s" is middle wrapper between front and back
        string[] strArr = { "l", "li", "m", "mi", "r", "ri", "d", "di", "e", "ei", "u", "ui", "f", "fi", "s", "si", "b", "bi" };

        // to test out one of each move:
        moveList.AddRange(strArr);

        // to spawn clickable arrows on cube
        // SpawnArrows();

        // making sure initial state of cube is "solved"
        print(CheckSolved());

        PrintArr(rubixCube);

        Scramble(ScrambleAmt);
        // Rotate("x", 0, 1);
        // Rotate("y", 0, 1);
        // Rotate("z", 0, 1);
        // Rotate("z", 0, -1);
        // Rotate("z", 0, -1);
            //SpawnArrows();

        // making sure initial state of cube is "solved"
        print(CheckSolved());
            //Scramble(ScrambleAmt);

        // basic script to test every layer of a nxnxn cube forwards and backwards
        /*
        for (int i = 0; i < cubeDims[0]; i++)
        {
            Rotate("x", i, 1);
            Rotate("x", i, -1);
            Rotate("y", i, 1);
            Rotate("y", i, -1);
            Rotate("z", i, 1);
            Rotate("z", i, -1);
        }
        */


    }

    void Update()
    {
        if (!currentlyRotating && MoveQueue.Count > 0)
         {
             System.Tuple<string, int, int> move = MoveQueue.Dequeue();
            Rotate(move.Item1, move.Item2, move.Item3);
         }
        center = GameObject.Find("THE CUBE").transform.position;
    }

    void RotateStr(string rot)
    {
        /*
         * rotate the cube based on a string move name from MoveList
         * 
         */
        // setting up spin direction
        int spinDirection = 0;
        int layer = -1;
        string axis = "";
        if (rot == "l" || rot == "ri" || rot == "m")
            spinDirection = 1;
        if (rot == "li" || rot == "r" || rot == "mi")
            spinDirection = -1;
        if (rot == "u" || rot == "di" || rot == "ei")
            spinDirection = 1;
        if (rot == "d" || rot == "ui" || rot == "e")
            spinDirection = -1;
        if (rot == "f" || rot == "bi" || rot == "s")
            spinDirection = 1;
        if (rot == "fi" || rot == "b" || rot == "si")
            spinDirection = -1;

        if (rot == "l" || rot == "li" || rot == "d" || rot == "di" || rot == "f" || rot == "fi")
            layer = 0;
        if (rot == "m" || rot == "mi" || rot == "e" || rot == "ei" || rot == "s" || rot == "si")
            layer = 1;
        if (rot == "r" || rot == "ri" || rot == "u" || rot == "ui" || rot == "b" || rot == "bi")
            layer = 2;

        if (rot == "l" || rot == "li" || rot == "m" || rot == "mi" || rot == "r" || rot == "ri")
            axis = "x";
        if (rot == "u" || rot == "ui" || rot == "e" || rot == "ei" || rot == "d" || rot == "di")
            axis = "y";
        if (rot == "f" || rot == "fi" || rot == "s" || rot == "si" || rot == "b" || rot == "bi")
            axis = "z";
        Rotate(axis, layer, spinDirection);
    }

    public void Rotate(string axis, int layer, int direction)
    {
        /*
         * axis : "x", "y", or "z"
         * layer : 0 to n where "n" is the size of the cube - 1
         * direction : "n" (normal) for clockwise, "p" (prime) for counterclockwise
         */

        if (direction == 0)
            return;
        if (currentlyRotating)
            MoveQueue.Enqueue(System.Tuple.Create(axis, layer, direction));
        else
        {
            if (axis == "x")
            {
                if (direction == 1)
                    StartCoroutine(RotateX(-1, layer));
                else
                    StartCoroutine(RotateX(1, layer));
            }
            else if (axis == "y")
            {
                if (direction == 1)
                    StartCoroutine(RotateY(1, layer));
                else
                    StartCoroutine(RotateY(-1, layer));
            }
            else // axis == z
            {
                if (direction == 1)
                    StartCoroutine(RotateZ(1, layer));
                else
                    StartCoroutine(RotateZ(-1, layer));
            }

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

    System.Tuple<GameObject[,,], GameObject[,,], List<GameObject>> SpawnCube(int dim0, int dim1, int dim2)
    {
        // iterators for 3x3x3 array
        int x, y, z;
        GameObject[,,] rubixCube = new GameObject[cubeDims[0], cubeDims[1], cubeDims[2]];
        GameObject[,,] solvedState = new GameObject[cubeDims[0], cubeDims[1], cubeDims[2]];
        List<GameObject> allCubes = new();
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789##";
        int alph = 0;
        center = new Vector3((cubeDims[0] - 1)/2f, (cubeDims[0] - 1)/2f, (cubeDims[0] - 1)/2f);
        GameObject.Find("THE CUBE").transform.position = center;
        for (x = 0; x < dim0; x++)
        {
            for (y = 0; y < dim1; y++)
            {
                for (z = 0; z < dim2; z++)
                {
                    // spawning cube at position (x, y, z) centered around 0, 0, 0
                    if (x == 0 || y == 0 || z == 0 || x == dim0 - 1 || y == dim1 - 1 || z == dim2 - 1)
                    {
                        rubixCube[x, y, z] = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity, cubeParent.transform);
                        if (alph > alphabet.Length - 1)
                        {
                            rubixCube[x, y, z].name = "#";
                        }
                        else
                        {
                            rubixCube[x, y, z].name = alphabet[alph].ToString();
                        }
                        
                        alph++;
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
                    // not spawning cubes in center
                    else
                        rubixCube[x, y, z] = null;
                    solvedState[x, y, z] = rubixCube[x, y, z];
                }
            }
        }
        return System.Tuple.Create(rubixCube, solvedState, allCubes);
    }

    IEnumerator RotateX(int spinDirection, int layer)
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
                    if (rubixCube[layer, a, b] != null)
                    {
                        rubixCube[layer, a, b].transform.RotateAround(cubeParent.transform.position, rubixCube[layer, a, b].transform.parent.transform.right * spinDirection * 1, rotate);
                        if (rotatedAmount == 90)
                        {
                            // rubixCube[layer, a, b].transform.localPosition = new Vector3(layer - (0.5f * (cubeDims[0] - 1)), a - (0.5f * (cubeDims[1] - 1)), b - (0.5f * (cubeDims[2] - 1)));
                        }
                    }
                }

            }
            yield return null;
        }
        RotateXArr(spinDirection, layer);
        currentlyRotating = false;
        print(CheckSolved());
        PrintArr(rubixCube);

    }

    IEnumerator RotateY(int spinDirection, int layer)
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
                    if (rubixCube[a, layer, b] != null)
                    {
                        rubixCube[a, layer, b].transform.RotateAround(cubeParent.transform.position, rubixCube[a, layer, b].transform.parent.transform.up * spinDirection * 1, rotate);
                        if (rotatedAmount == 90)
                        {
                            // rubixCube[a, layer, b].transform.localPosition = new Vector3(a - (0.5f * (cubeDims[0] - 1)), layer - (0.5f * (cubeDims[1] - 1)), b - (0.5f * (cubeDims[2] - 1)));
                        }
                    }
                }

            }
            yield return null;
        }
        currentlyRotating = false;
        RotateYArr(spinDirection, layer);
        print(CheckSolved());
        PrintArr(rubixCube);
    }

    IEnumerator RotateZ(int spinDirection, int layer)
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
                    if (rubixCube[a, b, layer] != null)
                    {
                        rubixCube[a, b, layer].transform.RotateAround(cubeParent.transform.position, rubixCube[a, b, layer].transform.parent.transform.forward * spinDirection * 1, rotate);
                        if (rotatedAmount == 90)
                        {
                            // rubixCube[a, b, layer].transform.localPosition = new Vector3(a - (0.5f * (cubeDims[0] - 1)), b - (0.5f * (cubeDims[1] - 1)), layer - (0.5f * (cubeDims[2] - 1)));
                        }
                    }
                }

            }
            yield return null;
        }
        RotateZArr(spinDirection, layer);
        currentlyRotating = false;
        print(CheckSolved());
        PrintArr(rubixCube);
    }

    void Scramble(int moves)
    {
        string[] axes = { "x", "y", "z" };
        int[] direction = { -1, 1 };
        int[] layerx = Enumerable.Range(0, cubeDims[0] - 1).ToArray();
        int[] layery = Enumerable.Range(0, cubeDims[1] - 1).ToArray();
        int[] layerz = Enumerable.Range(0, cubeDims[2] - 1).ToArray();
        for (int i = 0; i < moves + 1; i++)
        {
            string ax = axes[Random.Range(0, 3)];
            int dir = direction[Random.Range(0, 2)];
            int lay = -1;
            if (ax == "x")
                lay = layerx[Random.Range(0, layerx.Length)];
            if (ax == "y")
                lay = layery[Random.Range(0, layery.Length)];
            if (ax == "z")
                lay = layerz[Random.Range(0, layerz.Length)];
            Rotate(ax, lay, dir);
        }
    }

    void SpawnArrows()
    {
        // TrianglePrimitive.CreateObject();
        for (int i = 0; i < cubeDims[0]; i++)
        {
            GameObject tri1 = TrianglePrimitive.CreateObject();
            tri1.transform.position = new Vector3(-1, cubeDims[0] - 1, i);
            tri1.transform.rotation = Quaternion.Euler(-90, 0, 90);
            GameObject tri2 = TrianglePrimitive.CreateObject();
            tri2.transform.position = new Vector3(cubeDims[0], cubeDims[0] - 1, i);
            tri2.transform.rotation = Quaternion.Euler(-90, 0, 270);
        }
        for (int i = 0; i < cubeDims[0]; i++)
        {
            GameObject tri1 = TrianglePrimitive.CreateObject();
            tri1.transform.position = new Vector3(i, cubeDims[2] - 1, -1);
            tri1.transform.rotation = Quaternion.Euler(-90, 0, 0);
            GameObject tri2 = TrianglePrimitive.CreateObject();
            tri2.transform.position = new Vector3(i, cubeDims[2] - 1, cubeDims[2]);
            tri2.transform.rotation = Quaternion.Euler(-90, 0, 180);
        }
    }

    void PrintArr(GameObject[,,] arr)
    {
        string row;
        for (int i = 0; i < cubeDims[0]; i++)
        {
            row = "";
            for (int j = 0; j < cubeDims[1]; j++)
            {
                for (int k = 0; k < cubeDims[2]; k++)
                {
                    if (i == 0 || i == cubeDims[0] - 1 || j == 0 || j == cubeDims[1] - 1 || k == 0 || k == cubeDims[2] - 1)
                        row += arr[i, j, k].name + " ";
                    else
                        row += "@ ";
                }
                row += "\n";
            }
        }
    }

    GameObject[,,] CopyArr(GameObject[,,] arr)
    {
        GameObject[,,] copy = new GameObject[cubeDims[0], cubeDims[1], cubeDims[2]];
        for (int i = 0; i < cubeDims[0]; i++)
        {
            for (int j = 0; j < cubeDims[1]; j++)
            {
                for (int k = 0; k < cubeDims[2]; k++)
                {
                    copy[i, j, k] = arr[i, j, k];
                }
            }
        }
        return copy;
    }

    void RotateXArr(int direction, int layer)
    {
        GameObject[,,] copy = CopyArr(rubixCube);
        // rotate X left:
        if (direction == 1)
        {
            for (int i = 0; i < cubeDims[1]; i++)
            {
                for (int j = 0; j < cubeDims[2]; j++)
                {

                    rubixCube[layer, i, j] = copy[layer, j, cubeDims[0] - 1 - i];
                }
            }
        }

        // rotate X right:
        if (direction == -1)
        {
            for (int i = 0; i < cubeDims[1]; i++)
            {
                for (int j = 0; j < cubeDims[2]; j++)
                {
                    rubixCube[layer, j, cubeDims[0] - 1 - i] = copy[layer, i, j];
                }
            }
        }
    }

    void RotateYArr(int direction, int layer)
    {
        GameObject[,,] copy = CopyArr(rubixCube);
        // rotate Y left:
        if (direction == -1)
        {
            for (int i = 0; i < cubeDims[0]; i++)
            {
                for (int j = 0; j < cubeDims[2]; j++)
                {
                    rubixCube[i, layer, j] = copy[j, layer, cubeDims[1] - 1 - i];
                }
            }
        }

        // rotate Y right:
        if (direction == 1)
        {
            for (int i = 0; i < cubeDims[0]; i++)
            {
                for (int j = 0; j < cubeDims[2]; j++)
                {
                    rubixCube[j, layer, cubeDims[1] - 1 - i] = copy[i, layer, j];
                }
            }
        }
    }

    void RotateZArr(int direction, int layer)
    {

        GameObject[,,] copy = CopyArr(rubixCube);
        // rotate Z left:
        if (direction == 1)
        {
            for (int i = 0; i < cubeDims[0]; i++)
            {
                for (int j = 0; j < cubeDims[1]; j++)
                {
                    rubixCube[i, j, layer] = copy[j, cubeDims[2] - 1 - i, layer];
                }
            }
        }

        // rotate Z right:
        if (direction == -1)
        {
            for (int i = 0; i < cubeDims[0]; i++)
            {
                for (int j = 0; j < cubeDims[1]; j++)
                {
                    rubixCube[j, cubeDims[2] - 1 - i, layer] = copy[i, j, layer];
                }
            }
        }
    }
}

