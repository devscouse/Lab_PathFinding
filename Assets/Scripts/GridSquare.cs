using System.Collections.Generic;
using UnityEngine;

public class GridSquare : MonoBehaviour
{
    public enum Status
    {
        Blocked,
        Closed,
        Open,
        Start,
        Target,
        Traversable,
    };
    public Dictionary<Status, Color> statusColor = new Dictionary<Status, Color>
        {
            { Status.Blocked, Color.black },
            { Status.Closed, Color.yellow },
            { Status.Open, Color.yellow },
            { Status.Target, Color.green },
            { Status.Start, Color.blue },
            { Status.Traversable, new Color(0.3f, 0.3f, 0.3f) }
        };
    public Status status;

    void Start()
    {
        CheckMaterial();
    }

    public void SetBaseColor(Color c)
    {
        CheckMaterial();
        GetComponent<MeshRenderer>().material.SetColor("_BaseColor", c);
    }
    public bool CheckStatus(Status s) { return status == s; }
    public void SetStatus(Status s, Color c)
    {
        status = s;
        SetBaseColor(c);
    }
    public void SetStatus(Status s)
    {
        Debug.Log("statusColor: " + statusColor);
        SetStatus(s, statusColor[s]);
    }
    public void SetLightIntensity(float intensity)
    {
        gameObject.transform.Translate(Vector3.up * 0.1f);
        GetComponent<Light>().intensity = intensity;
    }

    private void CheckMaterial()
    {
        if (GetComponent<MeshRenderer>().material == null)
        {
            Debug.Log(this + " does not have a GetComponent<MeshRenderer>().material set");
        }
    }
}
