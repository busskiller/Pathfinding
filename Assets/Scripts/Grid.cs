using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public List<Node> Path;
    public Transform Player;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    public Node[,] grid;
    private float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }


    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
                                     Vector3.up * (y * nodeDiameter + nodeRadius);

                //If the is a collision, set walkable to false
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
                grid[x, y].nodeInitialization();

            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 tmp = transform.position;
        tmp.z = -10;

        Gizmos.DrawWireCube(tmp, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(Player.position);

            foreach (Node n in grid)
            {

                if (n.OriginalGcost == 1) {
                    Gizmos.color = Color.green;
                }
                if (n.OriginalGcost == 2)
                {
                    Gizmos.color = new Color(0, .75f, 0);
                }
                if (n.OriginalGcost == 3)
                {
                    Gizmos.color = Color.yellow;
                }
                if (n.OriginalGcost == 4)
                {
                    Gizmos.color = Color.red;
                }

                //Gizmos.color = (n.walkable) ? Color.white : Color.red;

                if (Path != null)
                {
                    if (Path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }

                if (playerNode == n)
                {
                    //Gizmos.color = Color.green;
                }

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));

            }
        }

    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {

        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }

            }
        }
        return neighbours;
    }


}