using System.Collections.Generic;
using UnityEngine;

public class MapTextureHandler : MonoBehaviour
{

    public Material mapMaterial;
    public Vector2Int textureSize;

    void Start()
    {
        ResetTexture();
    }

    public void ResetTexture()
    {
        mapMaterial.color = Color.white;
        mapMaterial.mainTexture = new Texture2D(textureSize.x, textureSize.y);
    }

    public void LoadTexture(Texture2D mazeTexture)
    {
        textureSize.x = mazeTexture.width;
        textureSize.y = mazeTexture.height;
        mapMaterial.mainTexture = mazeTexture;
    }

    public Texture2D GetTexture()
    {
        Texture2D tex = (Texture2D)mapMaterial.mainTexture;
        return tex;
    }

    public void SetPixel(int x, int y, Color c)
    {
        if (x < 0 || x >= textureSize.x || y < 0 || y >= textureSize.y)
            return;

        Texture2D tex = GetTexture();
        tex.SetPixel(x, y, c);
        tex.Apply();
        LoadTexture(tex);
    }
    public void SetPixel(Vector2Int pixelPos, Color c) { SetPixel(pixelPos.x, pixelPos.y, c); }
    public void SetPixel(Vector3 worldPos, Color c) { SetPixel(ToPixelPos(worldPos), c); }

    public Vector2Int ToPixelPos(Vector3 worldPos)
    {
        Texture2D tex = GetTexture();
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        float normX = -localPos.x / transform.localScale.x + 0.5f;
        float normY = -localPos.z / transform.localScale.z + 0.5f;

        int pixelX = Mathf.RoundToInt(Mathf.Clamp(normX * tex.width, 0, tex.width));
        int pixelY = Mathf.RoundToInt(Mathf.Clamp(normY * tex.height, 0, tex.height));
        return new Vector2Int(pixelX, pixelY);
    }

    public List<Vector2Int> GetPixelPointsAroundWorldPos(Vector3 worldPos, int pixelRadius)
    {
        List<Vector2Int> points = new();
        Vector2Int pixelPos = ToPixelPos(worldPos);

        int x;
        int y;
        int[,] offsets = {
            {1, 1},
            {-1, 1},
            {1, -1},
            {-1, -1},
        };
        for (int dx = 0; dx < pixelRadius; dx++)
        {
            for (int dy = 0; dy < pixelRadius; dy++)
            {
                if (dx * dx + dy * dy >= (pixelRadius * pixelRadius))
                    continue;

                for (int i = 0; i < 4; i++)
                {
                    x = pixelPos.x + (offsets[i, 0] * dx);
                    y = pixelPos.y + (offsets[i, 1] * dy);
                    if (x < 0 || x >= textureSize.x || y < 0 || y >= textureSize.y)
                        continue;
                    points.Add(new Vector2Int(x, y));
                }
            }
        }
        return points;
    }

    public void SetPixelsAroundWorldPos(Vector3 worldPos, int pixelRadius, Color c)
    {
        List<Vector2Int> pixelPoints = GetPixelPointsAroundWorldPos(worldPos, pixelRadius);
        Texture2D tex = GetTexture();

        foreach (Vector2Int pos in pixelPoints)
        {
            tex.SetPixel(pos.x, pos.y, c);
        }
        tex.Apply();
        LoadTexture(tex);
    }

}
