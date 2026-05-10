/*using System.ComponentModel.DataAnnotations.Schema;
using Krasnoludki.Core;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test1
    {    
        [Fact]
        public void DijkstraTest()
        {
            List<MineralType> pref1 = new List<MineralType> {MineralType.Gold, MineralType.Quartz};
            List<MineralType> pref2 = new List<MineralType> {MineralType.Gold, MineralType.Silver};
            List<MineralType> pref3 = new List<MineralType> {MineralType.Gold, MineralType.Coal};
            Point d1 = new Point(new Dwarf(pref1));
            Point d2 = new Point(new Dwarf(pref2));
            Point d3 = new Point(new Dwarf(pref3));
            Point m1 = new Point(new Mine(MineralType.Gold, 3));

            Point[] points = {d1, d2, d3, m1};

            EdgeWeight[] edges = 
            {
                new EdgeWeight(d1, d2, 1),
                new EdgeWeight(d2, d3, 2),
                new EdgeWeight(d1, d3, 5),
                new EdgeWeight(d3, m1, 2)    
            };

            int[] result = DwarfAssigning.Dijkstra(d2, points, edges);
            Assert.Equal(1, result[0]);
            Assert.Equal(0, result[1]);
            Assert.Equal(2, result[2]);
            Assert.Equal(4, result[3]);
        }
    }
}*/