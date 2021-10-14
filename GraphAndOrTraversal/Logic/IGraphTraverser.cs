using System.Collections.Generic;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal.Logic
{
    interface IGraphTraverser
    {
        GraphItem FindDescendant(GraphItem source);
        void Markup();
        IEnumerable<GraphItem> Traverse();
    }
}
