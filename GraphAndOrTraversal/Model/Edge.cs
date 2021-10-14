using System.Linq;

namespace GraphAndOrTraversal.Model
{
    class Edge: GraphItem
    {
        public Vertex[] Start { get; }
        public Vertex End { get; }

        public Edge(int label, Vertex[] start, Vertex end,
            States state = States.OPEN) : base(label, state)
        {
            Start = start;
            End = end;
        }

        public string Print()
        {
            return $"{End} ---{Label}--> {{" +
                   string.Join(", ",
                       Start.Select(vertex => vertex.ToString())) +
                   "}";
        }
    }
}
