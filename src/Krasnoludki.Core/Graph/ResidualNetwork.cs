using Krasnoludki.Core.McmfAlgorithm.Models;

namespace Krasnoludki.Core.Graph;

public class ResidualNetwork
{
    public int DwarvesCount {get;}
    public int MinesCount {get;}
    public const long NON_PREFERRED_PENALTY = 1000000000L; 
    // value that is added to cost if edge is between dwarf and mine with non-preferred mineral type
    // to prevent situation when some dwarfs are not working because of
    // lack of free place in mine with preferred minerals

    private readonly List<IGraphNode> _nodes;
    public List<EdgeFlow> Edges{get;}
    public int SourceID { get; } = 0;
    public int SinkID { get; }


    public IGraphNode GetNode(int id)
    {
        if (id > 0 && id <= _nodes.Count)
        {
            return _nodes[id - 1];
        }

        throw new ArgumentOutOfRangeException(nameof(id), $"\aArgumentOutOfRangeException: Node with id: {id} doesn't exists in residual network!\n");
    }
    
    public ResidualNetwork(List<GraphDwarf> dwarves, List<GraphMine> mines)
    {
        DwarvesCount = dwarves.Count;
        MinesCount = mines.Count;
        SinkID = DwarvesCount + MinesCount + 1;


        _nodes = new List<IGraphNode>();

        int CurrDwarfId = 1,CurrMineId;

        Edges = new List<EdgeFlow>();

        //Edges from artificial source to every Dwarf
        foreach(GraphDwarf dwarf in dwarves)
        {
            GraphNode<GraphDwarf> new_node = new GraphNode<GraphDwarf>(CurrDwarfId,dwarf); //adding new node with dwarf in list of nodes
            _nodes.Add(new_node);

            EdgeFlow edge = new EdgeFlow(SourceID, CurrDwarfId, 1); // source -> dwarf edge + dwarf -> source ege

            Edges.Add(edge);

            CurrMineId = DwarvesCount + 1;
            foreach(GraphMine mine in mines) //Edges from every Dwarf to every Mine
            {
                // cost = physical distance between dwarf's home and mine location
                // (in this case rounded to 3 digits after comma to prevent problems with computer precision)
                double distance = Math.Round(dwarf.HomeLocation.CalculateDistance(mine.Location), 5);
                long cost = (long)(distance * 100000);

                // A massive artificial cost is added to non-preferred mines, ensuring the algorithm only picks
                // them as an absolute last resort to prevent unemployment.
                if (!dwarf.PreferredMinerals.Contains(mine.Resource))
                {
                    cost += NON_PREFERRED_PENALTY;
                }
                EdgeFlow DwarfMineEdge = new EdgeFlow(CurrDwarfId, CurrMineId, 1, cost);

                Edges.Add(DwarfMineEdge);

                CurrMineId++;
            }
            CurrDwarfId++;
        }

    
        
        CurrMineId = DwarvesCount + 1;
        //Edges from every mine to artificial sink
        foreach(GraphMine mine in mines)
        {
            GraphNode<GraphMine> new_node = new GraphNode<GraphMine>(CurrMineId, mine); // adding new node with mine in list of nodes
            _nodes.Add(new_node);

            EdgeFlow MineSinkEdge = new EdgeFlow(CurrMineId, SinkID, mine.Capacity);

            Edges.Add(MineSinkEdge);

            CurrMineId++;
        }

    }

    // Artificial constructor designed exclusively for unit testing.
    // Allows direct injection of predefined nodes and edges to simulate specific graph topologies
    // and edge cases without breaking encapsulation.
    public ResidualNetwork(List<IGraphNode> nodes, List<EdgeFlow> edges, int sourceId, int sinkId)
    {
        _nodes = nodes;
        Edges = edges;
        SourceID = sourceId; 
        SinkID = sinkId;
    }

}