public class GraphNode<T>
{
  public int GraphId { get; }
  public T Data { get; }

  public GraphNode(int graphId, T data)
  {
    GraphId = graphId;
    Data = data;
  }
}