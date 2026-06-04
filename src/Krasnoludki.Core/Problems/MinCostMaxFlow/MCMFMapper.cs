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
            GraphDwarf newDwarf = new GraphDwarf(dwarf.PointId,dwarf.x,dwarf.y, dwarf.PreferredMinerals, dwarf.VoiceLoudness);
            graphDwarves.Add(newDwarf);
        }
        return graphDwarves;
    }

    public List<GraphMine> MapMines(List<Mine> frontendMines)
    {
        List<GraphMine> graphMines = new List<GraphMine>();
        foreach(var mine in frontendMines)
        {
            GraphMine newMine = new GraphMine(mine.PointId,mine.x,mine.y, mine.Resource, mine.Capacity);
            graphMines.Add(newMine);
        }
        return graphMines;
    }
}