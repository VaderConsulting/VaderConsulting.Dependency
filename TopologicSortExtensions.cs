using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{

    /// <summary>
    /// Extension method to implement topologic sorting. Will return a list where items with dependencies
    /// will come after items without dependencies.
    /// </summary>
    /// <remarks>
    /// From http://stackoverflow.com/questions/21189222/topological-sort-with-support-for-cyclic-dependencies
    /// </remarks>
    public static class TopologicSortExtensions
    {
        /// <summary>
        /// Sorts a list of items with dependencies such that items without dependencies come first.
        /// </summary>
        /// <typeparam name="T">Type of list, which must implement <see cref="ITopologicSortable"/>.</typeparam>
        /// <param name="graph">List of items to sort.</param>
        /// <returns>A sorted list of item, where items with dependencies after items without dependencies.</returns>
        public static List<T> TopologicSort<T>(this List<T> graph)
            where T : ITopologicSortable<T>
        {
            int[] state = new int[graph.Count];
            List<T> list = new List<T>();

            for (int i = 0; i < graph.Count; i++)
            {
                state[i] = 1; /* alive */
            }
            for (int i = 0; i < graph.Count; i++)
            {
                visit(graph, i, list, state);
            }
            return list;
        }

        private static void visit<T>(List<T> graph, int index, List<T> list, int[] state)
            where T : ITopologicSortable<T>
        {
            if (state[index] == -1) /* dead */
            {
                return; // We've done this one already.
            }
            if (state[index] == 0) /* processing */
            {
                return; // We have a cycle; if you have special cycle handling code do it here.
            }

            // It's alive. Mark it as undead.
            state[index] = 0; /* processing */

            foreach (T neighbour in getNeighbours(graph, graph[index]))
            {
                visit(graph, graph.IndexOf(neighbour), list, state);
            }

            state[index] = -1; /* dead */

            list.Add(graph[index]);
        }

        private static List<T> getNeighbours<T>(List<T> graph, T node)
            where T : ITopologicSortable<T>
        {
            List<T> neighbours = new List<T>();
            graph.ForEach(x => x.DependsOn.Where(y => y.Identifier == node.Identifier).ToList().ForEach(z =>
            {
                neighbours.Add(z);
            }));

            node.DependsOn.ToList().ForEach(x => neighbours.Add(x));

            List<T> unique = new List<T>();
            neighbours.ForEach(x =>
            {
                if (!unique.Any(y => y.Identifier == x.Identifier))
                {
                    unique.Add(x);
                }
            });

            // can't be neighbors with yourself
            if (unique.IndexOf(node) > -1)
            {
                unique.Remove(node);
            }

            return unique;
        }
    }
}
