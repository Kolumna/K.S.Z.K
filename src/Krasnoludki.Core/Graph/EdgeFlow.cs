namespace Krasnoludki.Core.Graph;

public class EdgeFlow
{
    public int From;
    public int To;
    public int Capacity;
    public int CurrFlow;
    public double Cost;
    public EdgeFlow BackwardEdge;

    public EdgeFlow(Models.Source source, Models.Dwarf dwarf)      //konstruktor od source do krasnoludków
    {
        From = source.PointId;
        To = dwarf.PointId;
        Capacity = 1;
        Cost = 0;
        CurrFlow = 0;
        BackwardEdge = new EdgeFlow(this);
    }
    public EdgeFlow(Models.Dwarf dwarf, Models.Mine mine)        //konstruktor od krasnoludka do kopalni
    {
        From = dwarf.PointId;
        To = mine.PointId;
        Capacity = 1;
        if(dwarf.PreferredMinerals.Contains(mine.Resource))
            {
                Cost = Math.Sqrt(Math.Pow(dwarf.x - mine.x, 2) + Math.Pow(dwarf.y - mine.y, 2));
            }
        else
            {
                Cost = 1000000;
            }
        CurrFlow = 0;
        BackwardEdge = new EdgeFlow(this);
    }

        public EdgeFlow(Models.Mine mine, Models.Sink sink)     //konstruktor od mine do sink
    {
        From = mine.PointId;
        To = sink.PointId;
        Capacity = mine.Capacity;
        Cost = 0;
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