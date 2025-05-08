public struct Pos
{
    public Pos(int y, int x)
    {
        this.x = x;
        this.y = y;
    }
    public int x { get; set; }
    public int y { get; set; }
    public override readonly string ToString() => $"({x}, {y})";
}
