namespace Krasnoludki.Core.Models;

public class Sink : Point
{
    public Sink(double x = 0, double y = 0) : base(x, y)
    {
        Type = PointType.Sink;
    }
}