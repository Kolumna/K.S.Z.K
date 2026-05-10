
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Algorithms;
class Graph
{
    public List<EdgeFlow> Graph{get;}
    public int SourceID = 0;
    public int SinkID;

    public Graph(ref List<Dwarf> dwarves,List<Mine> mines,List<List<int>> distances)
    {
        int CurrId = 1;

        foreach (ref Dwarf dwarf in dwarves)
        {
            dwarf.GraphID = CurrId++;
        }
        foreach (ref Mine mine in mines)
        {
            mine.Id = CurrId++;
        }

        SinkID = CurrId;

        //distances 

    }

}