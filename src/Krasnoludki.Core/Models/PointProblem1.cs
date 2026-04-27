namespace Krasnoludki.Core.Models;

public enum PointType
{
    Dwarf,Mine
}

public class Point
{
    public PointType Type;
    public Dwarf? Dwarf;
    public Mine? Mine;

    public Point(Mine mine)
    {
        Type = PointType.Mine;
        Mine = mine;
    }

    public Point(Dwarf dwarf)
    {
        Type = PointType.Dwarf;
        Dwarf = dwarf;
    }
}