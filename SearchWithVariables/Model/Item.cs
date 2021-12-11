namespace SearchWithVariables.Model
{
    /// <summary>
    /// Абстрактная сущность - элемент поиска.
    /// Содержит имя и состояние.
    /// </summary>
    internal abstract class Item
    {
        public enum States
        {
            OPEN = 0,
            CLOSED = 1
        }

        public char Label { get; } // имя (метка)
        public States State { get; private set; } // состояние

        protected Item(char label, States state)
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
            return obj is Item item && Label.Equals(item.Label);
        }

        public override int GetHashCode() => Label.GetHashCode();

        public override string ToString()
        {
            return Label.ToString();
        }
    }
}
