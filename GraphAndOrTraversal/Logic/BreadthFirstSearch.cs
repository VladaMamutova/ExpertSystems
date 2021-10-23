using System;
using System.Collections.Generic;
using System.Linq;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal.Logic
{
    /// <summary>
    /// Breadth-First Search (Поиск в ширину)
    /// </summary>
    class BreadthFirstSearch : IGraphTraverser
    {
        private readonly Queue<Vertex> _openedVertices; // очередь открытых вершин
        private readonly Queue<Edge> _openedRules; // очередь открытых вершин
        private readonly Queue<Edge> _closedRules; // очередь закрытых правил

        public Graph Graph { get; } // исходный граф, содержит список вершин и правил
        public Vertex[] Source { get; private set; } // список исходных вершин
        public Vertex Target { get; private set; } // целевая вершина

        /// <summary>
        /// Полученный обход графа в ширину,
        /// равен списку закрытых правил, если решение найдено, иначе - null.
        /// </summary>
        public List<Edge> Traversal { get; private set; } 

        public bool DebugMode { get; set; }

        public BreadthFirstSearch(Graph graph)
        {
            Graph = graph;
            Source = null;
            Target = null;
            Traversal = null;
            DebugMode = false;

            _openedVertices = new Queue<Vertex>();
            _openedRules = new Queue<Edge>();
            _closedRules = new Queue<Edge>();
        }

        public BreadthFirstSearch(Graph graph, int[] source, int target): this(graph)
        {
            SetSource(source);
            SetTarget(target);
        }

        public void SetSource(int[] labels)
        {
            Source = labels.Select(label => Graph.FindVertex(label)).ToArray();
        }

        public void SetTarget(int label)
        {
            Target = Graph.FindVertex(label);
        }

        public void ValidateSourceAndTarget()
        {
            if (Source == null || Source.Length == 0 || Target == null)
            {
                throw new Exception("Source vertices or target vertex are not defined");
            }

            if (Source.Contains(Target))
            {
                throw new Exception(
                    "Incorrect target! The target vertex is equal to one of the source vertices");
            }
        }

        /// <summary>
        /// Инициализация алгоритма поиска в ширину (для повторного запуске алгоритма).
        /// </summary>
        private void Initialize()
        {
            // Очищаем списки открытых правил.
            _openedVertices.Clear();
            _openedRules.Clear();
            _closedRules.Clear();

            // Перебираем все правила графа.
            foreach(var rule in Graph.Edges)
            {
                // Отмечаем выходную вершину правила как открыта.
                rule.Out.SetState(GraphItem.States.OPEN);

                // Отмечаем каждую выходную вершина как открыта.
                foreach (var vertex in rule.In)
                {
                    vertex.SetState(GraphItem.States.OPEN);
                }

                // Устанавливаем метку правила "открыта".
                rule.SetState(GraphItem.States.OPEN);
            }

            // Все исходные вершины помечаем как закрытые.
            foreach (var source in Source)
            {
                source.SetState(GraphItem.States.CLOSED);
            }

            // Заносим целевую вершину в список открытых вершин.
            _openedVertices.Enqueue(Target);
        }
        
        /// <summary>
        /// Проверяет, найдено ли решение, с учётом новой доказанной вершины.
        /// </summary>
        /// <param name="provenVertex">Доказанная вершина.</param>
        /// <returns>true, если решение найдено, false - иначе.</returns>
        private bool HasSolution(Vertex provenVertex)
        {
            bool hasSolution = false; // флаг решения

            // Создаём временную очередь доказанных вершин,
            // помещая в неё вновь доказанную.
            Queue<Vertex> provenVertices = new Queue<Vertex>();
            provenVertices.Enqueue(provenVertex);

            // Пока временный список не пуст и решение не найдено.
            while (provenVertices.Count > 0 && !hasSolution)
            {
                // Извлекаем доказанную вершину из головы очереди.
                provenVertex = provenVertices.Dequeue();

                // Перебираем все правила графа.
                foreach (var rule in Graph.Edges)
                {
                    // Пропускаем уже доказанное правило.
                    if(rule.State == GraphItem.States.CLOSED) continue;

                    // Если список входных вершин правила содержит текущую доказанную вершину
                    // и число открытых входных вершин правила равно 0.
                    if (rule.In.Contains(provenVertex) &&
                        rule.In.Count(vertex =>
                            vertex.State == GraphItem.States.OPEN) == 0)
                    {
                        // Помечаем правило как доказанные, добавляем его в список закрытых.
                        rule.SetState(GraphItem.States.CLOSED);
                        _closedRules.Enqueue(rule);

                        // Помечаем выходную вершину правила как доказанную,
                        // помещаем её во временный список доказанных вершин.
                        rule.Out.SetState(GraphItem.States.CLOSED);
                        provenVertices.Enqueue(rule.Out);

                        // Обновляем флаг решения, сравнивая выходную вершину правила с целевой.
                        hasSolution = rule.Out.Equals(Target);
                    }
                }
            }

            return hasSolution;
        }

        /// <summary>
        /// Метод потомков. Находит все открытые правила, выходная вершина
        /// которого равна заданной, и помещает его в список открытых правил.
        /// </summary>
        /// <param name="targetVertex">Раскрываемая вершина.</param>
        /// <returns>Первое правило с состоянием OPEN и выходной вершиной, равной заданной.</returns>
        public GraphItem FindDescendant(GraphItem targetVertex)
        {
            foreach (var rule in Graph.Edges)
            {
                if (rule.Out.Equals(targetVertex) &&
                    rule.State == GraphItem.States.OPEN)
                {
                    _openedRules.Enqueue(rule);
                }
            }

            return _openedRules.Count > 0 ? _openedRules.Peek() : null;
        }

        /// <summary>
        /// Метод разметки. Проверяет, являются ли все входные вершины правила доказанными.
        /// Все недоказанные входные вершины правила помещает в очередь открытых вершин.
        /// </summary>
        /// <param name="rule">Правило графа для проверки.</param>
        /// <returns>true, если все входные вершины правила доказаны, false - иначе.</returns>
        public bool Markup(GraphItem rule)
        {
            bool ruleIsProven = true; // флаг доказанности правила

            // Перебираем все входные вершины правила.
            foreach (var inVertex in ((Edge)rule).In)
            {
                // Если вершина открыта (не доказана).
                if (inVertex.State == GraphItem.States.OPEN)
                {
                    ruleIsProven = false; // правило не доказано
                    // Входную вершину помещаем в очередь открытых вершин.
                    if (!_openedVertices.Contains(inVertex))
                    {
                        _openedVertices.Enqueue(inVertex);
                    }
                }
            }

            return ruleIsProven;
        }

        /// <summary>
        /// Метод поиска (обхода) в ширину.
        /// </summary>
        /// <returns>список доказанных правил, если решение найдено, null - иначе.</returns>
        public IEnumerable<GraphItem> Traverse()
        {
            bool solved = false;
            ValidateSourceAndTarget();
            Initialize();

            do
            {
                // Извлекаем первую вершину очереди открытых вершин.
                var head = _openedVertices.Dequeue();

                // Находим потомков - все правила, раскрывающие текущую вершину.
                if (FindDescendant(head) != null) // правила найдены и записаны в очередь открытых правил
                {
                    // Пока очередь открытых правил не пуста и решение не найдено.
                    while (_openedRules.Count > 0 && !solved)
                    {
                        // Извлекаем правило из очереди открытых правил.
                        var rule = _openedRules.Dequeue();

                        // Вызываем метод разметки, в котором проверяем
                        // покрытие входов правила доказанными вершинами.
                        if (Markup(rule)) // все входные вершины правила доказаны
                        {
                            // Помечаем правило и его выходную вершины как закрытые (доказанные)
                            // добавляем правило в список закрытых.
                            rule.SetState(GraphItem.States.CLOSED);
                            rule.Out.SetState(GraphItem.States.CLOSED);
                            _closedRules.Enqueue(rule);

                            // Обновляем флаг решения:
                            // если выходная вершина равно целевой, решение найдено,
                            // иначе - вызываем метод проверки наличия решения.
                            solved = rule.Out.Equals(Target) || HasSolution(rule.Out);
                        }
                    }
                }
            } while (_openedVertices.Count > 0 && !solved);

            Traversal = solved? _closedRules.ToList() : null;
            return Traversal;
        }
    }
}
