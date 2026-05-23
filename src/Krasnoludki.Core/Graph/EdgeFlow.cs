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
        BackwardEdge = new EdgeFlow(this);
    }
    private EdgeFlow(EdgeFlow mainEdge) //konstruktor prywatny do tworzenia krawędzi powrotnej
    {
        From = mainEdge.To;
        To = mainEdge.From;
        Capacity = 0;
        Cost = 0 - mainEdge.Cost;
        CurrFlow = 0;
        BackwardEdge = mainEdge;
    }

    public void BadResource()   //metoda ustawia koszt na abstrakcyjnie wysoki, użwyna w momencie generowania krawędzi między krasnoludkiem a kopalnią o niezgodnych preferencjach i surowcach
    {
        Cost = 1000000;
    }

    public void AddFlow(int flow)
    {
        CurrFlow += flow;
        if(CurrFlow > BackwardEdge.Capacity) BackwardEdge.Capacity = CurrFlow;
    }

    /*public void AddBackFlow(int flow)
    {
        BackwardEdge.Capacity -= flow;
        CurrFlow -= flow;
    }*/
}