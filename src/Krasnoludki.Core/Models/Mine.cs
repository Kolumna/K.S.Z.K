namespace Krasnoludki.Core.Models;

public class Mine : Point
{
    private static int _MineCounter = 1;
    public int Id;
    public MineralType Resource{ get; }
    public int Capacity{ get; }
    public List<int> Workers;     //tablica na Id pracujących w kopalni krasnoludków
    public bool IsFull;     //czy kopalnia ma maks pracowników

    public Mine(double x, double y, MineralType mineral, int capacity) : base(x, y)
    {
        Id = _MineCounter++;
        Resource = mineral;
        Capacity = capacity;
        Workers = new List<int>();
        IsFull = false;
    }

    public void AddWorker(int DwarfId)
    {
        Workers.Add(DwarfId);
        if(Workers.Count >= Capacity) IsFull = true;
    }

}
