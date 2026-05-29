namespace Krasnoludki.Core.Models;

public class Source : Point
{
    public Source(double x = 0, double y = 0) : base(0, x, y)
    {
        Type = PointType.Source;
    }
}