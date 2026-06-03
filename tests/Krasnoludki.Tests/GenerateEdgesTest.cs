using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;
using Krasnoludki.Core;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test1
    {    
        [Fact]
        public void GenerateEdgesTest()
        {
            Source source = new Source();       //id = 0
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
            Sink sink = new Sink(mines.Count() + dwarves.Count());     //id = 5

            List<EdgeFlow> edges = EdgeGen.GenerateEdges(dwarves, mines, source, sink);

            Assert.Equal(8, edges.Count);

            //od source do dwarves[0]
            Assert.Equal(1, edges[0].Capacity);
            Assert.Equal(0, edges[0].From);
            Assert.Equal(1, edges[0].To);

            //powrotna
            Assert.Equal(1, edges[0].BackwardEdge.Capacity);
            Assert.Equal(1, edges[0].BackwardEdge.From);
            Assert.Equal(0, edges[0].BackwardEdge.To);

            //od dwarves[0] do mines[0]
            Assert.Equal(1, edges[1].Capacity);
            Assert.Equal(1, edges[1].From);
            Assert.Equal(3, edges[1].To);
            Assert.Equal(Math.Sqrt(8), edges[1].Cost);

            //powrotna
            Assert.Equal(1, edges[1].BackwardEdge.Capacity);
            Assert.Equal(3, edges[1].BackwardEdge.From);
            Assert.Equal(1, edges[1].BackwardEdge.To);
            Assert.Equal(-Math.Sqrt(8), edges[1].BackwardEdge.Cost);

            //od dwarves[0] do mines[1]
            Assert.Equal(1, edges[2].Capacity);
            Assert.Equal(1, edges[2].From);
            Assert.Equal(4, edges[2].To);
            Assert.Equal(1000000, edges[2].Cost);

            //powrotna
            Assert.Equal(1, edges[2].BackwardEdge.Capacity);
            Assert.Equal(4, edges[2].BackwardEdge.From);
            Assert.Equal(1, edges[2].BackwardEdge.To);
            Assert.Equal(-1000000, edges[2].BackwardEdge.Cost);

            //od source do dwarves[1]
            Assert.Equal(1, edges[3].Capacity);
            Assert.Equal(0, edges[3].From);
            Assert.Equal(2, edges[3].To);

            //powrotna
            Assert.Equal(1, edges[3].BackwardEdge.Capacity);
            Assert.Equal(2, edges[3].BackwardEdge.From);
            Assert.Equal(0, edges[3].BackwardEdge.To);

            //od dwarves[1] do mines[0]
            Assert.Equal(1, edges[4].Capacity);
            Assert.Equal(2, edges[4].From);
            Assert.Equal(3, edges[4].To);
            Assert.Equal(Math.Sqrt(2), edges[4].Cost);

            //powrotna
            Assert.Equal(1, edges[4].BackwardEdge.Capacity);
            Assert.Equal(3, edges[4].BackwardEdge.From);
            Assert.Equal(2, edges[4].BackwardEdge.To);
            Assert.Equal(-Math.Sqrt(2), edges[4].BackwardEdge.Cost);

            //od dwarves[1] do mines[1]
            Assert.Equal(1, edges[5].Capacity);
            Assert.Equal(2, edges[5].From);
            Assert.Equal(4, edges[5].To);
            Assert.Equal(Math.Sqrt(8), edges[5].Cost);

            //powrotna
            Assert.Equal(1, edges[5].BackwardEdge.Capacity);
            Assert.Equal(4, edges[5].BackwardEdge.From);
            Assert.Equal(2, edges[5].BackwardEdge.To);
            Assert.Equal(-Math.Sqrt(8), edges[5].BackwardEdge.Cost);

            //od mines[0] do sink
            Assert.Equal(2, edges[6].Capacity);
            Assert.Equal(3, edges[6].From);
            Assert.Equal(5, edges[6].To);

            //powrotna
            Assert.Equal(2, edges[6].BackwardEdge.Capacity);
            Assert.Equal(5, edges[6].BackwardEdge.From);
            Assert.Equal(3, edges[6].BackwardEdge.To);

            //od mines[0] do sink
            Assert.Equal(1, edges[7].Capacity);
            Assert.Equal(4, edges[7].From);
            Assert.Equal(5, edges[7].To);

            //powrotna
            Assert.Equal(1, edges[7].BackwardEdge.Capacity);
            Assert.Equal(5, edges[7].BackwardEdge.From);
            Assert.Equal(4, edges[7].BackwardEdge.To);
        }
    }
}