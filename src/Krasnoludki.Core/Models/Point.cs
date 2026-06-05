using System.Text.Json.Serialization;

namespace Krasnoludki.Core.Models;

public enum PointType { Source, Dwarf, Mine, Sink }
public enum MineralType { Gold, Quartz, Silver, Coal, None }

public abstract class Point
{
    public int PointId { get; init; }
    public double x { get; init; }
    public double y { get; init; }

    public PointType? Type { get; set; }

    public Point(int id, double x, double y)
    {
        PointId = id;
        this.x = x;
        this.y = y;
    }
}