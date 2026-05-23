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
            /*List<Point> points = new List<Point>();
            points.Add(new Point());
            List<MineralType> pref1 = new List<MineralType> {MineralType.Coal, MineralType.Silver};
            List<MineralType> pref2 = new List<MineralType> {MineralType.Coal, MineralType.Quartz};
            points.Add(new Dwarf(pref1));
            points.Add(new Dwarf(pref2));
            points.Add(new Mine(MineralType.Coal, 1));
            points.Add(new Mine(MineralType.Silver, 1));
            
            
            points.Add(new Point());

            List<EdgeFlow> edges = DwarfAssigning.GenerateEdges(points);

            int test = DwarfAssigning.EdmondsKarp(1, points.Count, edges);
            
            Assert.Equal(2, test);*/
        }
    }
}