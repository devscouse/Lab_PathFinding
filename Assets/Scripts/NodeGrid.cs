using System;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    private Node[,] grid;
    private Vector3 worldAnchor;
    private Vector3 worldSize;

    private int gridWidth;
    private int gridHeight;

    public float nodeRadius;
    private float nodeDiameter;

    public Material mazeMaterial;

    float SampleObstacleTexture(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        float normX = -localPos.x / transform.localScale.x + 0.5f;
        float normY = -localPos.z / transform.localScale.z + 0.5f;

        Texture2D obstacleTexture = (Texture2D)mazeMaterial.mainTexture;
        int pixelX = Mathf.RoundToInt(Mathf.Clamp(normX * obstacleTexture.width, 0, obstacleTexture.width));
        int pixelY = Mathf.RoundToInt(Mathf.Clamp(normY * obstacleTexture.height, 0, obstacleTexture.height));

        float value = obstacleTexture.GetPixel(pixelX, pixelY).grayscale;
        return value;
    }

    public Pos GetGridPos(Vector3 worldPos)
    {
        float pctX = Mathf.Clamp01((worldPos.x - worldAnchor.x) / worldSize.x);
        float pctY = Mathf.Clamp01((worldPos.z - worldAnchor.z) / worldSize.z);
        Pos gridPos = new(Mathf.RoundToInt((gridWidth - 1) * pctX), Mathf.RoundToInt((gridHeight - 1) * pctY));
        Debug.Log($"{worldPos} => ({pctX}%, {pctY}%) => {gridPos}");
        return gridPos;
    }

    public void CreateNodeGrid()
    {
        worldSize = GetComponent<MeshRenderer>().bounds.size;
        nodeDiameter = nodeRadius * 2;

        gridWidth = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        gridHeight = Mathf.RoundToInt(worldSize.z / nodeDiameter);
        grid = new Node[gridHeight, gridWidth];

        worldAnchor = transform.position - (Vector3.right * worldSize.x / 2) - (Vector3.forward * worldSize.z / 2);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector3 nodePos = (
                    worldAnchor
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.forward * (y * nodeDiameter + nodeRadius)
                );

                Node node = new(nodePos, 1 - SampleObstacleTexture(nodePos), 0.7f);
                node.gridPos = new Pos(x, y);
                grid[y, x] = node;
            }
        }
    }

    public Node GetNode(int x, int y) { return grid[Math.Clamp(y, 0, gridHeight), Math.Clamp(x, 0, gridWidth)]; }
    public Node GetNode(Pos p) { return GetNode(p.x, p.y); }
    public Node GetNode(Vector3 worldPos) { return GetNode(GetGridPos(worldPos)); }

    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        foreach (Node node in grid)
        {
            if (node.status == Node.Status.blocked || node.status == Node.Status.traversable)
            {
                continue;
            }
            var c = node.status switch
            {
                Node.Status.open => Color.yellow,
                Node.Status.closed => Color.red,
                Node.Status.path => Color.blue,
                _ => Color.black,
            };
            Gizmos.color = c;
            Gizmos.DrawCube(node.worldPos, new Vector3(nodeDiameter, 1, nodeDiameter) * .9f);
        }

    }
}
