using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{

    private readonly NodeGrid grid;
    private readonly Node targetNode;
    private Node currNode;

    private readonly List<Node> openNodes;
    private readonly int[,] posOffsets = {
        {-1, -1},
        {-1, 0},
        {-1, 1},
        {0, -1},
        {0, 0},
        {0, 1},
        {1, -1},
        {1, 0},
        {1, -1},
    };

    public bool finished;
    public bool solved;

    public PathFinder(NodeGrid grid, Vector3 startPos, Vector3 targetPos)
    {
        this.grid = grid;
        currNode = grid.GetNode(startPos);
        targetNode = grid.GetNode(targetPos);
        Debug.Log("PathFinder.targetNode at position " + targetNode.gridPos);
        openNodes = new();
    }

    public void Step()
    {
        Debug.Log("PathFinder.Step from node at position " + currNode.gridPos);

        // Evaluate the 8 nodes around the currNode
        Node node;
        int x, y;
        for (int i = 0; i < 8; i++)
        {
            Debug.Log("Evaluating neighbour " + i);
            x = currNode.gridPos.x + posOffsets[i, 0];
            y = currNode.gridPos.y + posOffsets[i, 1];
            if (x < 0 || x >= grid.GetGridWidth() || y < 0 || y >= grid.GetGridHeight())
            {
                Debug.Log("Neighbour off grid");
                continue;
            }
            node = grid.GetNode(x, y);

            // If the node is closed or cannot be traversed then continue
            if (node.status == Node.Status.closed || node.status == Node.Status.blocked) { continue; }


            // Add to the openNodes
            node.status = Node.Status.open;
            int gCost = currNode.gCost + GetManhattanDistance(currNode, node);
            if (gCost < node.gCost || !openNodes.Contains(node))
            {
                node.gCost = gCost;
                node.hCost = GetManhattanDistance(node, targetNode);
                node.parent = currNode;

                if (!openNodes.Contains(node))
                {
                    openNodes.Add(node);
                }
            }
            Debug.Log($"Node at {node.gridPos} has hCost {node.hCost}, gCost {node.gCost}, fCost {node.FCost()}");
        }

        // If we have no open nodes to explore the maze is unsolvable
        if (openNodes.Count == 0)
        {
            finished = true;
            return;
        }

        // Select the node with the lowest FCost
        currNode.status = Node.Status.closed;
        currNode = openNodes[0];
        for (int i = 1; i < openNodes.Count; i++)
        {
            if (openNodes[i].FCost() <= currNode.FCost() && openNodes[i].hCost < currNode.hCost)
            {
                currNode = openNodes[i];
            }
        }
        // If the node is the target node we have solved the maze
        if (currNode == targetNode)
        {
            finished = true;
            solved = true;
            SetPathTaken(currNode);
            return;
        }
        openNodes.Remove(currNode);
    }

    void SetPathTaken(Node endNode)
    {
        Debug.Log("Backtracking for path-taken to get to " + endNode);
        Node node = endNode;
        Debug.Log("endNode " + node);
        Debug.Log("endNode.parent " + node.parent);
        node.status = Node.Status.path;

        // while (node != null)
        // {
        //     node.status = Node.Status.path;
        //     node = node.parent;
        // }
    }

    int GetSebastianDistance(Node self, Node other)
    {
        int distX = Math.Abs(self.gridPos.x - other.gridPos.x);
        int distY = Math.Abs(self.gridPos.y - other.gridPos.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    int GetManhattanDistance(Node self, Node other)
    {
        return Math.Abs(self.gridPos.x - other.gridPos.x) + Math.Abs(self.gridPos.y - other.gridPos.y);
    }

}
