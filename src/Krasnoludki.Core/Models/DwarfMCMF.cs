namespace Krasnoludki.Core.McmfAlgorithm.Models;

public class GraphDwarf
{
    public GraphPoint HomeLocation{ get; }
    public int Id => HomeLocation.PointId; // the same id as we got from frontend
    public int VoiceLoudness{ get; }
    public List<MineralType> PreferredMinerals { get; } 
    public GraphMine? WorksIn { get; private set; }

    public GraphDwarf(int id, double x, double y, List<MineralType> minerals, int loudness)
    {
        HomeLocation = new GraphPoint(id, x, y);
        VoiceLoudness = loudness;
        PreferredMinerals = minerals;
    }

    /// <summary>
    /// Sets a reference to the mine where the dwarf works.
    /// NOTE: This method is part of an internal two-way binding.
    /// Do not call it manually! Instead, use the <see cref="GraphMine.AddWorker(GraphDwarf)"/>.
    /// </summary>
    /// <param name="dwarf"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void AssignMine(GraphMine mine)      //funkcja przydzielania kopalni
    {
        WorksIn = mine;
    }
}
