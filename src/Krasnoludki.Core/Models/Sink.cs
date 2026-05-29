namespace Krasnoludki.Core.Models;

public class Sink : Point
{
    public Sink(int id, double x = 0, double y = 0) : base(id+1, x, y)
    {
        Type = PointType.Sink;
    }
}