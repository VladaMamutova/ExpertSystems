namespace GraphTraversal.Model
{
    class Vertex: GraphItem
    {
        public enum States
        {
            OPEN = -1,
            CLOSED = 0,
            FORBIDDEN = 1
        }

        public States State { get; private set; }
        
        public Vertex(int label): base(label)
        {
            State = States.OPEN;
        }

        public void SetState(States state)
        {
            State = state;
        }
    }
}
