using System.Collections.Generic;
using GraphTraversal.Model;

namespace GraphTraversal.Logic
{
    interface IGraphTraverser
    {
        GraphItem FindDescendant(GraphItem source);
        IEnumerable<GraphItem> Traverse();
        void PrintTraversal();
    }
}
