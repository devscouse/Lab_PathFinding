public struct Pos
{
    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
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
        return x == other.x && y == other.y;
    }
    public override readonly int GetHashCode()
    {
        return base.GetHashCode();
    }

}
