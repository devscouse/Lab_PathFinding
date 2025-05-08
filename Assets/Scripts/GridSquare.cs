using UnityEngine;

public class GridSquare : MonoBehaviour
{
    private MeshRenderer rend;
    public Color traversableColor;
    public Color traversedColor;
    public Color blockedColor;
    public Color startColor;
    public Color targetColor;
    public Color openColor;
    public Color closedColor;

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        SetTraversable();
        CheckMaterial();
    }
    public bool IsTraversable() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == traversableColor; }
    public bool IsBlocked() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == blockedColor; }
    public bool IsStart() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == startColor; }
    public bool IsTarget() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == targetColor; }
    public bool IsOpen() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == openColor; }
    public bool IsClosed() { return GetComponent<MeshRenderer>().material.GetColor("_BaseColor") == closedColor; }
    private void CheckMaterial()
    {
        if (GetComponent<MeshRenderer>().material == null)
        {
            Debug.Log(this + " does not have a GetComponent<MeshRenderer>().material set");
        }
    }
    public void SetTraversable() { GetComponent<MeshRenderer>().material.SetColor("_BaseColor", traversableColor); }
    public void SetBlocked() { GetComponent<MeshRenderer>().material.SetColor("_BaseColor", blockedColor); }
    public void SetStart() { CheckMaterial(); GetComponent<MeshRenderer>().material.SetColor("_BaseColor", startColor); }
    public void SetTarget() { CheckMaterial(); GetComponent<MeshRenderer>().material.SetColor("_BaseColor", targetColor); }
    public void SetOpen() { CheckMaterial(); GetComponent<MeshRenderer>().material.SetColor("_BaseColor", openColor); }
    public void SetClosed() { CheckMaterial(); GetComponent<MeshRenderer>().material.SetColor("_BaseColor", closedColor); }

}
