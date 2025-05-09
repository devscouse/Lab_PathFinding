using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SimulationManager : MonoBehaviour
{
    private Maze maze;
    private PathFinder pathFinder;

    public GameObject mainCamera;
    public GameObject gridSquarePrefab;
    public float gridSquarePadding;
    public float obstacleChance;
    public int width;
    public int height;
    public float startDelay;
    public float stepDelay;

    private float squareHeight;
    private float squareWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer gsRender = gridSquarePrefab.GetComponent<Renderer>();
        squareHeight = gsRender.bounds.size.z + gridSquarePadding;
        squareWidth = gsRender.bounds.size.x + gridSquarePadding;

        float xPos = (squareWidth + gridSquarePadding) * width / 2;
        float zPos = (squareHeight + gridSquarePadding) * height / 2;
        float fov = mainCamera.GetComponent<Camera>().fieldOfView;
        float cameraHeight = (float)Math.Max(
            zPos / Math.Tan(fov / 2),
            xPos / Math.Tan(Camera.VerticalToHorizontalFieldOfView(fov, Camera.main.aspect) / 2)
        );
        mainCamera.transform.position = new Vector3(xPos, cameraHeight, zPos);


        maze = new Maze(width, height);
        GenerateMaze(maze);

        pathFinder = new PathFinder(maze, startDelay, stepDelay);
        StartCoroutine(pathFinder.PathFinderRoutine(pathFinder.AStarStep));
    }

    int GetRandomGridRow() { return UnityEngine.Random.Range(0, height); }
    int GetRandomGridCol() { return UnityEngine.Random.Range(0, width); }

    void GenerateMaze(Maze maze)
    {
        Vector3 gridPos;
        GridSquare gridSquare;

        for (int row = 0; row < maze.height; row++)
        {
            for (int col = 0; col < maze.width; col++)
            {
                gridPos = new(col * squareWidth + squareWidth / 2, 0, row * squareHeight + squareHeight / 2);
                gridSquare = Instantiate(gridSquarePrefab, gridPos, gridSquarePrefab.transform.rotation).GetComponent<GridSquare>();
                if (UnityEngine.Random.Range(0, 1f) < obstacleChance)
                {
                    gridSquare.GetComponent<GridSquare>().SetStatus(GridSquare.Status.Blocked);
                }
                else
                {
                    gridSquare.GetComponent<GridSquare>().SetStatus(GridSquare.Status.Traversable);
                    Debug.Log("Adding Traversable GridSquare...");
                }
                maze.SetGridSquare(row, col, gridSquare);
            }
        }
        // Create a random start and random target
        Pos startPos = new(GetRandomGridRow(), UnityEngine.Random.Range(0, Math.Min(10, width)));
        maze.GetGridSquare(startPos).SetStatus(GridSquare.Status.Start);
        maze.startPos = startPos;

        Pos targetPos = new(GetRandomGridRow(), UnityEngine.Random.Range(Math.Max(0, width - 10), width));
        maze.GetGridSquare(targetPos).SetStatus(GridSquare.Status.Target);
        maze.targetPos = targetPos;


    }
}
