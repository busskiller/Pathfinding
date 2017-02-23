using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Pathfinding : MonoBehaviour
{

    public Transform StartNode;
    public Transform TargetNode;

    private Node startNode;
    private Node targetNode;



    private Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }


    private bool Finished = false;

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
                FindPath(StartNode.position, TargetNode.position);
                StopAllCoroutines();
            }

        }




        if (Finished == true)
        {
            DrawPath(startNode, targetNode, 3);

            StartCoroutine(FollowPath(1));

            Finished = false;

        }

    }


    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        startNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

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

                Finished = true;

                //DrawPath(startNode, targetNode);
                return;
            }


            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {

                int newGcostToNeighbour = currentNode.OriginalGcost + neighbour.OriginalGcost;
                int newFcostToNeighbour = newGcostToNeighbour + neighbour.hCost;


                if (!openSet.Contains(neighbour) && !closedSet.Contains(neighbour))
                {
                    neighbour.gCost = newGcostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.fCost = newGcostToNeighbour + neighbour.hCost;

                    openSet.Add(neighbour);

                    neighbour.parent = currentNode;
                }
                else if (openSet.Contains(neighbour) && !closedSet.Contains(neighbour))
                {

                    if (neighbour.fCost < newFcostToNeighbour)
                    {
                        neighbour.gCost = newGcostToNeighbour;
                        neighbour.fCost = newFcostToNeighbour;

                        neighbour.parent = null;

                        neighbour.parent = currentNode;

                    }

                }

            }

        }

    }


    private List<Node> path;


    private IEnumerator FollowPath(int delay)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 tmp = new Vector3(1, 1, -1);
            Vector3 startPos = StartNode.transform.position;

            tmp.x = path[i].worldPosition.x;
            tmp.y = path[i].worldPosition.y;

            //float step = delay * Time.deltaTime;

            float t = 0;

            while (t < delay)
            {
                t += Time.deltaTime;
                StartNode.transform.position = Vector3.Lerp(startPos, tmp, t);
                yield return null;

                //print("Stage 2");
            }


            //print(path[i].worldPosition);

            //StartNode.transform.position = tmp;

            //yield return new WaitUntil(() => tmp == StartNode.transform.position);

            //float step = delay * Time.deltaTime;
            //StartNode.transform.position = Vector3.MoveTowards(transform.position, tmp, 1);

            //yield return new WaitForSeconds(t);
        }
    }


    private void DrawPath(Node startNode, Node endNode, int delay)
    {
        path = null;

        if (path != null)
        {
            Debug.Log("not empty");
        }

        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.Path = path;
    }


    private int GetDistance(Node nodeA, Node nodeB)
    {
        return Mathf.Abs(nodeA.gridX - nodeB.gridX) + Mathf.Abs(nodeA.gridY - nodeB.gridY);
    }


}