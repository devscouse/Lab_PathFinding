using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public Node[,] grid;
    private Vector3 worldAnchor;
    private Vector3 worldSize;

    private int gridWidth;
    private int gridHeight;

    public float nodeRadius;
    public float nodeDiameter;

    public Material mazeMaterial;

    Vector2Int ToPixelPos(Vector3 worldPos, Texture2D tex)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        float normX = -localPos.x / transform.localScale.x + 0.5f;
        float normY = -localPos.z / transform.localScale.z + 0.5f;

        int pixelX = Mathf.RoundToInt(Mathf.Clamp(normX * tex.width, 0, tex.width));
        int pixelY = Mathf.RoundToInt(Mathf.Clamp(normY * tex.height, 0, tex.height));
        return new Vector2Int(pixelX, pixelY);
    }


    float SampleObstacleTexture(Vector3 worldPos)
    {
        Texture2D tex = (Texture2D)mazeMaterial.mainTexture;
        Vector2Int centerPixelPos = ToPixelPos(worldPos, tex);

        int[,] offsets = {
            {1, 1},
            {1, -1},
            {-1, 1},
            {-1, -1},
        };

        // Average the pixel values for the node
        List<float> values = new();
        for (int x = 0; x < nodeRadius; x++)
        {
            for (int y = 0; y < nodeRadius; y++)
            {
                if (x * x + y * y > nodeRadius * nodeRadius)
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    float value = tex.GetPixel(centerPixelPos.x + (x * offsets[i, 0]), centerPixelPos.y + (y * offsets[i, 1])).grayscale;
                    values.Add(value);
                }
            }
        }

        float valueSum = 0;
        foreach (float val in values) { valueSum += val; }
        return valueSum / values.Count;
    }

    public Pos GetGridPos(Vector3 worldPos)
    {
        float pctX = Mathf.Clamp01((worldPos.x - worldAnchor.x) / worldSize.x);
        float pctY = Mathf.Clamp01((worldPos.z - worldAnchor.z) / worldSize.z);
        Pos gridPos = new(Mathf.RoundToInt((gridWidth - 1) * pctX), Mathf.RoundToInt((gridHeight - 1) * pctY));
        Debug.Log($"{worldPos} => ({pctX}%, {pctY}%) => {gridPos}");
        return gridPos;
    }

    public void CreateNodeGrid(float nodeSize)
    {
        worldSize = GetComponent<MeshRenderer>().bounds.size;
        nodeRadius = nodeSize;
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
}
