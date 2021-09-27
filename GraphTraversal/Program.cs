using System;
using GraphTraversal.Logic;
using GraphTraversal.Model;

namespace GraphTraversal
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Graph graph = Graph.FromFile("graph.txt");
                Console.WriteLine("    Graph   ");
                graph.Print();

                IGraphTraverser graphTraversal =
                    new DepthFirstSearch(graph, 0, 8)
                        {DebugMode = true};

                graphTraversal.Traverse();
                graphTraversal.PrintTraversal();

                graph = Graph.FromFile("graph.txt");
                graphTraversal = new BreadthFirstSearch(graph, 0, 8)
                    {DebugMode = true};

                graphTraversal.Traverse();
                graphTraversal.PrintTraversal();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
