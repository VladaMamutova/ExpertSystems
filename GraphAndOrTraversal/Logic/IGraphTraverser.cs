using System.Collections.Generic;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal.Logic
{
    interface IGraphTraverser
    {
        GraphItem FindDescendant(GraphItem item);
        bool Markup(GraphItem item);
        IEnumerable<GraphItem> Traverse();
    }
}
