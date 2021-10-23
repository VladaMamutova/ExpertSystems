using System.Linq;

namespace GraphAndOrTraversal.Model
{
    /// <summary>
    /// Правило (ребро графа)
    /// </summary>
    class Edge: GraphItem
    {
        public Vertex[] In { get; } // массив открытых вершин
        public Vertex Out { get; } // выходная вершина

        public Edge(int label, Vertex[] inVertices, Vertex outVertex,
            States state = States.OPEN) : base(label, state)
        {
            In = inVertices;
            Out = outVertex;
        }

        public string Print()
        {
            return $"{Out} ---{Label}--> {{" +
                   string.Join(", ",
                       In.Select(vertex => vertex.ToString())) +
                   "}";
        }
    }
}
