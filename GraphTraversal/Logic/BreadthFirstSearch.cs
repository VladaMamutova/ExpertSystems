using System;
using System.Collections.Generic;
using System.Linq;
using GraphTraversal.Model;

namespace GraphTraversal.Logic
{
    /// <summary>
    /// Breadth-First Search
    /// </summary>
    class BreadthFirstSearch: IGraphTraverser
    {
        public Graph Graph { get; }
        public Vertex Source { get; private set; }
        public Vertex Target { get; private set; }

        public List<Vertex> Traversal { get; private set; }

        public bool DebugMode { get; set; }

        public BreadthFirstSearch(Graph graph)
        {
            Graph = graph;
            Traversal = null;
            DebugMode = false;
        }

        public BreadthFirstSearch(Graph graph, int source, int target)
        {
            Graph = graph;
            SetSource(source);
            SetTarget(target);
            Traversal = null;
            DebugMode = false;
        }

        private void PrintDebugLog(string message)
        {
            if (DebugMode)
            {
                Console.Write(message);
            }
        }

        public GraphItem FindDescendant(GraphItem source)
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

            PrintDebugLog("\nBreadth First Search Algorithm:\n");

            bool solved = false;
            var opened = new Queue<Vertex>(Graph.Vertices.Count);
            opened.Enqueue(Source);
            var routes = new List<Stack<Vertex>>(Graph.Vertices.Count);
            routes.Add(new Stack<Vertex>(new List<Vertex> {Source}));
            
            do
            {
                var head = opened.Peek();
                if (head.State != Vertex.States.FORBIDDEN)
                {
                    head.SetState(Vertex.States.FORBIDDEN);
                    PrintDebugLog($"| {head} | ");
                }

                var descendant = (Vertex)FindDescendant(head);
                if (descendant == null)
                {
                    opened.Dequeue();
                    routes.RemoveAll(stack => stack.Peek().Equals(head));
                }
                else
                {
                    descendant.SetState(Vertex.States.CLOSED);
                    opened.Enqueue(descendant);
                    var route = routes.Find(stack => stack.Peek().Equals(head));
                    routes.Add(new Stack<Vertex>(route.Reverse()));
                    route.Push(descendant);
                    solved = descendant.Equals(Target);
                    if (solved)
                    {
                        Traversal = route.Reverse().ToList();
                        routes.RemoveAll(stack => stack.Peek().Equals(head));
                    }
                    PrintDebugLog($"-> {descendant} ");
                }
            } while (opened.Count > 1 && !solved);

            PrintDebugLog(solved ? "-> solved\n": "-> no solution\n");
            
            return Traversal;
        }
        
        public void PrintTraversal()
        {
            if (Traversal == null)
            {
                return;
            }

            Console.WriteLine("\nBreadth First Search");
            for (var i = 0; i < Traversal.Count - 1; i++)
            {
                Console.Write($"{Traversal[i]} -> ");
            }
            Console.WriteLine(Traversal[Traversal.Count - 1]);
        }
    }
}
