namespace Krasnoludki.Core.Models;

public class Mine : Point
{
    //private static int _MineCounter = 1;
    //public int Id{ get; }
    //public Point Location{ get; }
    public MineralType Resource{ get; }
    public int Capacity{ get; }
    public int Workers;     //licznik pracowników
    public bool IsFull;     //czy kopalnia ma maks pracowników

    public Mine(/*double x, double y, */MineralType mineral, int capacity) //: base(x, y)
    {
        //Id = _MineCounter++;
        //Location = new Point(x,y);        //zakomentowane niepotrzeben obecnie dane
        Resource = mineral;
        Capacity = capacity;
        Workers = 0;
        IsFull = false;
    }

    public void AddWorker()
    {
        Workers++;
        if(Workers >= Capacity) IsFull = true;
    }

}
