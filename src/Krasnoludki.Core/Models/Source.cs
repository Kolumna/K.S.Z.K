namespace Krasnoludki.Core.Models;

public class Source : Point
{
    public Source(double x = 0, double y = 0) : base(x, y)
    {
        Type = PointType.Source;
    }
}