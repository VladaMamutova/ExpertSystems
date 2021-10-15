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
            var graph = Graph.Undirected;
            foreach (var edge in graphEdges)
            {
                graph = graph.Add(NodeStatement.For(new Id(edge.Out.ToString()))
                    .Set(new Id("shape"), new Id("circle")));
                foreach (var start in edge.In)
                {
                    graph = graph.Add(NodeStatement.For(new Id(start.ToString()))
                        .Set(new Id("shape"), new Id("circle")));
                    graph = graph.Add(EdgeStatement.For(edge.Out.ToString(),
                        start.Label.ToString()).Set(new Id("label"), new Id(edge.ToString())));
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
