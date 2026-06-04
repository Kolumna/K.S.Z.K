using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;
using Krasnoludki.Core;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test3
    {    
        [Fact]
        public void EdmondsKarpTest()
        {
            Source source = new Source();
            List<Dwarf> dwarves = new List<Dwarf>
            {
                new Dwarf(1, 1, 1, new List<MineralType> {MineralType.Coal, MineralType.Silver}, 1),
                new Dwarf(2, 2, 2, new List<MineralType> {MineralType.Coal, MineralType.Quartz}, 1)
            };
            List<Mine> mines = new List<Mine>
            {
                new Mine(3, 3, 3, MineralType.Coal, 1),
                new Mine(4, 4, 4, MineralType.Silver, 1)
            };

            Sink sink = new Sink(mines.Count() + dwarves.Count());


            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            Assert.Equal(2, DwarfAssigning.EdmondsKarp(source, sink, edges));
        }
    }
}