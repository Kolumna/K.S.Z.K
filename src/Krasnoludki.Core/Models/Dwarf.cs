namespace Krasnoludki.Core.Models;

public class Dwarf : Point
{
    private static int _DwarfCounter = 1;
    public int Id{ get; }
    public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } 
    public Mine? WorksIn;

    public Dwarf(int id, double x, double y, List<MineralType> minerals, int loudness) : base(id, x, y)
    {
        VoiceLoudness = loudness;
        PreferredMinerals = minerals;
        Id = _DwarfCounter++;
    }
    public void AssignMine(Mine mine)      //funkcja przydzielania kopalni
    {
        WorksIn = mine;
    }
}
