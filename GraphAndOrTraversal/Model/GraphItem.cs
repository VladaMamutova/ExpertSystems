namespace GraphAndOrTraversal.Model
{
    /// <summary>
    /// Элемент графа, содержит номер и метку (состояние).
    /// </summary>
    abstract class GraphItem
    {
        /// <summary>
        /// Состояние элемента графа.
        /// </summary>
        public enum States
        {
            OPEN = 0, // открыто
            CLOSED = 1, // закрыто
            FORBIDDEN = -1 // запрещено
        }

        public int Label { get; } // номер
        public States State { get; private set; } // состояние (далее - метка)

        protected GraphItem(int label, States state)
        {
            Label = label;
            State = state;
        }
        public void SetState(States state)
        {
            State = state;
        }

        public override bool Equals(object obj)
        {
            return obj is GraphItem item && Label.Equals(item.Label);
        }

        public override int GetHashCode() => Label.GetHashCode();

        public override string ToString()
        {
            return Label.ToString();
        }
    }
}
