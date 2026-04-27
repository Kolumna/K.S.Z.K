namespace Krasnoludki.Core;

public class EdgeWeight
{
    private static int _EdgeCounter = 0;
    public int Id;
    public Models.Point[] Connecting;
    public int Length{ get; }

    public EdgeWeight(Models.Point from, Models.Point to, int length)
    {
        Connecting = new Models.Point[2];
        Id = _EdgeCounter++;
        Connecting[0] = from;
        Connecting[1] = to;
        Length = length;
    }
}