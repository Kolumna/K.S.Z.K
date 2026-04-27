using System.ComponentModel.DataAnnotations.Schema;
using Krasnoludki.Core;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test2
    {    
        [Fact]
        public void AssignTest()
        {
            List<MineralType> pref1 = new List<MineralType> {MineralType.Gold, MineralType.Silver};
            List<MineralType> pref2 = new List<MineralType> {MineralType.Gold, MineralType.Silver};
            List<MineralType> pref3 = new List<MineralType> {MineralType.Gold, MineralType.Coal};
            List<MineralType> pref4 = new List<MineralType> {MineralType.Silver, MineralType.Quartz};
            List<MineralType> pref5 = new List<MineralType> {MineralType.Coal, MineralType.Quartz};
            Point d1 = new Point(new Dwarf(pref1));
            Point d2 = new Point(new Dwarf(pref2));
            Point d3 = new Point(new Dwarf(pref3));
            Point d4 = new Point(new Dwarf(pref4));
            Point d5 = new Point(new Dwarf(pref5));
            Point m1 = new Point(new Mine(MineralType.Gold, 3));
            Point m2 = new Point(new Mine(MineralType.Quartz, 2));

            Point[] points = {d1, d2, d3, d4, d5, m1, m2};

            EdgeWeight[] edges = 
            {
                new EdgeWeight(d1, d2, 1),
                new EdgeWeight(d2, d3, 2),
                new EdgeWeight(d1, d3, 5),
                new EdgeWeight(d3, m1, 2),
                new EdgeWeight(m1, d4, 1),
                new EdgeWeight(d1, m2, 3),
                new EdgeWeight(d5, m2, 10),
                new EdgeWeight(d4, d5, 1),
                new EdgeWeight(d2, m2, 6)
            };

            DwarfAssigning.Assign(points, edges);

            Assert.True(m1.Mine.IsFull);
            Assert.True(m2.Mine.IsFull);
        }
    }
}