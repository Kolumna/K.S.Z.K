namespace Krasnoludki.Core.Models;

public class Mine : Point
{
    private static int _MineCounter = 1;
    public int Id;
    public MineralType Resource{ get; }
    public int Capacity{ get; }
    public List<Dwarf> Workers;     //lista na Id pracujących w kopalni krasnoludków
    public bool IsFull;     //czy kopalnia ma maks pracowników

    public Mine(double x, double y, MineralType mineral, int capacity) : base(x, y)
    {
        Id = _MineCounter++;
        Resource = mineral;
        Capacity = capacity;
        Workers = new List<Dwarf>();
        IsFull = false;
    }

    public void AddWorker(Dwarf dwarf)
    {
        Workers.Add(dwarf);
        if(Workers.Count >= Capacity) IsFull = true;
    }

}
