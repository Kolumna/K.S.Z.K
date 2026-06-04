using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;
using Krasnoludki.Core;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test2
    {    
        [Fact]
        public void BFSTest()
        {
            Source source = new Source();   //id = 0
            List<Dwarf> dwarves = new List<Dwarf>
            {
                new Dwarf(1, 1, 1, new List<MineralType> { MineralType.Gold }, 5),                     //id = 1
                new Dwarf(2, 2, 2, new List<MineralType> { MineralType.Quartz, MineralType.Gold}, 3)   //id = 2
            };
            List<Mine> mines = new List<Mine>
            {
                new Mine(3, 3, 3, MineralType.Gold, 2),        //id = 3
                new Mine(4, 4, 4, MineralType.Quartz, 1),      //id = 4
            };
            Sink sink = new Sink(mines.Count() + dwarves.Count());       //id = 5
            

            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            EdgeFlow[] parent = new EdgeFlow[sink.PointId+1];

            bool test = DwarfAssigning.BFS(source, sink, edges, ref parent);

            Assert.True(test);
            Assert.Equal(0, parent[dwarves[0].PointId].From);
            Assert.Equal(0, parent[dwarves[1].PointId].From);
            Assert.Equal(1, parent[mines[0].PointId].From);
            Assert.Equal(2, parent[mines[1].PointId].From);
            Assert.Equal(3, parent[sink.PointId].From);
        }
    }
} 