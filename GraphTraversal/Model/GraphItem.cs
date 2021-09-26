namespace GraphTraversal.Model
{
    abstract class GraphItem
    {
        public int Label { get; }

        protected GraphItem(int label)
        {
            Label = label;
        }

        //public abstract object Accept(INodeVisitor visitor);

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
