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
            /*Sink sink = new Sink();     //id = 1
            List<Dwarf> dwarves = new List<Dwarf>
            {
                new Dwarf(1, 1, new List<MineralType> { MineralType.Gold }, 5),                     //id = 2
                new Dwarf(2, 2, new List<MineralType> { MineralType.Quartz, MineralType.Gold}, 3)   //id = 3
            };
            List<Mine> mines = new List<Mine>
            {
                new Mine(3, 3, MineralType.Gold, 2),        //id = 4
                new Mine(4, 4, MineralType.Quartz, 1),      //id = 5
            };
            Source source = new Source();       //id = 6
            

            List<EdgeFlow> edges = DwarfAssigning.GenerateEdges(dwarves, mines, source, sink);

            int[] parent = new int[sink.HowManyPoints()];

            bool test = DwarfAssigning.BFS(source, sink, edges, ref parent);

            Assert.True(test);
            Assert.Equal(-1, parent[source.PointId-1]);     //-1 ponieważ tablica jest indeksowana od 0, a punkty od 1
            Assert.Equal(6, parent[dwarves[0].PointId-1]);
            Assert.Equal(6, parent[dwarves[1].PointId-1]);
            Assert.Equal(2, parent[mines[0].PointId-1]);
            Assert.Equal(2, parent[mines[1].PointId-1]);
            Assert.Equal(4, parent[sink.PointId-1]);*/
        }
    }
} 