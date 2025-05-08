using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private MazeGenerator mazeGenerator;
    public float startDelay;
    public float stepDelay;

    private bool mazeSolved = false;
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

    }

    void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
    }


    void AStarStep()
    {
        // Calculate valid nodes around current position and add to openNodes
        Pos nodePos;
        Node node;
        GridSquare gs;

        for (int i = 0; i < posOffsets.Length; i++)
        {
            nodePos = new(currPos.y + posOffsets[i, 0], currPos.x + posOffsets[i, 1]);
            gs = mazeGenerator.GetGridSquare(nodePos.y, nodePos.x);
            if (!gs.IsTraversable())
            {
                continue;
            }
            gs.SetOpen();
            node = new(CalculateG(nodePos), CalculateH(nodePos), nodePos);

            // Insert the new node into openNodes before the first node with a greater f score
            LinkedListNode<Node> other = openNodes.First;
            while (true)
            {
                if (other != null)
                {
                    openNodes.AddLast(node);
                    break;
                }

                if (node.f > other.Value.f)
                {
                    openNodes.AddBefore(other, node);
                    break;
                }
                other = other.Next;

            }
        }

        gs = mazeGenerator.GetGridSquare(currPos.y, currPos.x);
        if (!gs.IsStart()) { gs.SetClosed(); }

        // Move current position to best node
        currPos = openNodes.First.Value.pos;
        openNodes.RemoveFirst();
    }

    int CalculateG(Pos nodePos)
    {
        int xDiff = nodePos.x - mazeGenerator.startPos.x;
        int yDiff = nodePos.y - mazeGenerator.startPos.y;
        return (xDiff * xDiff) + (yDiff * yDiff);
    }

    int CalculateH(Pos nodePos)
    {
        int xDiff = nodePos.x - mazeGenerator.targetPos.x;
        int yDiff = nodePos.y - mazeGenerator.targetPos.y;
        return (xDiff * xDiff) + (yDiff * yDiff);
    }

    public void FindPath()
    {
        openNodes = new LinkedList<Node>();
        currPos = mazeGenerator.startPos;
        StartCoroutine(PathFinderRoutine(AStarStep));
    }

    IEnumerator PathFinderRoutine(Action PathFinderStepFunc)
    {
        yield return new WaitForSeconds(startDelay);
        while (!mazeSolved)
        {
            yield return new WaitForSeconds(stepDelay);
            PathFinderStepFunc();
        }

    }


}
