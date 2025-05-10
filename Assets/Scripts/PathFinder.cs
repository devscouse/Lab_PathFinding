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
        openNodes = new();
    }

    public void Step()
    {
        Debug.Log("PathFinder.Step from node at position " + currNode.gridPos);

        // Evaluate the 8 nodes around the currNode
        Node node;
        for (int i = 0; i < 8; i++)
        {
            node = grid.GetNode(currNode.gridPos.x + posOffsets[i, 0], currNode.gridPos.y + posOffsets[i, 1]);

            // If the node is closed or cannot be traversed then continue
            if (node.status == Node.Status.closed || node.status == Node.Status.blocked) { continue; }

            // If the node is the target node we have solved the maze
            if (node == targetNode)
            {
                finished = true;
                solved = true;
                SetPathTaken(node);
                return;
            }

            // Add to the openNodes
            node.status = Node.Status.open;
            int gCost = node.gCost + GetDistance(node, node);
            if (gCost < node.gCost || !openNodes.Contains(node))
            {
                node.gCost = gCost;
                node.hCost = GetDistance(node, targetNode);
                node.parent = currNode;

                if (!openNodes.Contains(node))
                {
                    openNodes.Add(node);
                }
            }
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
        openNodes.Remove(currNode);
    }

    void SetPathTaken(Node endNode)
    {
        Node node = endNode;
        while (node != null)
        {
            node.status = Node.Status.path;
            node = node.parent;
        }
    }

    int GetDistance(Node self, Node other)
    {
        int distX = Math.Abs(self.gridPos.x - other.gridPos.y);
        int distY = Math.Abs(self.gridPos.y - other.gridPos.y);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

}
