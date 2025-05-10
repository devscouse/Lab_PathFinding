using UnityEngine;

public class MazeImageLoader : MonoBehaviour
{

    public Material mazeMaterial;

    public void LoadImageToTexture(Texture2D mazeTexture)
    {
        mazeMaterial.mainTexture = mazeTexture;
    }

}
