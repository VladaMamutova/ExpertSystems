using System.Collections.Generic;
using DepthFirstSearch.Model;

namespace GraphTraversal.Logic
{
    interface IGraphTraverser
    {
        IList<Vertex> Traverse();
    }
}
