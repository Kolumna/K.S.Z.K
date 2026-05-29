namespace Krasnoludki.Core.Graph;

public class EdgeGen
{
    public static List<EdgeFlow> GenerateEdges(List<Models.Dwarf> dwarves, List<Models.Mine> mines, Models.Source source, Models.Sink sink)
    {
        EdgeFlow edge;
        List<EdgeFlow> result = new List<EdgeFlow>();
            foreach(Models.Dwarf d in dwarves)
            {
                edge = new EdgeFlow(source, d);         //tworzenie krawędzi od source do każdego krasnoludka
                result.Add(edge);       
                result.Add(edge.BackwardEdge);
                foreach(Models.Mine m in mines)
                {
                    edge = new EdgeFlow(d, m);       //tworzenie krawędzi od kranolduka do każdej koaplni                    if(!d.PreferredMinerals.Contains(m.Resource)) newEdge.BadResource();    //ustawiania sztucznie wysokiego dystansu w przypadku niezgodności surowców i preferencji
                    result.Add(edge);
                    result.Add(edge.BackwardEdge);
                }
            }
            foreach(Models.Mine m in mines)            
            {
                edge = new EdgeFlow(m, sink);       //tworzenie krawędzi od każdej kopalni do sink
                result.Add(edge);      
                result.Add(edge.BackwardEdge);
            }
            return result;
        }
}