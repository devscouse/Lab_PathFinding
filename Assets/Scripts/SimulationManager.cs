using System.Collections;
using TMPro;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject maze;
    public PathFinder pathFinder;
    public Texture2D mazeImage;

    public TextMeshProUGUI instructionText;
    private float lastInputTime;
    private bool startPlacement;
    private bool targetPlacement;

    public float startDelay;
    public float stepDelay;

    private MazeImageLoader mazeImageLoader;
    private NodeGrid nodeGrid;
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        mazeImageLoader = maze.GetComponent<MazeImageLoader>();
        mazeImageLoader.LoadImageToTexture(mazeImage);

        // FrameObjectInCamera(maze, mainCamera.GetComponent<Camera>());

        nodeGrid = maze.GetComponent<NodeGrid>();
        nodeGrid.CreateNodeGrid();

        startPlacement = true;
        targetPlacement = false;
        instructionText.text = "Please select a start point";
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

    void Update()
    {
        if ((Time.time - lastInputTime > 1) && (startPlacement || targetPlacement) && Input.GetAxisRaw("Fire1") > 0)
        {
            lastInputTime = Time.time;
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == maze)
            {
                if (startPlacement)
                {
                    startPos = hit.point;
                    Debug.Log("startPos set to " + startPos);

                    startPlacement = false;
                    targetPlacement = true;
                    instructionText.text = "Please select a target point";
                    return;
                }

                if (targetPlacement)
                {
                    targetPos = hit.point;
                    Debug.Log("targetPos set to " + targetPos);

                    targetPlacement = false;
                    instructionText.text = "";
                    pathFinder = new PathFinder(nodeGrid, startPos, targetPos);
                    StartCoroutine(PathFinderRoutine(pathFinder));
                    return;
                }
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
    }

    public IEnumerator PathFinderRoutine(PathFinder pathFinder)
    {
        yield return new WaitForSeconds(startDelay);
        while (!pathFinder.finished)
        {
            yield return new WaitForSeconds(stepDelay);
            pathFinder.Step();
        }

        if (pathFinder.solved)
        {
            instructionText.text = "MAZE SOLVED";
        }
        else
        {
            instructionText.text = "Couldn't solve it, sorry :(";
        }

    }

}
