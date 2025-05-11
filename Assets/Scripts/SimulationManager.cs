using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject maze;
    public PathFinder pathFinder;
    public Texture2D mazeImage;
    public UI ui;

    public float startDelay;
    public float stepDelay;
    public float costScale;
    public float nodeSize = 0.5f;

    private MazeImageLoader mazeImageLoader;
    private NodeGrid nodeGrid;
    private Vector3 startPos;
    private Vector3 targetPos;

    private float lastInputTime;
    private bool acceptingStartInput;
    private bool acceptingTargetInput;
    private bool simulationRunning;

    void Start()
    {
        mazeImageLoader = maze.GetComponent<MazeImageLoader>();
        mazeImageLoader.LoadImageToTexture(mazeImage);

        // FrameObjectInCamera(maze, mainCamera.GetComponent<Camera>());


    }

    public void StartSimulation()
    {
        nodeGrid = maze.GetComponent<NodeGrid>();
        nodeGrid.CreateNodeGrid(nodeSize);

        pathFinder = new PathFinder(nodeGrid, startPos, targetPos);
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

    public void HandleUiNodeSizeChanged()
    {
        StopSimulation();
        nodeSize = ui.nodeSizeSlider.value;
    }

    public void HandleUiStepDelayChanged()
    {
        stepDelay = ui.stepDelaySlider.value;
    }

    bool CursorOnMaze(out Vector3 hitPos)
    {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        bool hitRes = Physics.Raycast(ray, out RaycastHit hit);
        hitPos = hit.point;
        if (hitRes && hit.collider.gameObject == maze) { return true; }
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


    void FrameObjectInCamera(GameObject obj, Camera camera)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        float cameraDistance = 2.0f; // Constant factor
        Vector3 objectSizes = bounds.max - bounds.min;
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView); // Visible height 1 meter in front
        float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
        distance += 0.2f * objectSize; // Estimated offset from the center to the outside of the object
        camera.transform.position = bounds.center - distance * camera.transform.forward;
    }


    float NormCost(int cost)
    {
        return Mathf.Clamp01(cost / costScale);
    }

    void Update()
    {
        if ((acceptingStartInput || acceptingTargetInput) && Time.time - lastInputTime > 1 && Input.GetAxisRaw("Fire1") > 0)
        {
            if (acceptingStartInput)
            {
                CursorOnMaze(out startPos);
                acceptingStartInput = false;
            }
            else if (acceptingTargetInput)
            {
                CursorOnMaze(out targetPos);
                acceptingTargetInput = false;
            }
        }

    }

    void OnDrawGizmos()
    {
        if (startPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(startPos, 1);
        }
        if (targetPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPos, 1);
        }
        if (nodeGrid != null)
        {
            foreach (Node node in nodeGrid.grid)
            {
                if (node.status == Node.Status.blocked)
                {
                    continue;
                }
                var c = node.status switch
                {
                    Node.Status.open => Color.yellow,
                    Node.Status.closed => new Color(1 - NormCost(node.FCost()), 1 - NormCost(node.gCost), 1 - NormCost(node.hCost)),
                    Node.Status.path => Color.blue,
                    Node.Status.traversable => Color.green,
                    _ => Color.black,
                };
                Gizmos.color = c;
                Gizmos.DrawCube(node.worldPos, new Vector3(nodeGrid.nodeDiameter, 1, nodeGrid.nodeDiameter));
            }
        }
    }

    public IEnumerator PathFinderRoutine(PathFinder pathFinder)
    {
        yield return new WaitForSeconds(startDelay);
        while (!pathFinder.finished && simulationRunning)
        {
            yield return new WaitForSeconds(stepDelay);
            pathFinder.Step();
        }

        if (pathFinder.finished)
        {
            if (pathFinder.solved)
            {
                ui.ShowToast("Maze Solved :)", Color.green);
            }
            else
            {
                ui.ShowToast("Maze Unsolved :(", Color.red);
            }
        }
    }

}
