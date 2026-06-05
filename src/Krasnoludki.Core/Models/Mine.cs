using System.Text.Json.Serialization;

namespace Krasnoludki.Core.Models;

public class Mine : Point
{
    public MineralType Resource { get; }
    public int Capacity { get; }
    public List<Dwarf> Workers { get; set; }
    public bool IsFull { get; set; }     //czy kopalnia ma maks pracowników

    [JsonConstructor]
    public Mine(int pointId, double x, double y, MineralType resource, int capacity) : base(pointId, x, y)
    {
        Resource = resource;
        Capacity = capacity;
        Workers = new List<Dwarf>();
        IsFull = false;
    }

    public void AddWorker(Dwarf dwarf)
    {
        Workers.Add(dwarf);
        if (Workers.Count >= Capacity) IsFull = true;
    }

}