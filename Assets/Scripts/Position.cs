public struct Pos
{
    public Pos(int y, int x)
    {
        this.x = x;
        this.y = y;
    }
    public int x { get; set; }
    public int y { get; set; }
    public override readonly string ToString() => $"({y}, {x})";

    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Pos other = (Pos)obj;
        return this.x == other.x && this.y == other.y;
    }
    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

}
