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
    private float nodeDiameter;

    public int GetGridWidth() { return gridWidth; }
    public int GetGridHeight() { return gridHeight; }

    float SampleObstacleTexture(Vector3 worldPos)
    {
        MapTextureHandler mapTex = GetComponent<MapTextureHandler>();
        Texture2D tex = mapTex.GetTexture();
        List<Vector2Int> pixelPoints = mapTex.GetPixelPointsAroundWorldPos(worldPos, (int)(nodeRadius * 10));
        List<float> values = new();
        foreach (Vector2Int pixelPos in pixelPoints)
        {
            values.Add(tex.GetPixel(pixelPos.x, pixelPos.y).grayscale);
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
        Debug.Log($"CreateNodeGrid({nodeSize})");
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
        Debug.Log($"CreateNodeGrid({nodeSize}) complete");
    }

    public Node GetNode(int x, int y) { return grid[Math.Clamp(y, 0, gridHeight), Math.Clamp(x, 0, gridWidth)]; }
    public Node GetNode(Pos p) { return GetNode(p.x, p.y); }
    public Node GetNode(Vector3 worldPos) { return GetNode(GetGridPos(worldPos)); }
}
