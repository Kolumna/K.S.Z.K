namespace Krasnoludki.Core.Models;

public class Dwarf
{
    public Point HomeLocation{ get; }
    public int Id => HomeLocation.PointId; // the same id as we got from frontend
    public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } 
    public Mine? WorksIn { get; private set; }

    public Dwarf(int id, double x, double y, List<MineralType> minerals, int loudness)
    {
        HomeLocation = new Point(id, x, y);
        VoiceLoudness = loudness;
        PreferredMinerals = minerals;
    }

    /// <summary>
    /// Sets a reference to the mine where the dwarf works.
    /// NOTE: This method is part of an internal two-way binding.
    /// Do not call it manually! Instead, use the <see cref="Mine.AddWorker(Dwarf)"/>.
    /// </summary>
    /// <param name="dwarf"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void AssignMine(Mine mine)      //funkcja przydzielania kopalni
    {
        WorksIn = mine;
    }
}
