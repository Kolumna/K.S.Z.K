using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Xunit;

namespace Krasnoludki.Test;

public class ResidualNetworkTest
{
    [Fact]
    public void ResidualNetwork_Structure()
    {
        List<Dwarf> dwarves = new List<Dwarf>();

        Dwarf dwarf1 = new Dwarf(1,2,new List<MineralType>{MineralType.Coal,MineralType.Gold},12);
        Dwarf dwarf2 = new Dwarf(3,4,new List<MineralType>{MineralType.Quartz,MineralType.Gold},10);
        dwarves.Add(dwarf1);
        dwarves.Add(dwarf2);

        List<Mine> mines = new List<Mine>();

        Mine mine1 = new Mine(6,5,MineralType.Coal,40);
        Mine mine2 = new Mine(7,5,MineralType.Coal,10);
        mines.Add(mine1);
        mines.Add(mine2);

        List<List<int>> distances = new List<List<int>>
        {
            new List<int> { 10, 15 },
            new List<int> { 20, 25 }
        };
        var network = new ResidualNetwork(dwarves,mines,distances);

        Assert.Equal(6,network.Nodes.Count);
        Assert.Equal(16,network.Edges.Count);
        Assert.Equal(network.SourceID,network.Nodes[0].GraphId);
        Assert.Equal(network.SinkID,network.Nodes[network.Nodes.Count - 1].GraphId);
    }
}