namespace GraphAndOrTraversal.Model
{
    /// <summary>
    /// Вершина графа, содержит номер и метку (состояние).
    /// </summary>
    class Vertex : GraphItem
    {
        public Vertex(int label, States state = States.OPEN)
            : base(label, state)
        {
        }
    }
}
