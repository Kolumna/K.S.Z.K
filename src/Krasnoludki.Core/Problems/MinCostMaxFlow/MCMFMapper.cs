using Krasnoludki.Core.McmfAlgorithm.Models;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.McmfAlgorithm.Adapter;

public class McmfMapper
{
    public List<GraphDwarf> MapDwarves(List<Dwarf> frontendDwarves)
    {
        List<GraphDwarf> graphDwarves = new List<GraphDwarf>();
        foreach(var dwarf in frontendDwarves)
        {
            
        }
        return graphDwarves;
    }

    public List<GraphMine> MapMines(List<Mine> frontendMines)
    {
        List<GraphMine> graphMines = new List<GraphMine>();
        foreach(var mine in frontendMines)
        {
            
        }
        return graphMines;
    }
}