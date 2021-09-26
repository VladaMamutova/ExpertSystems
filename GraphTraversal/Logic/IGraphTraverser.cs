using System.Collections.Generic;
using GraphTraversal.Model;

namespace GraphTraversal.Logic
{
    interface IGraphTraverser
    {
        IEnumerable<GraphItem> Traverse();
    }
}
