using UnityEngine;
using System.Collections;

public class Node
{

    public Node parent;

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;


    public int gCost; //My own cost that i specify
    public int hCost; //distance from node to the target node

    public int fCost; //the total cost i.e. hCost+gCost of the node
    public int OriginalGcost; //Stores the orignal gCost

    public void nodeInitialization() {

        gCost = (int)Random.Range(1, 5);
        //Debug.Log(gCost);
        OriginalGcost = gCost;
    }




    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;

        gridX = _gridX;
        gridY = _gridY;
    }

}