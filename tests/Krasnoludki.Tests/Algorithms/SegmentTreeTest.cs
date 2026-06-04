using Krasnoludki.Core.Algorithms;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Tests;

public class SegmentTreeTest
{
    [Fact]
    public void test()
    {
        var decametrists = new List<Dwarf>
        {
            new Dwarf(1, 0, 0, new List<MineralType>(), 12),
            new Dwarf(2, 0, 0, new List<MineralType>(), 47),
            new Dwarf(3, 0, 0, new List<MineralType>(), 8),
            new Dwarf(4, 0, 0, new List<MineralType>(), 33),
            new Dwarf(5, 0, 0, new List<MineralType>(), 55),
            new Dwarf(6, 0, 0, new List<MineralType>(), 21),
            new Dwarf(7, 0, 0, new List<MineralType>(), 3)
        };

        var tree = new SegmentTree(decametrists);

        Dwarf commander = tree.GetLoudestDecametrist();
        Dwarf localCommander = tree.GetLoudestDecametrist(0, 3);

        Assert.Equal(5, commander.PointId);
        Assert.Equal(55, commander.getLoudness());

        Assert.Equal(2, localCommander.PointId);
        Assert.Equal(47, localCommander.getLoudness());
    }
}