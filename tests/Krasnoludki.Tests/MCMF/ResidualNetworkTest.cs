using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.McmfAlgorithm.Models;
using Xunit;
using Krasnoludki.Core.McmfAlgorithm.Graph;

namespace Krasnoludki.Test;

/// <summary>
/// Validates the structural integrity of the residual network topology.
/// Ensures that nodes, forward capacities, and specifically backward edges 
/// (which are vital for the MCMF algorithm's ability to revert bad flow decisions) 
/// are initialized with the correct inverse costs and capacities.
/// </summary>
public class ResidualNetworkTest
{
    private List<GraphDwarf> GetTestGraphDwarves()
    {
        return new List<GraphDwarf>
        {
            // 1. Krasnoludek idealnie blisko kopalni złota
            new GraphDwarf(1, 0.0, 1.0, new List<MineralType> { MineralType.Gold }, 5),
            
            // 2. Krasnoludek blisko kopalni srebra
            new GraphDwarf(2, 11.0, 0.0, new List<MineralType> { MineralType.Silver }, 5),
            
            // 3. Krasnoludek bez żadnych preferencji (musi otrzymać karę NON_PREFERRED_PENALTY do każdej kopalni)
            new GraphDwarf(3, 5.0, 5.0, new List<MineralType>(), 5),
            
            // 4. Konkurencja o kopalnię złota (bardzo blisko krasnoludka 1)
            new GraphDwarf(4, 1.0, 0.0, new List<MineralType> { MineralType.Gold, MineralType.Coal }, 5),
            
            // 5. Krasnoludek wygnaniec - bardzo daleko od wszystkiego
            new GraphDwarf(5, 100.0, 100.0, new List<MineralType> { MineralType.Coal }, 5)
        };
    }

    private List<GraphMine> GetTestGraphMines()
    {
        return new List<GraphMine>
        {
            // 1. Złoto, pojemność 2 (pomieści krasnale 1 i 4)
            new GraphMine(101, 0.0, 0.0, MineralType.Gold, 2),   
            
            // 2. Srebro, bardzo mała pojemność (wąskie gardło)
            new GraphMine(102, 10.0, 0.0, MineralType.Silver, 1), 
            
            // 3. Węgiel, potężna pojemność (tu trafią odrzutki)
            new GraphMine(103, 0.0, 10.0, MineralType.Coal, 5),   
            
            // 4. Złoto, ale wyczerpana (pojemność 0 - przepływ tędy nie przejdzie)
            new GraphMine(104, 20.0, 20.0, MineralType.Gold, 0)   
        };
    }
    [Fact]
    public void ResidualNetwork_Structure()
    {
        List<GraphDwarf> dwarves = GetTestGraphDwarves();
        List<GraphMine> mines = GetTestGraphMines();

    
        var network = new ResidualNetwork(dwarves,mines);

        Assert.Equal(network.DwarvesCount, dwarves.Count);
        Assert.Equal(network.MinesCount, mines.Count);

        Assert.Equal(0, network.SourceID);
        Assert.Equal(dwarves.Count + mines.Count + 1, network.SinkID);

        //check whether the edges are appropriately generated
        foreach(GraphEdgeFlow networkEdge in network.Edges)
        {
            // Source -> Dwarf Edge
            if(networkEdge.From == 0)
            {
                Assert.True(networkEdge.To <= network.DwarvesCount);
                Assert.Equal(0, networkEdge.Cost);
                Assert.Equal(1, networkEdge.Capacity);
                Assert.Equal(0, networkEdge.CurrFlow);

                Assert.True(networkEdge.BackwardEdge.From == networkEdge.To);
                Assert.True(networkEdge.BackwardEdge.To == networkEdge.From);
                Assert.Equal(0, networkEdge.BackwardEdge.Cost);
                Assert.Equal(1, networkEdge.BackwardEdge.Capacity);
                Assert.Equal(networkEdge.Capacity, networkEdge.BackwardEdge.CurrFlow);

                continue;
            }
            // Mine -> Sink Edge
            else if (networkEdge.To == network.SinkID)
            {
                Assert.True(networkEdge.From >= network.DwarvesCount && networkEdge.From <= network.MinesCount + network.DwarvesCount);


                GraphMine mineData =  ((GraphNode<GraphMine>)network.GetNode(networkEdge.From)).Data;
                Assert.Equal(0, networkEdge.CurrFlow);
                Assert.Equal(0, networkEdge.Cost);
                Assert.Equal(mineData.Capacity, networkEdge.Capacity);

                Assert.True(networkEdge.BackwardEdge.From == networkEdge.To);
                Assert.True(networkEdge.BackwardEdge.To == networkEdge.From);

                Assert.Equal(0, networkEdge.BackwardEdge.Cost);
                Assert.Equal(networkEdge.Capacity, networkEdge.BackwardEdge.Capacity);
                Assert.Equal(networkEdge.Capacity, networkEdge.BackwardEdge.CurrFlow);

                continue;
            }
            // Dwarf -> Mine Edge
            else if (networkEdge.From <= network.DwarvesCount && networkEdge.To > network.DwarvesCount)
            {
                Assert.True(networkEdge.From > 0 && networkEdge.To <= network.MinesCount + network.DwarvesCount);
                
                Assert.Equal(0, networkEdge.CurrFlow);
                Assert.Equal(1, networkEdge.Capacity);


                GraphDwarf dwarfData = ((GraphNode<GraphDwarf>)network.GetNode(networkEdge.From)).Data;
                GraphMine mineData =  ((GraphNode<GraphMine>)network.GetNode(networkEdge.To)).Data;

                double distance = Math.Round(dwarfData.HomeLocation.CalculateDistance(mineData.Location), 5);
                // because we assume precision to 5 digets after comma 
                // and to prevent  problems with computer precision (bellman - ford can enter an infinite loop)
                // we operate on long and in the end of MCMF divide total cost by 100000
                long expectedCost = (long)(distance * 100000);

                // we add some big penalty to cost of edge to prevent unemployment in case if there's no place in
                // mine with preferred resource, and this edge (nearest) is chosen in case if dwarf is unemployed
                if (!dwarfData.PreferredMinerals.Contains(mineData.Resource))
                {
                    expectedCost += ResidualNetwork.NON_PREFERRED_PENALTY;
                }

                Assert.Equal(expectedCost, networkEdge.Cost);

                Assert.True(networkEdge.BackwardEdge.From == networkEdge.To);
                Assert.True(networkEdge.BackwardEdge.To == networkEdge.From);

                Assert.Equal(-networkEdge.Cost, networkEdge.BackwardEdge.Cost);
                Assert.Equal(networkEdge.Capacity, networkEdge.BackwardEdge.Capacity);
                Assert.Equal(networkEdge.Capacity, networkEdge.BackwardEdge.CurrFlow);
            }
        }

    }
}