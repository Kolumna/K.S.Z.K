using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Graph;

class ResidualNetwork
{
    public List<EdgeFlow> Edges{get;}
    public int SourceID = 0;
    public int SinkID;

    public ResidualNetwork(List<Dwarf> dwarves, List<Mine> mines, List<List<int>> distances)
    {
        int CurrId = 1;

        Edges = new List<EdgeFlow>();

        SinkID = CurrId;

        //distances 

    }

}