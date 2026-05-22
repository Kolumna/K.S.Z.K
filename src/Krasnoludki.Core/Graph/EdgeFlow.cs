namespace Krasnoludki.Core.Graph;

public class EdgeFlow
{
    public int From;
    public int To;
    public int Capacity;
    public int CurrFlow;
    public double Cost;
    public EdgeFlow BackwardEdge;

    public EdgeFlow(Models.Point from, Models.Point to, int capacity)
    {
        From = from.PointId;
        To = to.PointId;
        Capacity = capacity;
        Cost = Math.Sqrt(Math.Pow(from.x - to.x, 2) + Math.Pow(from.y - to.y, 2));
        CurrFlow = 0;
        BackwardEdge = new EdgeFlow(to, from, this);
    }
    public EdgeFlow(Models.Point from, Models.Point to, EdgeFlow mainEdge)
    {
        From = from.PointId;
        To = to.PointId;
        Capacity = 0;
        Cost = Math.Sqrt(Math.Pow(from.x - to.x, 2) + Math.Pow(from.y - to.y, 2));
        CurrFlow = 0;
        BackwardEdge = mainEdge;
    }

    public void AddFlow(int flow)
    {
        CurrFlow += flow;
        BackwardEdge.Capacity += flow;
    }

    public void AddBackFlow(int flow)
    {
        BackwardEdge.Capacity -= flow;
        CurrFlow -= flow;
    }
}