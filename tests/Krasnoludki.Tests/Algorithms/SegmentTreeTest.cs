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
            new(12), new(47), new(8), new(33), new(55), new(21), new(3)
        };

        var tree = new SegmentTree(decametrists);

        Dwarf commander = tree.GetLoudestDecametrist();
        Dwarf localCommander = tree.GetLoudestDecametrist(0, 3);

        Assert.Equal(5, commander.getId());
        Assert.Equal(55, commander.getLoudness());

        Assert.Equal(2, localCommander.getId());
        Assert.Equal(47, localCommander.getLoudness());
    }
}