namespace Krasnoludki.Core.Models;

public enum PointType
{
    Source,Dwarf,Mine,Sink
}

public enum MineralType
{
    Gold,Quartz,Silver,Coal,None
}

public class Point
{
    private static int _PointCounter = 1;
    public int PointId;
    public double x;
    public double y;
    public PointType? Type;

    public Point(double x, double y)
    {
        PointId = _PointCounter++;
        this.x = x;
        this.y = y;
    }
}