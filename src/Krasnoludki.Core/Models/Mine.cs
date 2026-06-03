namespace Krasnoludki.Core.Models;

public class Mine
{
    private static int _MineCounter = 1;
    public int Id{ get; }
    public Point Location{ get; }
    public MineralType Resource{ get; }
    public int Capacity{ get; }

    public Mine(double x, double y, MineralType mineral, int capacity)
    {
        Id = _MineCounter++;
        Location = new Point(x,y);
        Resource = mineral;
        Capacity = capacity;
    }

}
