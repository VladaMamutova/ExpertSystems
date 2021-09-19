namespace DepthFirstSearch.Model
{
    public class Vertex
    {
        public enum States
        {
            OPEN = -1,
            CLOSED = 0,
            FORBIDDEN = 1
        }

        public States State { get; private set; }
        public int Label { get; private set; }

        public Vertex(int label)
        {
            State = States.OPEN;
            Label = label;
        }
    }
}
