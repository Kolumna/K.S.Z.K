using System.Drawing;
using Krasnoludki.Core;
using Krasnoludki.Core.Models;

namespace Krasnoludki.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        
        List<MineralType> pref1 = new List<MineralType> {MineralType.Gold, MineralType.Silver};
        List<MineralType> pref2 = new List<MineralType> {MineralType.Gold, MineralType.Silver};
        List<MineralType> pref3 = new List<MineralType> {MineralType.Gold, MineralType.Coal};
        //Krasnoludki.Core.Models.Point[] arr = { new Dwarf(pref1), new Dwarf(pref2), new Mine(MineralType.Coal, 4)};

    }
}
