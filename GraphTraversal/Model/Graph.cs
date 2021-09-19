using System.Collections.Generic;

namespace DepthFirstSearch.Model
{
    public class Graph
    {
        private static int VertexCount = 0;

        public Dictionary<int, Vertex> vertices { get; }

        public HashSet<int> opened { get; }
        public HashSet<int> closed { get; }

        public Stack<Vertex> Stack { get; }
        public List<Edge> Edges { get; }
        
        public Graph()
        {
            
        }
    }
}