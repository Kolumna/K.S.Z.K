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
            /*Source source = new Source();
            Sink sink = new Sink();
            List<Dwarf> dwarves = new List<Dwarf>
            {
                new Dwarf(1, 1, new List<MineralType> {MineralType.Coal, MineralType.Silver}, 1),
                new Dwarf(2, 2, new List<MineralType> {MineralType.Coal, MineralType.Quartz}, 1)
            };
            List<Mine> mines = new List<Mine>
            {
                new Mine(3, 3, MineralType.Coal, 1),
                new Mine(4, 4, MineralType.Silver, 1)
            };


            List<EdgeFlow> edges = DwarfAssigning.GenerateEdges(dwarves, mines, source, sink);

            int test = DwarfAssigning.EdmondsKarp(source, sink, edges);
            
            Assert.Equal(2, test);*/
        }
    }
}