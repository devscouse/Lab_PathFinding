using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject map;
    public PathFinder pathFinder;
    public Texture2D mazeImage;
    public UI ui;
    public GameObject startFlag;
    public GameObject targetFlag;

    public float startDelay;
    public float stepDelay;
    public float costScale;
    private float nodeSize = 0.5f;

    private MapTextureHandler mapTex;
    private NodeGrid nodeGrid;
    private NodeShaderManager nodeShader;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float lastInputTime;
    private bool acceptingStartInput;
    private bool acceptingTargetInput;
    private bool simulationRunning;

    private int brushSize;
    private Color brushColor;
    private bool acceptingDrawInput;

    void Start()
    {
        mapTex = map.GetComponent<MapTextureHandler>();
        SetTextureFromMapSelector();
        nodeShader = GetComponent<NodeShaderManager>();
    }

    public void StartSimulation()
    {
        nodeGrid = map.GetComponent<NodeGrid>();
        nodeGrid.CreateNodeGrid(nodeSize);
        nodeShader.ResetTextures();

        pathFinder = new PathFinder(nodeGrid, startPos, targetPos);
        pathFinder.NodeExplored += nodeShader.UpdateNodeTextures;
        simulationRunning = true;
        ui.stopStartButton.GetComponent<Image>().color = Color.red;
        ui.stopStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
        StartCoroutine(PathFinderRoutine(pathFinder));
    }

    public void StopSimulation()
    {
        simulationRunning = false;
        ui.stopStartButton.GetComponent<Image>().color = Color.green;
        ui.stopStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
    }

    private void SetTextureFromMapSelector()
    {
        if (ui.mapSelector.value == 1)
        {
            Debug.Log("Loading Maze texture...");
            mapTex.LoadTexture(mazeImage);
        }
        else
        {
            Debug.Log("Resetting map texture...");
            mapTex.ResetTexture();
        }
    }
    public void HandleMapSelectorChanged() { SetTextureFromMapSelector(); }

    public void HandleUiNodeSizeChanged()
    {
        StopSimulation();
        nodeSize = ui.nodeSizeSlider.value;
    }

    public void HandleUiStepDelayChanged()
    {
        stepDelay = ui.stepDelaySlider.value;
    }

    public void HandleUiEditMapInputChanges()
    {
        brushSize = (int)ui.brushSizeSlider.value;
        if (ui.brushToggle.isOn)
        {
            acceptingDrawInput = true;
            brushColor = Color.black;
        }
        else if (ui.eraseToggle.isOn)
        {
            acceptingDrawInput = true;
            brushColor = Color.white;
        }
        else
        {
            acceptingDrawInput = false;
        }
    }

    bool CursorOnMaze(out Vector3 hitPos)
    {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        bool hitRes = Physics.Raycast(ray, out RaycastHit hit);
        hitPos = hit.point;
        if (hitRes && hit.collider.gameObject == map) { return true; }
        return false;
    }

    public void HandleSelectStartPoint()
    {
        StopSimulation();
        acceptingStartInput = true;
        lastInputTime = Time.time;
    }

    public void HandleSelectTargetPoint()
    {
        StopSimulation();
        acceptingTargetInput = true;
        lastInputTime = Time.time;
    }

    public void HandleStartStopEvent()
    {
        if (simulationRunning) { StopSimulation(); }
        else { StartSimulation(); }
    }

    void Update()
    {
        if ((acceptingStartInput || acceptingTargetInput || acceptingDrawInput) && Time.time - lastInputTime > 1 && Input.GetAxisRaw("Fire1") > 0)
        {
            if (CursorOnMaze(out Vector3 hitPos))
            {
                if (acceptingStartInput)
                {
                    startPos = hitPos;
                    startFlag.transform.position = startPos;
                    startFlag.SetActive(true);
                    acceptingStartInput = false;
                }
                else if (acceptingTargetInput)
                {
                    targetPos = hitPos;
                    targetFlag.transform.position = targetPos;
                    targetFlag.SetActive(true);
                    acceptingTargetInput = false;
                }
                else if (acceptingDrawInput)
                {
                    mapTex.SetPixelsAroundWorldPos(hitPos, brushSize, brushColor);
                }

            }
        }

    }


    public IEnumerator PathFinderRoutine(PathFinder pathFinder)
    {
        pathFinder.state = PathFinder.State.running;
        yield return new WaitForSeconds(startDelay);
        while (pathFinder.state == PathFinder.State.running && simulationRunning)
        {
            yield return new WaitForSeconds(stepDelay);
            pathFinder.Step();
        }
        if (pathFinder.state == PathFinder.State.solved)
        {
            ui.ShowToast("Maze Solved :)", Color.green);
        }
        else if (pathFinder.state == PathFinder.State.unsolved)
        {
            ui.ShowToast("Maze Unsolved :(", Color.red);
        }
    }

}
