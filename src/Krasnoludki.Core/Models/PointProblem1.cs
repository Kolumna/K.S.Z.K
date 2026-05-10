namespace Krasnoludki.Core.Models;

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
    public bool IsSoruce;
    public bool IsSink;

    public Point(/*double x, double y,*/bool isSoruce = false, bool isSink = false)
    {
        PointId = _PointCounter++;
        /*this.x = x;
        this.y = y;*/
        IsSoruce = isSoruce;
        IsSink = isSink;
    }
}