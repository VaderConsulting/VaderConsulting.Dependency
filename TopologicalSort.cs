using System;
using System.Collections.Generic;
using System.Diagnostics;

// (c) Gregory Adam 2009
//http://www.brpreiss.com/books/opus4/html/page557.html
/*   and
 * An Introduction to data structures with applications
 *   Jean-Paul Tremblay - Paul G. Sorensen
 *   pages 494-497
 *   
 * Topological sort of a forest of directed acyclic graphs
 * 
 * The algorithm is pretty straight
 * for each node, a list of succesors is built
 * each node contains its indegree  (predecessorCount)
 *
 * (1) create a queue containing the key of every node that has no predecessors
 * (2) while (the queue is not empty)
 *      dequeue()
 *      output the key
 *      remove the key
 *      
 *      for each node in the successorList
 *          decrement its predecessorCount
 *          if the predecessorCount becomes empty, add it to the queue
 * 
 * 
 * (3) if any node left, then there is a least one cycle
 *
 */
// <T> has to implement IEquatable
// <T> cannot be null - since IDictionary is used

/* Methods
 * 
 * public bool Edge(Node)
 *	returns true or false
 *	
 * public bool Edge(successor, predecessor)
 *	returns true or false
 *	
 *  public bool Sort(out Queue<T> outQueue)
 *  if true
 *		returns the evaluation queue
 *	else
 *		returns a queue with one cycle
 */
namespace VaderConsulting.Dependency
{
    public sealed class TopologicalSort<T> where T : IEquatable<T>
    {
        private Dictionary<T, NodeInfo> _Nodes = new Dictionary<T, NodeInfo>();
        

        /// <summary>
        /// Adds a node with nodeKey
        /// Does not complain if the node is already present
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns>
        ///		true on success
        ///		false if the nodeKey is null
        /// </returns>
        public bool AddNode(T nodeKey)
        {
            if (nodeKey == null)
            {
                return false;
            }

            if (!_Nodes.ContainsKey(nodeKey))
                _Nodes.Add(nodeKey, new NodeInfo());

            return true;
        }
        
        /// <summary>
        /// Add an Edge where successor depends on predecessor
        /// Does not complain if the directed arc is already in
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="DependsUponNode"></param>
        /// <returns>
        ///		true on success
        ///		false if either parameter is null
        ///			or successor equals predecessor
        /// </returns>
        public bool AddConnection(T ParentNode, T DependsUponNode)
        {
            //Debug.WriteLine("--- Adding topo connection from " + ParentNode.ToString() + " to " + DependsUponNode.ToString());
            
            // make sure both nodes are there
            if (!AddNode(ParentNode))
            {
                Debug.WriteLine("[ERR1009] Topological Sort could not add " + ParentNode.ToString(), "error");
                return false;
            }
            if (!AddNode(DependsUponNode))
            {
                Debug.WriteLine("[ERR1010] Topological Sort could not create dependency between " + ParentNode.ToString() + " and " + DependsUponNode.ToString(), "error");
                return false;
            }

            // if successor == predecessor (cycle) fail
            if (ParentNode.Equals(DependsUponNode))
            {
                Debug.WriteLine("[ERR1011] CIRCULAR DEPENDENCY FOUND WITH " + ParentNode.ToString(), "critical");
                return false;
            }

            var successorsOfPredecessor = _Nodes[DependsUponNode].Successors;

            // if the Parent is already there, keep silent
            if (!successorsOfPredecessor.Contains(ParentNode))
            {
                // add the sucessor to the predecessor's successors
                successorsOfPredecessor.Add(ParentNode);

                // increment predecessorCount of successor
                _Nodes[ParentNode].PredecessorCount++;
            }
            return true;

        }

