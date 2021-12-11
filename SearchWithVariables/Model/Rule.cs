using System.Collections.Generic;

namespace SearchWithVariables.Model
{
    /// <summary>
    /// Класс правила.
    /// Содержит имя состояние, список входных вершин-предикатов (конънктов)
    /// и выходную вершину (результат).
    /// </summary>
    class Rule : Item
    {
        public List<Vertex> Predicates { get; } // список конъюнктов (предикатов)
        public Vertex Result { get; } // результат (выходная вершина)

        public Rule(char label, List<Vertex> predicates, Vertex result,
            States state = States.OPEN) : base(label, state)
        {
            Predicates = predicates;
            Result = result;
        }

        public override string ToString()
        {
            return $"{Label}) {string.Join(" ∧ ", Predicates)} -> {Result}";
        }
    }
}
