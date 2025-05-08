using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int gridHeight;
    public int gridWidth;
    public int padding;
    public float obstacleChance;
    public GameObject gridSquarePrefab;
    public GameObject[,] maze;

    public Pos startPos;
    public Pos targetPos;
    private float squareHeight;
    private float squareWidth;

    void Start()
    {
        Renderer squareRend = gridSquarePrefab.GetComponent<Renderer>();
        squareHeight = squareRend.bounds.size.z + padding;
        squareWidth = squareRend.bounds.size.x + padding;
        startPos = new(-1, -1);
        targetPos = new(-1, -1);
    }

    public GridSquare GetGridSquare(int y, int x)
    {
        return maze[y, x].GetComponent<GridSquare>();
    }

    int GetRandomRow() { return Random.Range(0, gridHeight); }
    int GetRandomCol() { return Random.Range(0, gridWidth); }

    public void GenerateMaze()
    {
        maze = new GameObject[gridHeight, gridWidth];

        Vector3 gridPosition;
        GameObject gridSquareObj;
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                gridPosition = new(col * squareWidth, 0, row * squareHeight);
                gridSquareObj = Instantiate(gridSquarePrefab, gridPosition, gridSquarePrefab.transform.rotation);
                if (Random.Range(0, 1f) < obstacleChance)
                {
                    gridSquareObj.GetComponent<GridSquare>().SetBlocked();
                }
                else
                {
                    gridSquareObj.GetComponent<GridSquare>().SetTraversable();
                    Debug.Log("Adding Traversable GridSquare...");
                }
                maze[row, col] = gridSquareObj;

            }
        }
        // Create a random start and random target
        Pos startPos = new(GetRandomRow(), GetRandomCol());
        maze[startPos.y, startPos.x].GetComponent<GridSquare>().SetStart();
        Pos targetPos = new(GetRandomRow(), GetRandomCol());
        maze[targetPos.y, targetPos.x].GetComponent<GridSquare>().SetTarget();

    }

}
