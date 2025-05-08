using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private MazeGenerator mazeGenerator;
    private PathFinder pathFinder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        pathFinder = GetComponent<PathFinder>();

        mazeGenerator.GenerateMaze();
        pathFinder.FindPath();
    }
}
