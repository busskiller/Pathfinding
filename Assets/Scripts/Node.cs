using UnityEngine;
using System.Collections;

public class Node
{

    public Node parent;

    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;


    public int gCost; //distance to start node
    public int hCost; //distance to target node

    public int fCost;
    public int OGcost;

    public void nodeInitialization() {

        gCost = (int)Random.Range(1, 5);
        Debug.Log(gCost);
        OGcost = gCost;
    }




    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;

        gridX = _gridX;
        gridY = _gridY;
    }

}