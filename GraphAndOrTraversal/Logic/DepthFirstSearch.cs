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
        }

        public DepthFirstSearch(Graph graph, int[] source, int target)
        {
            Graph = graph;
            SetSource(source);
            SetTarget(target);
            Traversal = null;
            DebugMode = false;
        }
        
        public void SetSource(int[] labels)
        {
            Source = labels.Select(label => Graph.FindVertex(label)).ToArray();
        }

        public void SetTarget(int label)
        {
            var vertex = Graph.FindVertex(label);
            if (Source.Contains(vertex))
            {
                throw new Exception(
                    "Incorrect target! The target vertex is equal to one of the source vertices");
            }

            Target = vertex;
        }

        public GraphItem FindDescendant(GraphItem source)
        {
            foreach (var edge in Graph.Edges)
            {
                //
            }

            return null;
        }

        public void Markup()
        {
            throw new NotImplementedException();
        }
        private void PrintDebugLog(string message)
        {
            if (DebugMode)
            {
               // 
            }
        }

        public IEnumerable<GraphItem> Traverse()
        {
            if (Source == null || Target == null)
            {
                throw new Exception("The source or target vertex is not defined");
            }

            
            return Traversal;
        }
    }
}
