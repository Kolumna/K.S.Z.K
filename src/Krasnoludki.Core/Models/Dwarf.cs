namespace Krasnoludki.Core.Models;

public enum MineralType
{
    Gold,Quartz,Silver,Coal,None
}

public class Dwarf
{
    private static int _DwarfCounter = 1;

    public int Id{ get; }
    //public int GraphID{ get; set;}        //Zakomentowałem to co nie było mi obecnie potrzebne
    //public Point HomeLocation{ get; }
    //public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } // Zmienilem na List, bo w zadaniu jest mowa o "preferowanych minerałach" (w liczbie mnogiej), a nie o jednym preferowanym minerale
    public Mine? WorksIn;

    public Dwarf(/*double x, double y, */List<MineralType> minerals/*, int loudness*/)
    {
        //HomeLocation = new Point(x,y);
        //VoiceLoudness = loudness;
        PreferredMinerals = minerals;
        Id = _DwarfCounter++;
    }
    public void AssignMine(Mine m)      //funkcja przydzielania kopalni
    {
        WorksIn = m;
    }
}
