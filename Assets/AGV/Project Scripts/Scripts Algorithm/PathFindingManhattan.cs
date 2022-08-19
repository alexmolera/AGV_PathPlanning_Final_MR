using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManhattan : MonoBehaviour
{
    public Grid GridReference;//For referencing the grid class
    public GameObject Movement;
    public GameObject StartPosition;//Starting position to pathfind from
    public GameObject TargetPosition;//Starting position to pathfind to
    public GameObject floorReference;
    //public int NofNodes = 0;
    [HideInInspector] public bool pathFinded;
    [HideInInspector] public List<Node> FinalPath;



    private void Awake()//When the program starts
    {

    }
    private void Start()
    {
        pathFinded = false;
    }

    void OnEnable()
    {
        pathFinded = false;
    }

    private void Update()//Every frame
    {
        if (GetComponent<Grid>().gridCreated && !pathFinded)
        {
            StartPosition.transform.parent = floorReference.transform;
            TargetPosition.transform.parent = floorReference.transform;

            FindPath(StartPosition.transform.localPosition, TargetPosition.transform.localPosition); //Find a path to the goal
        }
        if (pathFinded)
        {
            pathFinded = false;
            Movement.SetActive(true);
            Movement.GetComponent<RobotMovement>().AppearRoadManhattan();
            gameObject.GetComponent<PathFindingManhattan>().enabled = false;
        }
    }

    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position
        List<Node> OpenList = new List<Node>();//List of nodes for the open list
        HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

        OpenList.Add(StartNode);//Add the starting node to the open list to begin the program

        while (OpenList.Count > 0)//Whilst there is something in the open list
        {
            Node CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
            for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].ihCost < CurrentNode.ihCost)//If the f cost of that object is less than or equal to the f cost of the current node
                {
                    CurrentNode = OpenList[i];//Set the current node to that object
                }
            }
            OpenList.Remove(CurrentNode);//Remove that from the open list
            ClosedList.Add(CurrentNode);//And add it to the closed list
            
            if (CurrentNode == TargetNode)//If the current node is the same as the target node
            {
                GetFinalPath(StartNode, TargetNode);//Calculate the final path
            }

            foreach (Node NeighborNode in GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
            {

                if (NeighborNode.bIsWall || ClosedList.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
                {
                    continue;//Skip it
                }
                float MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                if (MoveCost < NeighborNode.igCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                {
                    NeighborNode.igCost = MoveCost;//Set the g cost to the f cost
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                    NeighborNode.ParentNode = CurrentNode;//Set the parent of the node for retracing steps

                    if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                    {
                        OpenList.Add(NeighborNode);//Add it to the list
                    }
                }
            }

        }
    }

    //Function that gets the neighboring nodes of the given node.
    public List<Node> GetNeighboringNodes(Node a_NeighborNode)
    {
        List<Node> NeighborList = new List<Node>();//Make a new list of all available neighbors.
        int icheckX;//Variable to check if the XPosition is within range of the node array to avoid out of range errors.
        int icheckY;//Variable to check if the YPosition is within range of the node array to avoid out of range errors.

        //Check the right side of the current node.
        icheckX = a_NeighborNode.iGridX + 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < GridReference.iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < GridReference.iGridSizeY)//If the YPosition is in range of the array
            {
                NeighborList.Add(GridReference.NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Left side of the current node.
        icheckX = a_NeighborNode.iGridX - 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < GridReference.iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < GridReference.iGridSizeY)//If the YPosition is in range of the array
            {
                NeighborList.Add(GridReference.NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Top side of the current node.
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY + 1;
        if (icheckX >= 0 && icheckX < GridReference.iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < GridReference.iGridSizeY)//If the YPosition is in range of the array
            {
                NeighborList.Add(GridReference.NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }
        //Check the Bottom side of the current node.
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY - 1;
        if (icheckX >= 0 && icheckX < GridReference.iGridSizeX)//If the XPosition is in range of the array
        {
            if (icheckY >= 0 && icheckY < GridReference.iGridSizeY)//If the YPosition is in range of the array
            {
                NeighborList.Add(GridReference.NodeArray[icheckX, icheckY]);//Add the grid to the available neighbors list
            }
        }

        return NeighborList;//Return the neighbors list.
    }



    //It establishes the parental reference with the neighbor nodes found before to storage the path
    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        FinalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode)//While loop to work through each node going through the parents to the beginning of the path
        {
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.ParentNode;//Move onto its parent node
                                                 // NofNodes++;
        }

        FinalPath.Reverse();//Reverse the path to get the correct order

        GridReference.FinalPath = FinalPath;    //Set the final path for the gizmos drawing
        //The path is reduce as only the edge nodes must be considered so that the stair effect is avoided and we don't have unnecessary points
        FinalPath = TargetsInPath();           
        pathFinded = true;                    //Once the path is finished, the movement script will be enables
    }

    //This function reduces te nodes at minimum for avoiding the stair effect
    public List<Node> TargetsInPath()
    {
        List<Node> newPath = new List<Node>();
        List<Node> auxnode = new List<Node>();

        //The initial and final node are added 
        newPath.Add(GridReference.FinalPath[0]);

        for (int i = 1; i < GridReference.FinalPath.Count; i++)
        {
            //It tests if both, x and y have changed in comparison with their previous values
            if (GridReference.FinalPath[i].iGridX != newPath[newPath.Count - 1].iGridX && GridReference.FinalPath[i].iGridY != newPath[newPath.Count - 1].iGridY)
            {
                newPath.Add(GridReference.FinalPath[i - 1]);
            }
        }
        newPath.Add(GridReference.FinalPath[GridReference.FinalPath.Count - 1]);    //The final point is added
        /*
        for (int i = 1; i < newPath.Count - 1; i++)                     //We don't want to remove the initial and final nodes
        {
            //It tests if both, x and y have changed in comparison with their previous values
            if (newPath[i + 1].iGridX != newPath[i - 1].iGridX && newPath[i + 1].iGridY != newPath[i - 1].iGridY)
            {
                //It testes if the path has changed its direction, because there was a problem at some time with, being possible to cross the object by its middle
                if (Mathf.Abs(newPath[i + 1].iGridX - newPath[i - 1].iGridX) > 0 && Mathf.Abs(newPath[i + 1].iGridY - newPath[i - 1].iGridY) > 0)   //If it's 0, the path will be changing its direction
                {
                    //It is for deletting the stair effect with a maximum range of 3 nodes of distance
                    if ((Mathf.Abs(newPath[i + 1].iGridX - newPath[i - 1].iGridX) <= 3 && Mathf.Abs(newPath[i + 1].iGridY - newPath[i - 1].iGridY) <= 3))
                    {
                        //The initial node can't be removed
                        if (newPath[i - 1] != GridReference.FinalPath[0] && i > 1)   //*****aqui solo con poner i>0 creo que valdría*****
                        {
                            //This is done for not having trouble with a very long diagonal, as there was a porblem with the inital point in the stair sequence
                            if ((Mathf.Abs(newPath[i - 1].iGridX - newPath[i - 2].iGridX) <= 3 && Mathf.Abs(newPath[i - 1].iGridY - newPath[i - 2].iGridY) <= 3))
                            {
                                auxnode.Add(newPath[i - 1]);
                            }
                        }
                        auxnode.Add(newPath[i]);
                    }
                }
            }
        }
        int j = 0;
        //After having gone trough the whole path, the nodes are erased
        while (j < auxnode.Count)
        {
            newPath.Remove(auxnode[j]);
            j++;
        }*/
        return newPath;
    }
    private float GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        float ix = Mathf.Abs(a_nodeA.iGridX - a_nodeB.iGridX);//x1-x2
        float iy = Mathf.Abs(a_nodeA.iGridY - a_nodeB.iGridY);//y1-y2

        return ix + iy;//Return the sum
    }
}