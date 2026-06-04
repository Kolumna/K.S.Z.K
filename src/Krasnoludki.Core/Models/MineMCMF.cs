namespace Krasnoludki.Core.McmfAlgorithm.Models;

public class GraphMine
{
    public GraphPoint Location { get; }
    public int Id => Location.PointId;
    public MineralType Resource{ get; }
    public int Capacity{ get; }
    private List<GraphDwarf> _workers;    //private list of dwarves that work in that mine to prevent someone from asigning more than it can be asigned
    public IReadOnlyList<GraphDwarf> Workers => _workers; // easy alternative for _workers list to be readable but not modified by chance in not expected way
    public bool IsFull => _workers.Count >= Capacity;     //czy kopalnia ma maks pracowników

    public GraphMine(int id, double x, double y, MineralType mineral, int capacity)
    {
        Location = new GraphPoint(id, x, y);
        Resource = mineral;
        Capacity = capacity;
        _workers = new List<GraphDwarf>();
    }

    /// <summary>
    /// Assigns a dwarf to this mine.
    /// This method automatically updates state in both objects (two-way binding).
    /// </summary>
    /// <param name="dwarf">The dwarf assigned to the mine by the residual network algorithm.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the algorithm attempts to assign a dwarf to a mine that has already reached its capacity limit.
    /// </exception>
    public void AddWorker(GraphDwarf dwarf)
    {
       if (IsFull)
        throw new InvalidOperationException($"Algorytm próbował przepełnić kopalnię {Id}!");
        
        _workers.Add(dwarf);
        dwarf.AssignMine(this);
    }

}
