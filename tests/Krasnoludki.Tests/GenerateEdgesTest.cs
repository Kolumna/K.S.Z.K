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
            /*Source source = new Source();       //id = 1
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
            Sink sink = new Sink();     //id = 6

            List<EdgeFlow> edges = DwarfAssigning.GenerateEdges(dwarves, mines, source, sink);

            Assert.Equal(8, edges.Count);

            //od source do dwarves[0]
            Assert.Equal(1, edges[0].Capacity);
            Assert.Equal(1, edges[0].From);
            Assert.Equal(2, edges[0].To);

            //od dwarves[0] do mines[0]
            Assert.Equal(1, edges[1].Capacity);
            Assert.Equal(2, edges[1].From);
            Assert.Equal(4, edges[1].To);
            Assert.Equal(Math.Sqrt(8), edges[1].Cost);

            //od dwarves[0] do mines[1]
            Assert.Equal(1, edges[2].Capacity);
            Assert.Equal(2, edges[2].From);
            Assert.Equal(5, edges[2].To);
            Assert.Equal(1000000, edges[2].Cost);

            //od source do dwarves[1]
            Assert.Equal(1, edges[3].Capacity);
            Assert.Equal(1, edges[3].From);
            Assert.Equal(3, edges[3].To);

            //od dwarves[1] do mines[0]
            Assert.Equal(1, edges[4].Capacity);
            Assert.Equal(3, edges[4].From);
            Assert.Equal(4, edges[4].To);
            Assert.Equal(Math.Sqrt(2), edges[4].Cost);

            //od dwarves[1] do mines[1]
            Assert.Equal(1, edges[5].Capacity);
            Assert.Equal(3, edges[5].From);
            Assert.Equal(5, edges[5].To);
            Assert.Equal(Math.Sqrt(8), edges[5].Cost);

            //od mines[0] do sink
            Assert.Equal(2, edges[6].Capacity);
            Assert.Equal(4, edges[6].From);
            Assert.Equal(6, edges[6].To);

            //od mines[0] do sink
            Assert.Equal(1, edges[7].Capacity);
            Assert.Equal(5, edges[7].From);
            Assert.Equal(6, edges[7].To);*/
        }
    }
}