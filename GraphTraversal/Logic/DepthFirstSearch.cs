using System;
using System.Collections.Generic;
using System.Linq;
using GraphTraversal.Model;

namespace GraphTraversal.Logic
{
    /// <summary>
    /// Depth-First Search
    /// </summary>
    class DepthFirstSearch: IGraphTraverser
    {
        public Graph Graph { get; }
        public Vertex Source { get; private set; }
        public Vertex Target { get; private set; }

        public List<Vertex> Traversal { get; private set; }

        public bool DebugMode { get; set; }

        public DepthFirstSearch(Graph graph)
        {
            Graph = graph;
            Traversal = null;
            DebugMode = false;
        }

        public DepthFirstSearch(Graph graph, int source, int target)
        {
            Graph = graph;
            SetSource(source);
            SetTarget(target);
            Traversal = null;
            DebugMode = false;
        }

        private Vertex FindDescendant(Vertex source)
        {
            foreach (var edge in Graph.Edges)
            {
                if (edge.Start.Equals(source) &&
                    edge.End.State == Vertex.States.OPEN)
                {
                    return edge.End;
                }
            }

            return null;
        }

        private void PrintDebugLog(string message)
        {
            if (DebugMode)
            {
                Console.Write(message);
            }
        }

        public void SetSource(int label)
        {
            var vertex = Graph.FindVertex(label);
            Source = vertex ?? throw new Exception(
                         $"Vertex with label '{label}' not found in the graph");
        }

        public void SetTarget(int label)
        {
            var vertex = Graph.FindVertex(label);
            if (vertex == null)
            {
                throw new Exception(
                    $"Vertex with label '{label}' not found in the graph");
            }
            if (Source.Equals(vertex))
            {
                throw new Exception(
                    "Incorrect target! The target vertex is equal to the source vertex");
            }

            Target = vertex;
        }

        public IEnumerable<GraphItem> Traverse()
        {
            if (Source == null || Target == null)
            {
                throw new Exception("The source or target vertex is not defined");
            }

            PrintDebugLog("\nDepth First Search Algorithm:\n");

            bool solved = false;
            var opened = new Stack<Vertex>(Graph.Vertices.Count);
            opened.Push(Source);

            do
            {
                var head = opened.Peek();
                head.SetState(Vertex.States.CLOSED);
                PrintDebugLog(head.ToString());

                var descendant = FindDescendant(head);
                if (descendant == null)
                {
                    head = opened.Pop();
                    head.SetState(Vertex.States.FORBIDDEN);
                    PrintDebugLog(" <- ");
                }
                else
                {
                    opened.Push(descendant);
                    solved = descendant.Equals(Target);
                    PrintDebugLog(" -> ");
                }
            } while (opened.Count > 1 && !solved);

            PrintDebugLog(solved ? $"{opened.Peek()} -> solved\n": " no solution\n");
            Traversal = solved ? opened.Reverse().ToList() : null;
            return Traversal;
        }
        
        public void PrintTraversal()
        {
            for (var i = 0; i < Traversal.Count - 1; i++)
            {
                Console.Write($"{Traversal[i]} -> ");
            }
            Console.WriteLine(Traversal[Traversal.Count - 1]);
        }
    }
}
