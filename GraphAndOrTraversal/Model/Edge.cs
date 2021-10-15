using System.Linq;

namespace GraphAndOrTraversal.Model
{
    class Edge: GraphItem
    {
        public Vertex[] In { get; }
        public Vertex Out { get; }

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
