using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (!TryFindVertex(label, out var vertex))
            {
                vertex = new Vertex(label);
                Vertices.Add(vertex.Label, vertex);
            }

            return vertex;
        }

        public bool TryFindVertex(int label, out Vertex vertex)
        {
            return Vertices.TryGetValue(label, out vertex);
        }

        public Vertex FindVertex(int label)
        {
            if (!TryFindVertex(label, out var vertex))
            {
                throw new Exception(
                    $"Vertex with label '{label}' not found in the graph");
            }

            return vertex;
        }

        public void AddRule(int label, int[] inLabels, int outLabel)
        {
            List<Vertex> inVertices = new List<Vertex>();
            foreach (var inLabel in inLabels)
            {
                inVertices.Add(FindOrCreateVertex(inLabel));
            }
            var outVertex = FindOrCreateVertex(outLabel);

            Edges.Add(new Edge(label, inVertices.ToArray(), outVertex));
        }

        public string Print()
        {
            var builder = new StringBuilder();
            Edges.ForEach(edge => builder.Append(edge.Print()));
            return builder.ToString();
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
                var label = int.Parse(match.Groups[3].Value);
                graph.AddRule(label, inLabels.ToArray(), outLabel);
            }

            return graph;
        }
    }
}
