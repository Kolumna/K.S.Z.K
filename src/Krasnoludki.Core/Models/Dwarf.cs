namespace Krasnoludki.Core.Models;

public class Dwarf : Point
{
    public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } 
    public Mine? WorksIn;

    public Dwarf(int id, double x, double y, List<MineralType> minerals, int loudness) : base(id, x, y)
    {
        VoiceLoudness = loudness;
        PreferredMinerals = minerals;
    }
    public void AssignMine(Mine mine)      //funkcja przydzielania kopalni
    {
        WorksIn = mine;
    }
}
