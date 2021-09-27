namespace GraphTraversal.Model
{
    class Edge: GraphItem
    {
        public enum States
        {
            OPEN = -1,
            CLOSED = 0,
            FORBIDDEN = 1
        }

        public States State { get; private set; }

        public Vertex Start { get; }
        public Vertex End { get; }

        public Edge(int label, Vertex start, Vertex end): base(label)
        {
            Start = start;
            End = end;
        }

        public void SetState(States state)
        {
            State = state;
        }

        public override string ToString()
        {
            return $"{Start} ---{Label}--> {End}";
        }
    }
}
