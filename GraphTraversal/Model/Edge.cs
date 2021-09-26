namespace GraphTraversal.Model
{
    class Edge: GraphItem
    {
        public Vertex Start { get; }
        public Vertex End { get; }

        public Edge(int label, Vertex start, Vertex end): base(label)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start} ---{Label}--> {End}";
        }
    }
}
