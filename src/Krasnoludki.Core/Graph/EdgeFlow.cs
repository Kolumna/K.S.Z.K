namespace Krasnoludki.Core.Graph;

public class EdgeFlow
{
    public int From;
    public int To;
    public int Capacity;
    public int CurrFlow;
    public double Cost;
    public EdgeFlow BackwardEdge;

    
    public EdgeFlow(int from, int to, int capacity, double cost = 0)
    {
        From = from;
        To = to;
        Capacity = capacity;
        Cost = cost;
        CurrFlow = 0;
        BackwardEdge = new EdgeFlow(this);
    }

    private EdgeFlow(EdgeFlow mainEdge) //konstruktor prywatny do tworzenia krawędzi powrotnej
    {
        From = mainEdge.To;
        To = mainEdge.From;
        Capacity = mainEdge.Capacity;   //ustawia takie samo capacity jak w krawedzi głównej
        Cost = 0 - mainEdge.Cost;       //
        CurrFlow = Capacity;            //zapycha od razu pływ
        BackwardEdge = mainEdge;        //zmiany te pozwalają obsługiwać pływ za pomocą jednej metody
    }

    public int ReturnCapacity()
    {
        return Capacity - CurrFlow;
    }
    public void AddFlow(int flow)
    {
        CurrFlow += flow;
        BackwardEdge.CurrFlow -= flow;
    }
}