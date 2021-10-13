namespace GraphAndOrTraversal.Model
{
    class Vertex : GraphItem
    {
        public Vertex(int label, States state = States.OPEN)
            : base(label, state)
        {
        }
    }
}
