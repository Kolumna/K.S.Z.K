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

            int[] parent = new int[points.Count+1];

            bool test = DwarfAssigning.BFS(1, points.Count, edges, ref parent);

            Assert.True(test);
            Assert.Equal(-1, parent[0]);
            Assert.Equal(-1, parent[1]);
            Assert.Equal(1, parent[2]);
            Assert.Equal(1, parent[3]);
            Assert.Equal(2, parent[4]);
            Assert.Equal(3, parent[5]);
            Assert.Equal(4, parent[6]);*/
        }
    }
}