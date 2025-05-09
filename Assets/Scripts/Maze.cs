using UnityEngine;
using System;

public class Maze
{
    public GridSquare[,] maze;
    public int width;
    public int height;
    public Pos startPos;
    public Pos targetPos;

    public Maze(int width, int height)
    {
        this.width = width;
        this.height = height;
        maze = new GridSquare[height, width];
        startPos = new(-1, -1);
        targetPos = new(-1, -1);
    }

    public void SetGridSquare(int y, int x, GridSquare gs) { maze[y, x] = gs; }
    public void SetGridSquare(Pos p, GridSquare gs) { SetGridSquare(p.y, p.x, gs); }
    public GridSquare GetGridSquare(int y, int x)
    {
        if (y < 0 || y >= height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), "y should be between 0 and " + height);
        }
        else if (x < 0 || x >= width)
        {
            throw new ArgumentOutOfRangeException(nameof(y), "y should be between 0 and " + height);
        }
        Debug.Log("GetGridSquare( " + y + ", " + x + ") => " + maze[y, x]);
        return maze[y, x];
    }
    public GridSquare GetGridSquare(Pos p) { return GetGridSquare(p.y, p.x); }
}
