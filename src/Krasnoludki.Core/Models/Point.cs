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
    public int PointId;
    public double x { get; init; }
    public double y { get; init; }
    public PointType? Type;

    public Point(int id, double x, double y)
    {
        PointId = id;
        this.x = x;
        this.y = y;
    }

    /*public int HowManyPoints()  //metoda zwraca ile punktów istnieje w momencie wywołania metody
    {
        return _PointCounter-1;
    }*/
}