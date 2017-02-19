﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public List<Node> Path;
    public Transform Player;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;

    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
    }


    void CreateGrid()
    {
        grid = new Node[gridSizeX,gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right*gridWorldSize.x/2 - Vector3.up*gridWorldSize.y/2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right*(x*nodeDiameter + nodeRadius) +
                                     Vector3.up*(y*nodeDiameter + nodeRadius);

                //If the is a collision, set walkable to false
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid [x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(Player.position);

            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;

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

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter -.1f));
                
            }
        }

    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {

        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2 ) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt( (gridSizeX - 1) * percentX );
        int y = Mathf.RoundToInt( (gridSizeY - 1) * percentY);

        return grid [x,y];
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
                    neighbours.Add(grid[checkX,checkY]);
                }

            }
        }
        return neighbours;
    }


}