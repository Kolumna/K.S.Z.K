using System.Text.Json.Serialization;

namespace Krasnoludki.Core.Models;

public class Dwarf : Point
{
    public int VoiceLoudness { get; }
    public List<MineralType> PreferredMinerals { get; init; }
    public Mine? WorksIn { get; private set; }

    [JsonConstructor]
    public Dwarf(int pointId, double x, double y, List<MineralType> preferredMinerals, int voiceLoudness) : base(pointId, x, y)
    {
        VoiceLoudness = voiceLoudness;
        PreferredMinerals = preferredMinerals ?? new List<MineralType>();
    }
    public void AssignMine(Mine mine)
    {
        WorksIn = mine;
    }

    public int GetLoudness()
    {
        return VoiceLoudness;
    }
}