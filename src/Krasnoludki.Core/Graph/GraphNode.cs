public class GraphNode<T>:IGraphNode
{
  public int GraphId { get; }
  public T Data { get; }

  public GraphNode(int graphId, T data)
  {
    GraphId = graphId;
    Data = data;
  }
}