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

                var graphTraversal = new DepthFirstSearch(graph, 0, 3);
                graphTraversal.DebugMode = true;

                graphTraversal.Traverse();
                Console.WriteLine("\nDepth First Search");
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
