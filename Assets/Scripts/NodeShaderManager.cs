using UnityEngine;

public class NodeShaderManager : MonoBehaviour
{
    public NodeGrid nodeGrid;
    public Material vizMaterial;
    public Material debugNodeInfoMaterial;

    // Each pixel encodes information on a node
    // R: isPath (0 or 1)
    // A: exploredTime
    private Texture2D nodeInfoTex;


    void Start()
    {
        vizMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        vizMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        vizMaterial.SetInt("_ZWrite", 1);
        vizMaterial.EnableKeyword("_ALPHATEST_ON");
        vizMaterial.DisableKeyword("_ALPHABLEND_ON");
        vizMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        vizMaterial.renderQueue = 2450;
    }

    void Update()
    {
        vizMaterial.SetFloat("_CurrTime", Time.time);
    }

    public void ResetTextures()
    {
        if (nodeGrid.grid == null)
            return;

        int width = nodeGrid.GetGridWidth();
        int height = nodeGrid.GetGridHeight();
        Debug.Log($"Creating Node textures with dimensions ({width}, {height})");

        nodeInfoTex = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };


        Color defaultColor = new(0, 0, 0, 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pixelPos = ToPixelPos(new Pos(x, y));
                nodeInfoTex.SetPixel(pixelPos.x, pixelPos.y, defaultColor);
            }
        }
        nodeInfoTex.Apply();
        vizMaterial.SetTexture("_NodeInfoTex", nodeInfoTex);
        vizMaterial.SetFloat("_FadeTime", 10f);
    }

    public void SetNodeInfoTexPixel(Node node, Color color)
    {

    }

    public Vector2Int ToPixelPos(Pos nodeGridPos)
    {
        return new Vector2Int(nodeGrid.GetGridWidth() - nodeGridPos.x - 1, nodeGrid.GetGridHeight() - nodeGridPos.y - 1);
    }

    public void UpdateNodeTextures(Node node)
    {
        Debug.Log("Updating node textures with last explored node " + node.gridPos + " status=" + node.status);
        Vector2Int pixelPos = ToPixelPos(node.gridPos);
        float isPath = node.status == Node.Status.path ? 1 : 0;
        nodeInfoTex.SetPixel(pixelPos.x, pixelPos.y, new Color(isPath, 0, 0, Mathf.Clamp01(node.timeExplored / 255f)));
        Debug.Log(nodeInfoTex.GetPixel(pixelPos.x, pixelPos.y));
        nodeInfoTex.Apply();
        debugNodeInfoMaterial.mainTexture = nodeInfoTex;

        vizMaterial.SetTexture("_NodeInfoTex", nodeInfoTex);
        vizMaterial.SetFloat("_CurrTime", Time.time);

        if (vizMaterial.GetTexture("_NodeInfoTex") == null)
            Debug.LogError("NodeInfo texture is not assigned to vizMaterial");

    }

    public void UpdateNodeTextures()
    {
        Node node = nodeGrid.lastExploredNode;
        UpdateNodeTextures(node);
    }

}
