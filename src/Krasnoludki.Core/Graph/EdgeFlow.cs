namespace Krasnoludki.Core.Graph;

public class EdgeFlow
{
    public int From;
    public int To;
    public int Capacity;
    public int CurrFlow;
    public int BackCapacity;

    public EdgeFlow(int from, int to, int capacity/*, double cost = 0*/)
    {
        From = from;
        To = to;
        Capacity = capacity;
        //Cost = cost;
        CurrFlow = 0;
        BackCapacity = 0;
    }

    public void AddFlow(int Flow)
    {
        CurrFlow += Flow;
        BackCapacity += Flow;
    }

    public void AddBackFlow(int Flow)
    {
        BackCapacity -= Flow;
        CurrFlow -= Flow;
    }
}