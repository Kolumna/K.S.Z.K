using Krasnoludki.Core.McmfAlgorithm.Models;
using Krasnoludki.Core.Models;
using Krasnoludki.Core.McmfAlgorithm.Adapter;

using Xunit;
using System.Runtime.CompilerServices;

namespace Krasnoludki.Test;

public class MapperTest
{
    private List<Dwarf> GetSampleFrontendDwarves()
    {
        return new List<Dwarf>
        {
            // --- Standard cases ---
            new Dwarf(1, 10.5, 20.0, new List<MineralType> { MineralType.Gold, MineralType.Silver }, 5),
            new Dwarf(2, 45.0, 12.5, new List<MineralType> { MineralType.Coal }, 3),
            
            // --- Edge cases and unusual values ---
            // No preferences (extreme case)
            new Dwarf(3, 0.0, 0.0, new List<MineralType>(), 10), 
            
            // Negative coordinates on both axes
            new Dwarf(4, -5.0, -15.2, new List<MineralType> { MineralType.Silver }, 2), 
            
            // Large coordinate spread and long preference list
            new Dwarf(5, -100.5, 250.7, new List<MineralType> { MineralType.Gold, MineralType.Coal, MineralType.Silver }, 1), 
            
            // Mixed coordinate signs (+ and -)
            new Dwarf(6, 15.0, -10.0, new List<MineralType> { MineralType.Coal }, 8), 
            
            // Extremely large coordinate values and loudness 0
            new Dwarf(7, 9999.9, 9999.9, new List<MineralType> { MineralType.Gold }, 0), 
            
            // Exact same coordinates as Dwarf 1 (checking if mapping handles stacked objects)
            new Dwarf(8, 10.5, 20.0, new List<MineralType> { MineralType.Silver }, 5)  
        };
    }

    private List<Mine> GetSampleFrontendMines()
    {
        return new List<Mine>
        {
            // --- Standard cases ---
            new Mine(101, 15.0, 25.0, MineralType.Gold, 3),
            new Mine(102, 50.0, 20.0, MineralType.Coal, 5),
            new Mine(103, 10.0, 10.0, MineralType.Silver, 2),
            
            // --- Edge cases ---
            // Depleted/closed mine (capacity 0)
            new Mine(104, 100.0, 100.0, MineralType.Silver, 0), 
            
            // Very large capacity and negative coordinates
            new Mine(105, -10.0, -10.0, MineralType.Coal, 100), 
            
            // Center of the grid (overlaps with Dwarf 3)
            new Mine(106, 0.0, 0.0, MineralType.Gold, 1), 
            
            // Mixed coordinates
            new Mine(107, -50.5, 75.2, MineralType.Silver, 4), 
            
            // Same coordinates as Mine 101, but different resource
            new Mine(108, 15.0, 25.0, MineralType.Coal, 3) 
        };
    }

    [Fact]
    public void TestMapper()
    {
        List<Dwarf> dwarves = GetSampleFrontendDwarves();
        List<Mine> mines = GetSampleFrontendMines();

        McmfMapper mapper = new McmfMapper();

        List<GraphDwarf> mappedDwarves = mapper.MapDwarves(dwarves);
        List<GraphMine> mappedMines = mapper.MapMines(mines);

        Assert.Equal(dwarves.Count, mappedDwarves.Count);
        Assert.Equal(mines.Count, mappedMines.Count);
        
        int index = 0;
        foreach (Dwarf dwarf in dwarves)
        {
            Assert.Equal(dwarf.PointId, mappedDwarves[index].Id);
            Assert.Equal(dwarf.x, mappedDwarves[index].HomeLocation.x);
            Assert.Equal(dwarf.y, mappedDwarves[index].HomeLocation.y);
            Assert.Equal(dwarf.PreferredMinerals, mappedDwarves[index].PreferredMinerals);
            Assert.Equal(dwarf.VoiceLoudness, mappedDwarves[index].VoiceLoudness);
            
            index++;
        }

        index = 0;
        foreach (Mine mine in mines)
        {
            Assert.Equal(mine.PointId, mappedMines[index].Id);
            Assert.Equal(mine.x, mappedMines[index].Location.x);
            Assert.Equal(mine.y, mappedMines[index].Location.y);
            Assert.Equal(mine.Resource, mappedMines[index].Resource);
            Assert.Equal(mine.Capacity, mappedMines[index].Capacity);

            index++;
        }
    }
}