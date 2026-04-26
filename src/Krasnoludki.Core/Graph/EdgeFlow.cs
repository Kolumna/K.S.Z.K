namespace Krasnoludki.Core.Graph;

public class EdgeFlow
{
    public int From{ get; }
    public int To{ get; }
    public int Capacity{ get; }
    public int CurrFlow{ get; set;}
    public double Cost { get; }
    public EdgeFlow? BackwardEdge{ get; set; }

    public EdgeFlow(int from, int to, int capacity, double cost = 0)
    {
        From = from;
        To = to;
        Capacity = capacity;
        Cost = cost;
        CurrFlow = 0;
    }

}