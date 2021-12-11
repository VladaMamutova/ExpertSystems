using System.Collections.Generic;

namespace SearchWithVariables.Model
{
    /// <summary>
    /// Класс вершины.
    /// Содержит имя, состояние и список аргументов.
    /// </summary>
    class Vertex : Item
    {
        public List<Argument> Arguments; // список аргументов

        public Vertex(char label, List<string> arguments, States state = States.OPEN)
            : base(label, state)
        {
            Arguments = arguments.ConvertAll(value => new Argument(value));
        }

        public Vertex(char label, Argument[] arguments, States state = States.OPEN)
            : base(label, state)
        {
            Arguments = new List<Argument>(arguments);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vertex vertex)) return false;

            if (Label != vertex.Label) return false;
            if (Arguments.Count != vertex.Arguments.Count) return false;

            for (var i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals( vertex.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Label.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString() + "(" + string.Join(", ", Arguments) + ")";
        }
    }
}
