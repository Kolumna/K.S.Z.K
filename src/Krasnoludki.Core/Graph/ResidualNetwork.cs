using Krasnoludki.Core.Models;

namespace Krasnoludki.Core.Graph;

class ResidualNetwork
{
    public List<IGraphNode> Nodes;
    public List<EdgeFlow> Edges{get;}
    public int SourceID = 0;
    public int SinkID;

    public ResidualNetwork(List<Dwarf> dwarves, List<Mine> mines, List<List<int>> distances)
    {

        Nodes = new List<IGraphNode>();

        IGraphNode Source = new GraphNode<object>(0,null);
        Nodes.Add(Source);


        int CurrId = 1;

        Edges = new List<EdgeFlow>();

        //Edges from artificial source to every Dwarf
        foreach(Dwarf dwarf in dwarves)
        {
            GraphNode<Dwarf> new_node = new GraphNode<Dwarf>(CurrId,dwarf);
            Nodes.Add(new_node);

            EdgeFlow edge = new EdgeFlow(0, CurrId, 1);
            EdgeFlow backwardEdge = new EdgeFlow(CurrId, 0, 0);
            edge.BackwardEdge = backwardEdge;
            backwardEdge.BackwardEdge = edge;

            Edges.Add(edge);
            Edges.Add(backwardEdge);

            CurrId++;
        }

        //Edges from every Dwarf to every Mine
        int mine_id,dwarves_count = CurrId;
        for(int dwarf_id = 1; dwarf_id < dwarves_count; dwarf_id++)
        {
            for(mine_id = dwarves_count; mine_id - dwarves_count < mines.Count; mine_id++)
            {
                double cost = distances[dwarf_id - 1][mine_id - dwarves_count];
                EdgeFlow DwarfMineEdge = new EdgeFlow(dwarf_id, mine_id, 1, cost);
                EdgeFlow MineDwarfEdge = new EdgeFlow(mine_id,dwarf_id, 0, -cost);

                DwarfMineEdge.BackwardEdge = MineDwarfEdge;
                MineDwarfEdge.BackwardEdge = DwarfMineEdge;

                Edges.Add(DwarfMineEdge);
                Edges.Add(MineDwarfEdge);

            }
        }

        //Edges from every mine to artificial sink
        SinkID = dwarves.Count + mines.Count + 1;
        mine_id = dwarves_count;
        foreach(Mine mine in mines)
        {
            GraphNode<Mine> new_node = new GraphNode<Mine>(mine_id, mine);
            Nodes.Add(new_node);

            EdgeFlow MineSinkEdge = new EdgeFlow(mine_id, SinkID, mine.Capacity);
            EdgeFlow SinkMineEdge = new EdgeFlow(SinkID, mine_id, 0);

            MineSinkEdge.BackwardEdge = SinkMineEdge;
            SinkMineEdge.BackwardEdge = MineSinkEdge;

            Edges.Add(MineSinkEdge);
            Edges.Add(SinkMineEdge);

            mine_id++;

        }


        IGraphNode Sink = new GraphNode<object>(SinkID,null);
        Nodes.Add(Sink);

    }

}