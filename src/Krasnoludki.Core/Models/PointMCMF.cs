namespace Krasnoludki.Core.Models;

public enum PointType
{
    Source,Dwarf,Mine,Sink
}

public enum MineralType
{
    Gold,Quartz,Silver,Coal,None
}
public class GraphPoint
{
    public int PointId;
    public double x { get; init; }
    public double y { get; init; }
    public PointType? Type;

    public GraphPoint(int id, double x, double y)
    {
        PointId = id;
        this.x = x;
        this.y = y;
    }
    public double CalculateDistance(GraphPoint other)
    {
        return Math.Sqrt(
                (this.x - other.x) * (this.x - other.x)
            + 
                (this.y - other.y) * (this.y - other.y));
    }
    /*public int HowManyPoints()  //metoda zwraca ile punktów istnieje w momencie wywołania metody
    {
        return _PointCounter-1;
    }*/
}