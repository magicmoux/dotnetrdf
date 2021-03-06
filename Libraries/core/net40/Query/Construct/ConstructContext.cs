/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using VDS.Common.Collections;
using VDS.Common.References;
using VDS.RDF.Query.Algebra;

namespace VDS.RDF.Query.Construct
{
    /// <summary>
    /// Context used for Constructing Triples in SPARQL Query/Update
    /// </summary>
    public class ConstructContext
    {
        private static ThreadIsolatedReference<NodeFactory> _globalFactory = new ThreadIsolatedReference<NodeFactory>(() => new NodeFactory());
        private ISet _s;
        private INodeFactory _factory;
        private IGraph _g;
        private bool _preserveBNodes = false;
        private Dictionary<String, INode> _bnodeMap;
        private MultiDictionary<INode, INode> _nodeMap;

        /// <summary>
        /// Creates a new Construct Context
        /// </summary>
        /// <param name="g">Graph to construct Triples in</param>
        /// <param name="s">Set to construct from</param>
        /// <param name="preserveBNodes">Whether Blank Nodes bound to variables should be preserved as-is</param>
        /// <remarks>
        /// <para>
        /// Either the <paramref name="s">Set</paramref>  or <paramref name="g">Graph</paramref> parameters may be null if required
        /// </para>
        /// </remarks>
        public ConstructContext(IGraph g, ISet s, bool preserveBNodes)
        {
            this._g = g;
            this._factory = (this._g != null ? (INodeFactory)this._g : _globalFactory.Value);
            this._s = s;
            this._preserveBNodes = preserveBNodes;
        }

        /// <summary>
        /// Creates a new Construct Context
        /// </summary>
        /// <param name="factory">Factory to create nodes with</param>
        /// <param name="s">Set to construct from</param>
        /// <param name="preserveBNodes">Whether Blank Nodes bound to variables should be preserved as-is</param>
        /// <remarks>
        /// <para>
        /// Either the <paramref name="s">Set</paramref>  or <paramref name="factory">Factory</paramref> parameters may be null if required
        /// </para>
        /// </remarks>
        public ConstructContext(INodeFactory factory, ISet s, bool preserveBNodes)
        {
            this._factory = (factory != null ? factory : _globalFactory.Value);
            this._s = s;
            this._preserveBNodes = preserveBNodes;
        }

        /// <summary>
        /// Gets the Set that this Context pertains to
        /// </summary>
        public ISet Set
        {
            get
            {
                return this._s;
            }
        }

        /// <summary>
        /// Gets the Graph that Triples should be constructed in
        /// </summary>
        public IGraph Graph
        {
            get
            {
                return this._g;
            }
        }

        private INodeFactory NodeFactory
        {
            get
            {
                return this._factory;
            }
        }

        /// <summary>
        /// Gets whether Blank Nodes bound to variables should be preserved
        /// </summary>
        public bool PreserveBlankNodes
        {
            get
            {
                return this._preserveBNodes;
            }
        }

        /// <summary>
        /// Creates a new Blank Node for this Context
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// If the same Blank Node ID is used multiple times in this Context you will always get the same Blank Node for that ID
        /// </para>
        /// </remarks>
        public INode GetBlankNode(String id)
        {
            if (this._bnodeMap == null) this._bnodeMap = new Dictionary<string, INode>();

            if (this._bnodeMap.ContainsKey(id)) return this._bnodeMap[id];

            INode temp;
            if (this._g != null)
            {
                temp = this._g.CreateBlankNode();
            }
            else if (this._factory != null)
            {
                temp = this._factory.CreateBlankNode();
            }
            else if (this._s != null)
            {
                temp = new BlankNode(this._g, id.Substring(2) + this._s.ID);
            }
            else
            {
                temp = new BlankNode(this._g, id.Substring(2));
            }
            this._bnodeMap.Add(id, temp);
            return temp;
        }

        /// <summary>
        /// Creates a Node for the Context
        /// </summary>
        /// <param name="n">Node</param>
        /// <returns></returns>
        /// <remarks>
        /// <para>
        /// In effect all this does is ensure that all Nodes end up in the same Graph which may occassionally not happen otherwise when Graph wrappers are involved
        /// </para>
        /// </remarks>
        public INode GetNode(INode n)
        {
            if (this._nodeMap == null) this._nodeMap = new MultiDictionary<INode,INode>(new FastVirtualNodeComparer());

            if (this._nodeMap.ContainsKey(n)) return this._nodeMap[n];

            INode temp;
            switch (n.NodeType)
            {
                case NodeType.Blank:
                    temp = this.GetBlankNode(((IBlankNode)n).InternalID);
                    break;

                case NodeType.Variable:
                    IVariableNode v = (IVariableNode)n;
                    temp = this._factory.CreateVariableNode(v.VariableName);
                    break;

                case NodeType.GraphLiteral:
                    IGraphLiteralNode g = (IGraphLiteralNode)n;
                    temp = this._factory.CreateGraphLiteralNode(g.SubGraph);
                    break;

                case NodeType.Uri:
                    IUriNode u = (IUriNode)n;
                    temp = this._factory.CreateUriNode(u.Uri);
                    break;

                case NodeType.Literal:
                    ILiteralNode l = (ILiteralNode)n;
                    if (l.DataType != null)
                    {
                        temp = this._factory.CreateLiteralNode(l.Value, l.DataType);
                    } 
                    else if (!l.Language.Equals(String.Empty))
                    {
                        temp = this._factory.CreateLiteralNode(l.Value, l.Language);
                    } 
                    else
                    {
                        temp = this._factory.CreateLiteralNode(l.Value);
                    }
                    break;
                
                default:
                    throw new RdfQueryException("Cannot construct unknown Node Types");
            }
            this._nodeMap.Add(n, temp);
            return temp;
        }
    }
}
