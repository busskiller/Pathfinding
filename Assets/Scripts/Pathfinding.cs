using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Pathfinding : MonoBehaviour
{

    public Transform StartNode;
    public Transform TargetNode;


    private Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 point = hit.point;
                TargetNode.transform.position = point;
            }

        }

        FindPath(StartNode.position, TargetNode.position);
    }


    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                grid.grid[x, y].gCost = grid.grid[x, y].OGcost;
            }
        }


        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {

            Node currentNode = openSet[0];

            for (int i = 0; i < openSet.Count; i++)
            {

                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }

            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                DrawPath(startNode, targetNode);
                return;
            }


            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {

                //int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);


                //int newMovementCostToNeighbour = currentNode  neighbour.gCost + GetDistance(targetNode, neighbour);

                if (!openSet.Contains(neighbour) && !closedSet.Contains(neighbour))
                {
                    neighbour.gCost = currentNode.gCost + neighbour.gCost;
                    neighbour.fCost = neighbour.gCost + GetDistance(targetNode, neighbour);
                    openSet.Add(neighbour);


                    //neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                }

            }


        }


    }


    private void DrawPath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        for (int i = 0; i < path.Count; i++)
        {

            Vector3 tmp = new Vector3(1, 1, -1);
            tmp.x = path[i].worldPosition.x;
            tmp.y = path[i].worldPosition.y;

            StartNode.transform.position = tmp;

        }

        path.Reverse();

        grid.Path = path;
    }



    //14 min. in video. Dont quite get it.
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }


}