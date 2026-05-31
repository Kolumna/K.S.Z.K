namespace Krasnoludki.Core.Graph;

public class EdgeGen
{
    public static List<EdgeFlow> GenerateEdges(List<Models.Dwarf> dwarves, List<Models.Mine> mines, int source, int sink)
    {
        EdgeFlow edge;
        List<EdgeFlow> result = new List<EdgeFlow>();
            foreach(Models.Dwarf d in dwarves)
            {
                edge = new EdgeFlow(source, d.HomeLocation.PointId, 1);         //tworzenie krawędzi od source do każdego krasnoludka
                result.Add(edge);       
                //result.Add(edge.BackwardEdge);
                foreach(Models.Mine m in mines)
                {
                    double cost = Math.Sqrt(Math.Pow(d.HomeLocation.x - m.Location.x, 2) + Math.Pow(d.HomeLocation.y - m.Location.y, 2));
                    if (!d.PreferredMinerals.Contains(m.Resource))
                    {
                        cost += 1000000;
                    }
                    edge = new EdgeFlow(d.HomeLocation.PointId, m.Location.PointId, 1, cost);       //tworzenie krawędzi od kranolduka do każdej koaplni
                    result.Add(edge);
                    //result.Add(edge.BackwardEdge);
                }
            }
            foreach(Models.Mine m in mines)            
            {
                edge = new EdgeFlow(m.Location.PointId, sink, m.Capacity);       //tworzenie krawędzi od każdej kopalni do sink
                result.Add(edge);      
                //result.Add(edge.BackwardEdge);
            }
            return result;
        }
}