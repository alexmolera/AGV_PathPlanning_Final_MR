using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{

    public LayerMask WallMask;//This is the mask that the program will look for when trying to find obstructions to the path.
    private Vector2 vGridWorldSize;//A vector2 to store the width and height of the graph in world units.
    public float fNodeRadius;//This stores how big each square on the graph will be
    public float fDistanceBetweenNodes;//The distance that the squares will spawn from eachother.
    public GameObject floor;
    public float floorAngle;

    public int SafetyZone;
    [HideInInspector] public bool gridCreated;

    [HideInInspector] public Node[,] NodeArray;//The array of nodes that the A Star algorithm uses.
    public List<Node> FinalPath;//The completed path that the red line will be drawn along


    float fNodeDiameter;//Twice the amount of the radius (Set in the start function)
    [HideInInspector] public int iGridSizeX, iGridSizeY;//Size of the Grid in Array units.


    private void OnEnable()//Ran once the program is enabled
    {
        gridCreated = false;
        //Grid's size determinaiton

        vGridWorldSize.x = 10 * floor.transform.localScale.x;
        vGridWorldSize.y = 10 * floor.transform.localScale.z;
        fNodeDiameter = fNodeRadius * 2;//Double the radius to get diameter
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);//Divide the grids world co-ordinates by the diameter to get the size of the graph in array units.
    }

    void CreateGrid()
    {

        NodeArray = new Node[iGridSizeX, iGridSizeY];//Declare the array of nodess
        //Vector3.right & other components are part of the unitary vectors
        //Left corner's position
        Vector3 bottomLeft = floor.transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.

        for (int x = 0; x < iGridSizeX; x++)//Loop through the array of nodes.
        {
            for (int y = 0; y < iGridSizeY; y++)//Loop through the array of nodes
            {
                bool Wall = false;//Defines the variable as true

                Vector3 worldPoint = (bottomLeft + (new Vector3(1,0, floorAngle)) * (x * fNodeDiameter + fNodeRadius) + (new Vector3(-floorAngle, 0, 1)) * (y * fNodeDiameter + fNodeRadius));//Get the world co ordinates of the bottom left of the graph

                //If the node is not being obstructed
                //Quick collision check against the current node and anything in the world at its position. If it is colliding with an object with a WallMask,
                //The if statement will return false.
                //-------------------------------------------------

                if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask)) //radius is added as it needs the center point of the node
                {
                    Wall = true;//Object is not a wall
                }



                NodeArray[x, y] = new Node(Wall, worldPoint, x, y);//Create a new node in the array.

            }
        }
        //Check for the nodes

        SafetyNodes();
    }

    void SafetyNodes()
    {

        //To got horugh the array
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                //The main condition is that the closest node is a real wall, so that the new node can also be considered as obstructed
                if (Physics.CheckSphere(NodeArray[x, y].vPosition, fNodeRadius, WallMask))
                {
                    //To start adding, beacause if not the same node/wall would be considered as a part of the safety zone
                    //The main value is 1, if it were 0, the same node woul dbe considered
                    for (int k = 1; k <= SafetyZone; k++)
                    {
                        //Vertical movement forward
                        if (k + x < iGridSizeX)
                        {
                            if (!NodeArray[x + k, y].bIsWall)
                            {
                                NodeArray[x + k, y].bIsWall = true;

                            }
                        }
                        //vertical movement backwards
                        if (x - k >= 0)
                        {
                            if (!NodeArray[x - k, y].bIsWall)
                            {
                                NodeArray[x - k, y].bIsWall = true;

                            }
                        }
                        //Horizontal Movement forward
                        if (k + y < iGridSizeY)
                        {
                            if (!NodeArray[x, y + k].bIsWall)
                            {
                                NodeArray[x, y + k].bIsWall = true;

                            }
                        }
                        //Horizontal movement backwards
                        if (y - k >= 0)
                        {
                            if (!NodeArray[x, y - k].bIsWall)
                            {
                                NodeArray[x, y - k].bIsWall = true;

                            }
                        }
                        //Major diagonal movement forward
                        if (k + y < iGridSizeY && k + x < iGridSizeX)
                        {
                            if (!NodeArray[x + k, y + k].bIsWall)
                            {
                                NodeArray[x + k, y + k].bIsWall = true;

                            }
                        }
                        //Major diagonal movement backwards
                        if (y - k >= 0 && x - k >= 0)
                        {
                            if (!NodeArray[x - k, y - k].bIsWall)
                            {
                                NodeArray[x - k, y - k].bIsWall = true;

                            }
                        }
                        //secondary diagonal movement to the +y axis
                        if (k + y < iGridSizeY && x - k >= 0)
                        {
                            if (!NodeArray[x - k, y + k].bIsWall)
                            {
                                NodeArray[x - k, y + k].bIsWall = true;

                            }
                        }
                        //secondary diagonal movement to the -y axis
                        if (y - k >= 0 && k + x < iGridSizeX)
                        {
                            if (!NodeArray[x + k, y - k].bIsWall)
                            {
                                NodeArray[x + k, y - k].bIsWall = true;

                            }
                        }

                    }
                }

            }
        }


    }



    //Gets the closest node to the given world position.
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {

        float ixPos = ((a_vWorldPos.x / 4.92f * (vGridWorldSize.x / 2) + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z / 4.92f * (vGridWorldSize.y / 2) + vGridWorldSize.y / 2) / vGridWorldSize.y);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }



    //Function that draws the wireframe
    private void OnDrawGizmos()
    {

        Gizmos.DrawWireCube(floor.transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));//Draw a wire cube with the given dimensions from the Unity inspector

        if (NodeArray != null)//If the grid is not empty
        {
            foreach (Node n in NodeArray)//Loop through every node in the grid
            {
                if (!n.bIsWall)//If the current node is a wall node
                {
                    Gizmos.color = Color.white;//Set the color of the node
                }
                else
                {
                    Gizmos.color = Color.blue;//Set the color of the node
                }


                if (FinalPath != null)//If the final path is not empty
                {
                    if (FinalPath.Contains(n))//If the current node is in the final path
                    {
                        Gizmos.color = Color.green;//Set the color of that node
                    }

                }

                Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.
            }
        }
    }

    public void MakeGrid()
    {
        CreateGrid();//Draw the grid
        gridCreated = true;
    }
}
