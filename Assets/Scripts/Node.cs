using UnityEngine;

public class Node
{
    public enum Status
    {
        open,
        closed,
        traversable,
        blocked,
        path,
    };
    public float traversalCost;
    public int hCost;
    public int gCost;
    public Vector3 worldPos;
    public Pos gridPos;
    public Status status;
    public Node parent;

    public Node(Vector3 worldPos, float traversalCost, float maxTraversalCost)
    {
        this.traversalCost = traversalCost;
        this.worldPos = worldPos;
        if (this.traversalCost <= maxTraversalCost) { status = Status.traversable; }
        else { status = Status.blocked; }
    }

    public int FCost() { return gCost + hCost; }
}