        /// <summary>
        /// Sort Nodes.  Upon completion there are no Nodes remaining
        /// </summary>
        /// <param name="sortedQueue"></param>
        /// <returns></returns>
        public bool Sort(out Queue<T> sortedQueue)
        {
            sortedQueue = new Queue<T>(); // create, even if it stays empty

            var outputQueue = new Queue<T>(); // with predecessorCount == 0

            // (1) go through all the nodes
            //		if the node's predecessorCount == 0
            //			add it to the outputQueue
            foreach (KeyValuePair<T, NodeInfo> kvp in _Nodes)
                if (kvp.Value.PredecessorCount == 0)
                {
                    outputQueue.Enqueue(kvp.Key);
                }

            // (2) process the output Queue
            //	output the key
            //	delete the key from Nodes
            //	foreach successor
            //		decrement its predecessorCount
            //		if it becomes zero
            //			add it to the output Queue

            T nodeKey;
            NodeInfo nodeInfo;

            while (outputQueue.Count != 0)
            {
                nodeKey = outputQueue.Dequeue();

                sortedQueue.Enqueue(nodeKey); // add it to sortedQueue

                nodeInfo = _Nodes[nodeKey]; // get successors of nodeKey

                _Nodes.Remove(nodeKey);	// remove it from Nodes

                foreach (T successor in nodeInfo.Successors)
                    if (--_Nodes[successor].PredecessorCount == 0)
                    {
                        outputQueue.Enqueue(successor);
                    }

                nodeInfo.Clear();
            }

            // outputQueue is empty here
            if (_Nodes.Count == 0)
            {
                return true;	// if there are no nodes left in Nodes, return true
            }

            // there is at least one cycle
            CycleInfo(sortedQueue); // get one cycle in sortedQueue
            {
                return false; // and fail
            }

        }

        /// <summary>
        /// Clears the Nodes for reuse.  Note that Sort() already does this
        /// </summary>
        public void Clear()
        {
            foreach (NodeInfo nodeInfo in _Nodes.Values)
                nodeInfo.Clear();

            _Nodes.Clear();
        }

        /// <summary>
        /// puts one cycle in cycleQueue
        /// </summary>
        /// <param name="cycleQueue"></param>
        public void CycleInfo(Queue<T> cycleQueue)
        {
            Debug.WriteLine("...Cycling queue...", "error");
            
            cycleQueue.Clear(); // Clear the queue, it may have data in it

            // init  Cycle info of remaining nodes
            foreach (NodeInfo nodeInfo in _Nodes.Values)
                nodeInfo.ContainsCycleKey = nodeInfo.CycleWasOutput = false;

            // (1) put the predecessor in the CycleKey of the successor
            T cycleKey = default(T);
            bool cycleKeyFound = false;

            NodeInfo successorInfo;

            foreach (KeyValuePair<T, NodeInfo> kvp in _Nodes)
            {
                foreach (T successor in kvp.Value.Successors)
                {
                    successorInfo = _Nodes[successor];

                    if (!successorInfo.ContainsCycleKey)
                    {
                        successorInfo.CycleKey = kvp.Key;
                        successorInfo.ContainsCycleKey = true;

                        if (!cycleKeyFound)
                        {
                            cycleKey = kvp.Key;
                            cycleKeyFound = true;
                        }
                    }
                }
                kvp.Value.Clear();
            }

            if (!cycleKeyFound)
                throw new Exception("program error: !cycleKeyFound");

            // (2) put a cycle in cycleQueue
            NodeInfo cycleNodeInfo;
            while (!(cycleNodeInfo = _Nodes[cycleKey]).CycleWasOutput)
            {
                if (!cycleNodeInfo.ContainsCycleKey)
                    throw new Exception("program error: nodeInfo.ContainsCycleKey");

                cycleQueue.Enqueue(cycleKey);
                cycleNodeInfo.CycleWasOutput = true;
                cycleKey = cycleNodeInfo.CycleKey;
            }
        }

        internal class NodeInfo
        {
            // for construction
            public int PredecessorCount;
            public List<T> Successors = new List<T>();

            // for Cycles in case the sort fails
            public T CycleKey;
            public bool ContainsCycleKey;
            public bool CycleWasOutput;

            // Clear NodeInfo
            public void Clear()
            {
                Successors.Clear();
            }

        }

        internal Dictionary<T, NodeInfo> Nodes
        {
            get
            {
                return _Nodes;
            }
        }

    }
}
