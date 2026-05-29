using System.ComponentModel.DataAnnotations.Schema;
using Krasnoludki.Core;
using Krasnoludki.Core.Graph;
using Krasnoludki.Core.Models;
using Xunit;
using Xunit.Sdk;

namespace Krasnoludki.Tests
{
    public class Test4
    {    
        [Fact]
        public void AssignTest()
        {
            List<Dwarf> dwarves = new List<Dwarf>
            {
                new Dwarf(1, 1, 1, new List<MineralType> {MineralType.Coal, MineralType.Silver}, 1),
                new Dwarf(2, 2, 2, new List<MineralType> {MineralType.Coal, MineralType.Quartz}, 1)
            };
            List<Mine> mines = new List<Mine>
            {
                new Mine(3, 3, 3, MineralType.Coal, 1),
                new Mine(4, 4, 4, MineralType.Silver, 1)
            };

            DwarfAssigning.Assign(dwarves, mines);
            Assert.True(dwarves[1].WorksIn.PointId == mines[0].PointId);
            Assert.True(dwarves[0].WorksIn.PointId == mines[1].PointId);
        }
    }
}