using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphAndOrTraversal.Model;
using Shields.GraphViz.Components;
using Shields.GraphViz.Models;
using Shields.GraphViz.Services;
using Graph = Shields.GraphViz.Models.Graph;

namespace GraphAndOrTraversal.Helpers
{
    class GraphViz
    {
        private const string GraphVizBin =
            @"C:\Program Files (x86)\GraphViz\bin";

        private readonly Lazy<IRenderer> _renderer;

        private IRenderer Renderer => _renderer.Value;

        private static GraphViz _graphViz;
        private static readonly object SyncRoot = new object();

        private GraphViz()
        {
            _renderer = new Lazy<IRenderer>(() => new Renderer(GraphVizBin));
        }

        private static GraphViz GetGraphViz()
        {
            if (_graphViz == null)
            {
                lock (SyncRoot)
                {
                    _graphViz = new GraphViz();
                }
            }

            return _graphViz;
        }

        private Graph BuildGraph(List<Edge> graphEdges)
        {
            var graph = Graph.Directed;
            foreach (var edge in graphEdges)
            {
                foreach (var start in edge.Start)
                {
                    graph = graph.Add(EdgeStatement.For(edge.End.Label.ToString(),
                        start.Label.ToString()));
                }
            }

            return graph;
        }

        public static async Task GenerateImage(List<Edge> graphEdges, Stream stream)
        {
            var graphViz = GetGraphViz();

            Graph graph = graphViz.BuildGraph(graphEdges);
            await graphViz.Renderer.RunAsync(graph, stream, RendererLayouts.Dot,
                RendererFormats.Png, CancellationToken.None);
        }
    }
}
