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
            /*List<Point> points = new List<Point>();
            points.Add(new Point(true));
            List<MineralType> pref1 = new List<MineralType> {MineralType.Coal, MineralType.Silver};
            List<MineralType> pref2 = new List<MineralType> {MineralType.Coal, MineralType.Quartz};
            points.Add(new Dwarf(pref1));
            points.Add(new Dwarf(pref2));
            points.Add(new Mine(MineralType.Coal, 2));
            points.Add(new Mine(MineralType.Quartz, 5));
            points.Add(new Point(false, true));

            List<EdgeFlow> edges = DwarfAssigning.GenerateEdges(points);

            Assert.Equal(7, edges.Count);
            Assert.Equal(1, edges[0].Capacity);
            Assert.Equal(1, edges[0].From);
            Assert.Equal(2, edges[0].To);
            Assert.Equal(1, edges[1].Capacity);
            Assert.Equal(1, edges[1].From);
            Assert.Equal(3, edges[1].To);
            Assert.Equal(1, edges[2].Capacity);
            Assert.Equal(2, edges[2].From);
            Assert.Equal(4, edges[2].To);
            Assert.Equal(1, edges[3].Capacity);
            Assert.Equal(3, edges[3].From);
            Assert.Equal(4, edges[3].To);
            Assert.Equal(1, edges[4].Capacity);
            Assert.Equal(3, edges[4].From);
            Assert.Equal(5, edges[4].To);
            Assert.Equal(2, edges[5].Capacity);
            Assert.Equal(4, edges[5].From);
            Assert.Equal(6, edges[5].To);
            Assert.Equal(5, edges[6].Capacity);
            Assert.Equal(5, edges[6].From);
            Assert.Equal(6, edges[6].To);*/
        }
    }
}