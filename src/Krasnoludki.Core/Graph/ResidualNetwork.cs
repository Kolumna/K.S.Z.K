using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Graph;

public class ResidualNetwork
{
    public int DwarvesCount {get;}
    public int MinesCount {get;}

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
    
    public ResidualNetwork(List<Dwarf> dwarves, List<Mine> mines)
    {
        DwarvesCount = dwarves.Count;
        MinesCount = mines.Count;
        SinkID = DwarvesCount + MinesCount + 1;


        _nodes = new List<IGraphNode>();

        int CurrDwarfId = 1,CurrMineId;

        Edges = new List<EdgeFlow>();

        //Edges from artificial source to every Dwarf
        foreach(Dwarf dwarf in dwarves)
        {
            GraphNode<Dwarf> new_node = new GraphNode<Dwarf>(CurrDwarfId,dwarf); //adding new node with dwarf in list of nodes
            _nodes.Add(new_node);

            EdgeFlow edge = new EdgeFlow(SourceID, CurrDwarfId, 1); // source -> dwarf edge + dwarf -> source ege

            Edges.Add(edge);
            //Edges.Add(edge.BackwardEdge);

            CurrMineId = DwarvesCount + 1;
            foreach(Mine mine in mines) //Edges from every Dwarf to every Mine
            {
                double cost = Math.Sqrt(Math.Pow(dwarf.HomeLocation.x - mine.Location.x, 2) + Math.Pow(dwarf.HomeLocation.y - mine.Location.y, 2));
                // A massive artificial cost is added to non-preferred mines, ensuring the algorithm only picks
                // them as an absolute last resort to prevent unemployment.
                if (!dwarf.PreferredMinerals.Contains(mine.Resource))
                {
                    cost += 1000000;
                }
                EdgeFlow DwarfMineEdge = new EdgeFlow(CurrDwarfId,CurrMineId, 1, cost);

                Edges.Add(DwarfMineEdge);
                //Edges.Add(DwarfMineEdge.BackwardEdge);

                CurrMineId++;
            }
            CurrDwarfId++;
        }

    
        
        CurrMineId = DwarvesCount + 1;
        //Edges from every mine to artificial sink
        foreach(Mine mine in mines)
        {
            GraphNode<Mine> new_node = new GraphNode<Mine>(CurrMineId, mine); // adding new node with mine in list of nodes
            _nodes.Add(new_node);

            EdgeFlow MineSinkEdge = new EdgeFlow(CurrMineId, SinkID, mine.Capacity);

            Edges.Add(MineSinkEdge);
            //Edges.Add(MineSinkEdge.BackwardEdge);

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