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
using System.Linq;
using System.Text;
using NUnit.Framework;
using VDS.RDF.Configuration;
using VDS.RDF.Query.PropertyFunctions;

namespace VDS.RDF.Configuration
{
    [TestFixture]
    public class LoadObjectTests
    {
        [Test]
        public void ConfigurationLoadObjectPropertyFunctionFactory()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:SparqlPropertyFunctionFactory ;
  dnr:type """ + typeof(MockPropertyFunctionFactory).AssemblyQualifiedName + @""" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            IPropertyFunctionFactory factory = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as IPropertyFunctionFactory;
            Assert.IsNotNull(factory);
            Assert.AreEqual(typeof(MockPropertyFunctionFactory), factory.GetType());
        }

        [Test]
        public void ConfigurationLoadObjectTripleCollection1()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:TripleCollection ;
  dnr:type ""VDS.RDF.TreeIndexedTripleCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseTripleCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseTripleCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(TreeIndexedTripleCollection), collection.GetType());
        }

        [Test]
        public void ConfigurationLoadObjectTripleCollection2()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:TripleCollection ;
  dnr:type ""VDS.RDF.ThreadSafeTripleCollection"" ;
  dnr:usingTripleCollection _:b .
_:b a dnr:TripleCollection ;
  dnr:type ""VDS.RDF.TreeIndexedTripleCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseTripleCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseTripleCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(ThreadSafeTripleCollection), collection.GetType());
        }

        [Test]
        public void ConfigurationLoadObjectGraphCollection1()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.GraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseGraphCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseGraphCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(GraphCollection), collection.GetType());
        }

        [Test]
        public void ConfigurationLoadObjectGraphCollection2()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.ThreadSafeGraphCollection"" ;
  dnr:usingGraphCollection _:b .
_:b a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.GraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseGraphCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseGraphCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(ThreadSafeGraphCollection), collection.GetType());
        }

#if !NO_FILE // No DiskDemandGraphCollection
        [Test]
        public void ConfigurationLoadObjectGraphCollection3()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.DiskDemandGraphCollection"" ;
  dnr:usingGraphCollection _:b .
_:b a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.GraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseGraphCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseGraphCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(DiskDemandGraphCollection), collection.GetType());
        }
#endif

#if !SILVERLIGHT // No WebDemandGraphCollection
        [Test]
        public void ConfigurationLoadObjectGraphCollection4()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.WebDemandGraphCollection"" ;
  dnr:usingGraphCollection _:b .
_:b a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.GraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseGraphCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseGraphCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(WebDemandGraphCollection), collection.GetType());
        }
#endif

#if !SILVERLIGHT // No WebDemandGraphCollection
        [Test]
        public void ConfigurationLoadObjectGraphCollection5()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.WebDemandGraphCollection"" ;
  dnr:usingGraphCollection _:b .
_:b a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.ThreadSafeGraphCollection"" ;
  dnr:usingGraphCollection _:c .
_:c a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.GraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            BaseGraphCollection collection = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as BaseGraphCollection;
            Assert.IsNotNull(collection);
            Assert.AreEqual(typeof(WebDemandGraphCollection), collection.GetType());
        }
#endif

        [Test]
        public void ConfigurationLoadObjectGraphEmpty1()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:Graph ;
  dnr:type ""VDS.RDF.Graph"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            IGraph result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as IGraph;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(Graph), result.GetType());
        }

#if !NO_RWLOCK // No ThreadSafeGraph
        [Test]
        public void ConfigurationLoadObjectGraphEmpty2()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:Graph ;
  dnr:type ""VDS.RDF.ThreadSafeGraph"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            IGraph result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as IGraph;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(ThreadSafeGraph), result.GetType());
        }
#endif

        [Test]
        public void ConfigurationLoadObjectGraphEmpty3()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:Graph ;
  dnr:type ""VDS.RDF.Graph"" ;
  dnr:usingTripleCollection _:b .
_:b a dnr:TripleCollection ;
  dnr:type ""VDS.RDF.ThreadSafeTripleCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            IGraph result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as IGraph;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(Graph), result.GetType());
            Assert.AreEqual(typeof(ThreadSafeTripleCollection), result.Triples.GetType());
        }

        [Test]
        public void ConfigurationLoadObjectTripleStoreEmpty1()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:TripleStore ;
  dnr:type ""VDS.RDF.TripleStore"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            ITripleStore result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as ITripleStore;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(TripleStore), result.GetType());
        }

#if !SILVERLIGHT // No WebDemandTripleStore
        [Test]
        public void ConfigurationLoadObjectTripleStoreEmpty2()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:TripleStore ;
  dnr:type ""VDS.RDF.WebDemandTripleStore"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            ITripleStore result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as ITripleStore;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(WebDemandTripleStore), result.GetType());
        }
#endif

        [Test]
        public void ConfigurationLoadObjectTripleStoreEmpty3()
        {
            String graph = ConfigLookupTests.Prefixes + @"
_:a a dnr:TripleStore ;
  dnr:type ""VDS.RDF.TripleStore"" ;
  dnr:usingGraphCollection _:b .
_:b a dnr:GraphCollection ;
  dnr:type ""VDS.RDF.ThreadSafeGraphCollection"" .";

            Graph g = new Graph();
            g.LoadFromString(graph);

            ITripleStore result = ConfigurationLoader.LoadObject(g, g.GetBlankNode("a")) as ITripleStore;
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(TripleStore), result.GetType());
            Assert.AreEqual(typeof(ThreadSafeGraphCollection), result.Graphs.GetType());
        }
    }

    class MockPropertyFunctionFactory
        : IPropertyFunctionFactory
    {
        public bool IsPropertyFunction(Uri u)
        {
            throw new NotImplementedException();
        }

        public bool TryCreatePropertyFunction(PropertyFunctionInfo info, out RDF.Query.Patterns.IPropertyFunctionPattern function)
        {
            throw new NotImplementedException();
        }
    }
}
