namespace GraphAndOrTraversal.Model
{
    abstract class GraphItem
    {
        public enum States
        {
            OPEN = 0,
            CLOSED = 1,
            FORBIDDEN = -1
        }

        public int Label { get; }
        public States State { get; private set; }

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
