using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GraphAndOrTraversal.Model
{
    class Graph
    {
        public Dictionary<int, Vertex> Vertices { get; }
        public List<Edge> Edges { get; }

        public Graph()
        {
            Vertices = new Dictionary<int, Vertex>();
            Edges = new List<Edge>();
        }

        private Vertex FindOrCreateVertex(int label)
        {
            var vertex = FindVertex(label);
            if (vertex == null)
            {
                vertex = new Vertex(label);
                Vertices.Add(vertex.Label, vertex);
            }

            return vertex;
        }

        public Vertex FindVertex(int label)
        {
            Vertices.TryGetValue(label, out var vertex);
            return vertex;
        }

        public void AddEdge(int label, int[] source, int destination)
        {
            List<Vertex> start = new List<Vertex>();
            foreach (var sourceLabel in source)
            {
                start.Add(FindOrCreateVertex(sourceLabel));
            }
            var end = FindOrCreateVertex(destination);

            Edges.Add(new Edge(label, start.ToArray(), end));
        }

        public void Print()
        {
            foreach (var edge in Edges)
            {
                Console.WriteLine(edge);
            }
        }

        public static Graph FromFile(string fileName)
        {
            var graph = new Graph();
            
            var text = File.ReadAllText(fileName);
            string pattern = @"\((\d+); ((?:\d+, )*\d+); (\d+)\)";
            MatchCollection matches = Regex.Matches(text, pattern);

            foreach (Match match in matches)
            {
                var outLabel = int.Parse(match.Groups[1].Value);
                var inLabels = match.Groups[2].Value
                    .Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse);
                var edge = int.Parse(match.Groups[3].Value);
                graph.AddEdge(edge, inLabels.ToArray(), outLabel);
            }

            return graph;
        }
    }
}
