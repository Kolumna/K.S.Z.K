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

            List<int[]> assigned = DwarfAssigning.Assign(dwarves, mines);

            Assert.Equal(2, assigned.Count());      //ile krasnoludków przydzielono
            //pierwszy przydzielony krasnoludek
            Assert.Equal(1, assigned[0][0]);
            Assert.Equal(4, assigned[0][1]);
            //drugi przydzielony krasnoludek
            Assert.Equal(2, assigned[1][0]);
            Assert.Equal(3, assigned[1][1]);
        }
    }
}