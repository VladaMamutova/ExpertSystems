using System.Collections.Generic;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal.Logic
{
    /// <summary>
    /// Интерфейс обхода графа (поиска в графе).
    /// </summary>
    interface IGraphTraverser
    {
        GraphItem FindDescendant(GraphItem item); // метод потомков
        bool Markup(GraphItem item); // метод разметки
        IEnumerable<GraphItem> Traverse(); // метод поиска (обхода графа)
    }
}
