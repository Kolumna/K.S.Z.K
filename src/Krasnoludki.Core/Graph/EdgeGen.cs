namespace Krasnoludki.Core.Graph;

public class EdgeGen
{
    public static List<EdgeFlow> GenerateEdges(List<Models.Dwarf> dwarves, List<Models.Mine> mines, Models.Source source, Models.Sink sink)
    {
        List<EdgeFlow> result = new List<EdgeFlow>();
            foreach(Models.Dwarf d in dwarves)
            {
                result.Add(new EdgeFlow(source, d));      //tworzenie krawędzi od source do każdego krasnoludka 
                foreach(Models.Mine m in mines)
                {
                    result.Add(new EdgeFlow(d, m));       //tworzenie krawędzi od kranolduka do każdej koaplni
                }
            }
            foreach(Models.Mine m in mines)            
            {      
                result.Add(new EdgeFlow(m, sink));      //tworzenie krawędzi od każdej kopalni do sink
            }
            return result;
        }
}