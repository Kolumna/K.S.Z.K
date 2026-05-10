namespace Krasnoludki.Core.Models;

public enum MineralType
{
    Gold,Quartz,Silver,Coal
}

public class Dwarf
{
    private static int _DwarfCounter = 1;
    
    public int Id{ get; }
    public Point HomeLocation{ get; }
    public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } // Zmienilem na List, bo w zadaniu jest mowa o "preferowanych minerałach" (w liczbie mnogiej), a nie o jednym preferowanym minerale

    public Dwarf(double x, double y, List<MineralType> minerals, int loudness)
    {
        HomeLocation = new Point(x,y);
        VoiceLoudness = loudness;
        PreferredMinerals = minerals;
        Id = _DwarfCounter++;
    }
}
