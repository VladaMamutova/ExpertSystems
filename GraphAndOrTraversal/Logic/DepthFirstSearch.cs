using System;
using System.Collections.Generic;
using System.Linq;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal.Logic
{
    /// <summary>
    /// Depth-First Search
    /// </summary>
    class DepthFirstSearch: IGraphTraverser
    {
        private readonly Stack<Vertex> _openedVertices;
        private readonly Stack<Vertex> _closedVertices;
        private readonly Stack<Vertex> _forbiddenVertices;

        private readonly Stack<Edge> _openedRules;
        private readonly Stack<Edge> _closedRules;
        private readonly Stack<Edge> _forbiddenRules;

        public Graph Graph { get; }
        public Vertex[] Source { get; private set; }
        public Vertex Target { get; private set; }

        public List<Edge> Traversal { get; private set; }

        public bool DebugMode { get; set; }

        public DepthFirstSearch(Graph graph)
        {
            Graph = graph;
            Source = null;
            Target = null;
            Traversal = null;
            DebugMode = false;

            _openedVertices = new Stack<Vertex>();
            _closedVertices = new Stack<Vertex>();
            _forbiddenVertices = new Stack<Vertex>();

            _openedRules = new Stack<Edge>();
            _closedRules = new Stack<Edge>();
            _forbiddenRules = new Stack<Edge>();
        }

        public DepthFirstSearch(Graph graph, int[] source, int target): this(graph)
        {
            SetSource(source);
            SetTarget(target);
        }

        private void ValidateSourceAndTarget()
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

        private void Initialize()
        {
            _openedVertices.Clear();
            _closedVertices.Clear();
            _forbiddenVertices.Clear();

            _openedRules.Clear();
            _closedRules.Clear();
            _forbiddenRules.Clear();

            Graph.Edges.ForEach(edge =>
            {
                edge.Out.SetState(GraphItem.States.OPEN);
                foreach (var vertex in edge.In)
                {
                    vertex.SetState(GraphItem.States.OPEN);
                }
                edge.SetState(GraphItem.States.OPEN);
            });

            foreach (var source in Source.Reverse())
            {
                source.SetState(GraphItem.States.CLOSED);
                _closedVertices.Push(source);
            }
            _openedVertices.Push(Target);
        }
        
        public void SetSource(int[] labels)
        {
            Source = labels.Select(label => Graph.FindVertex(label)).ToArray();
        }

        public void SetTarget(int label)
        {
            Target = Graph.FindVertex(label);
        }

        /// <summary>
        /// Метод потомков. Находит первое правило с меткой OPEN (т.е. правило ещё
        /// не выбиралось и не было запрещено), выходная вершина которого
        /// равна заданной вершине.
        /// </summary>
        /// <param name="targetVertex">Раскрываемая вершина.</param>
        /// <returns>Правило с состоянием OPEN и выходной вершиной, равной заданной.</returns>
        public GraphItem FindDescendant(GraphItem targetVertex)
        {
            foreach (var rule in Graph.Edges)
            {
                if (rule.Out.Equals(targetVertex) &&
                    rule.State == GraphItem.States.OPEN)
                {
                    return rule;
                }
            }

            return null;
        }

        /// <summary>
        /// Метод разметки. Проверяет, являются ли все входные вершины правила доказанными.
        /// Все недоказанные входные вершины правила помещает в стек открытых вершин.
        /// </summary>
        /// <param name="edge">Ребро графа (правило) для проверки.</param>
        /// <returns>Признак, показывающий, являются ли
        /// все входные вершины правила доказаны.</returns>
        public bool Markup(GraphItem edge)
        {
            bool ruleIsProven = true;

            var rule = (Edge)edge;
            foreach (var inVertex in rule.In)
            {
                if (!_closedVertices.Contains(inVertex))
                {
                    ruleIsProven = false;
                    _openedVertices.Push(inVertex);
                }
            }

            return ruleIsProven;
        }

        public IEnumerable<GraphItem> Traverse()
        {
            bool solved = false;
            ValidateSourceAndTarget();
            Initialize();

            do
            {
                var head = _openedVertices.Peek();

                var rule = (Edge)FindDescendant(head);
                if (rule == null)
                {
                    head.SetState(GraphItem.States.FORBIDDEN);
                    _forbiddenVertices.Push(_openedVertices.Pop());

                    if (_openedRules.Count > 0)
                    {
                        rule = _openedRules.Pop();
                        rule.SetState(GraphItem.States.FORBIDDEN);
                        _forbiddenRules.Push(rule);
                        int openedInVertices = rule.In.Count(inVertex =>
                            inVertex.State == GraphItem.States.OPEN);
                        for (int i = 0; i < openedInVertices; i++)
                        {
                            _openedVertices.Pop();
                        }
                    }
                }
                else
                {
                    _openedRules.Push(rule);
                    if (Markup(rule))
                    {
                        _openedVertices.Pop();
                        _openedRules.Pop();
                        rule.Out.SetState(GraphItem.States.CLOSED);
                        _closedVertices.Push(rule.Out);

                        rule.SetState(GraphItem.States.CLOSED);
                        _closedRules.Push(rule);

                        solved = rule.Out.Equals(Target);
                    }
                }
                
            } while (_openedVertices.Count > 0 && !solved);

            Traversal = solved ? _closedRules.ToList() : null;
            return Traversal;
        }
    }
}
