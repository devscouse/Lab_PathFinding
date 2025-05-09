using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public bool mazeSolved = false;

    private float startDelay;
    private float stepDelay;
    public Maze maze;

    private LinkedList<Node> openNodes;
    private Pos currPos;
    private int[,] posOffsets = {
        {-1, -1},
        {-1, 1},
        {-1, 0},
        {1, -1},
        {1, 1},
        {1, 0},
        {0, -1},
        {0, 1},
    };
    private int maxF = 100;

    private struct Node
    {
        public int g;
        public int h;
        public int f;
        public Pos pos;

        public Node(int g, int h, Pos pos)
        {
            this.g = g;
            this.h = h;
            this.pos = pos;
            this.f = g + h;
        }
        public override readonly string ToString() => $"(pos={pos}, g={g}, h={h}, f={f})";
        public readonly Color GetColor(int maxF)
        {
            return new Color(f / Math.Max(maxF, f), 0, 0);
        }

    }

    public PathFinder(Maze maze, float startDelay, float stepDelay)
    {
        this.maze = maze;
        this.startDelay = startDelay;
        this.stepDelay = stepDelay;
        this.openNodes = new LinkedList<Node>();
        this.currPos = maze.startPos;
    }


    public void AStarStep()
    {
        if (currPos.Equals(maze.targetPos)) { mazeSolved = true; }

        // Calculate valid nodes around current position and add to openNodes
        Pos nodePos;
        Node node;
        GridSquare gs;

        gs = maze.GetGridSquare(currPos);
        if (!gs.CheckStatus(GridSquare.Status.Start)) { gs.SetStatus(GridSquare.Status.Closed); }

        for (int i = 0; i < 8; i++)
        {
            nodePos = new(currPos.y + posOffsets[i, 0], currPos.x + posOffsets[i, 1]);
            if (nodePos.x < 0 || nodePos.y < 0 || nodePos.x >= maze.width || nodePos.y >= maze.height)
            {
                continue;
            }
            gs = maze.GetGridSquare(nodePos);
            if (gs.CheckStatus(GridSquare.Status.Target))
            {
                Debug.Log("Maze is solved!");
                mazeSolved = true;
                return;
            }
            else if (!gs.CheckStatus(GridSquare.Status.Traversable))
            {
                continue;
            }
            node = new(CalculateG(nodePos, currPos), CalculateH(nodePos), nodePos);
            gs.SetStatus(GridSquare.Status.Open, node.GetColor(maxF));

            // Insert the new node into openNodes before the first node with a greater f score
            LinkedListNode<Node> other = openNodes.First;
            while (true)
            {
                if (other == null)
                {
                    openNodes.AddLast(node);
                    break;
                }

                // Put the node before the first node with a higher F cost
                // If there is a node with an identical F cost then the node with the lowest
                // H cost should come first
                if (node.f < other.Value.f || (node.f == other.Value.f && node.h < other.Value.h))
                {
                    openNodes.AddBefore(other, node);
                    break;
                }
                other = other.Next;

            }
        }
        if (openNodes.Count == 0) { mazeSolved = true; }


        // Move current position to best node
        currPos = openNodes.First.Value.pos;
        maxF = openNodes.First.Value.f;
        openNodes.RemoveFirst();

    }

    int CalculateG(Pos nodePos, Pos currPos)
    {
        int xDiff = nodePos.x - currPos.x;
        int yDiff = nodePos.y - currPos.y;
        return (xDiff * xDiff) + (yDiff * yDiff);
    }

    int CalculateH(Pos nodePos)
    {
        int xDiff = nodePos.x - maze.targetPos.x;
        int yDiff = nodePos.y - maze.targetPos.y;
        return (xDiff * xDiff) + (yDiff * yDiff);
    }

    public IEnumerator PathFinderRoutine(Action PathFinderStepFunc)
    {
        yield return new WaitForSeconds(startDelay);
        while (!mazeSolved)
        {
            yield return new WaitForSeconds(stepDelay);
            PathFinderStepFunc();
        }

    }


}
