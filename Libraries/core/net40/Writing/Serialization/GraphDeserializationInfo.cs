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

#if !(SILVERLIGHT||NETCORE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VDS.RDF.Writing.Serialization
{
    class GraphDeserializationInfo
    {
        private List<Triple> _triples;
        private List<KeyValuePair<String, String>> _namespaces;
        private Uri _baseUri;

        public GraphDeserializationInfo(SerializationInfo info, StreamingContext context)
        {
            this._triples = (List<Triple>)info.GetValue("triples", typeof(List<Triple>));
            this._namespaces = (List<KeyValuePair<String, String>>)info.GetValue("namespaces", typeof(List<KeyValuePair<String, String>>));
            String baseUri = info.GetString("base");
            if (!baseUri.Equals(String.Empty))
            {
                this._baseUri = UriFactory.Create(baseUri);
            }
        }

        public void Apply(IGraph g)
        {
            g.BaseUri = this._baseUri;
            g.Assert(this._triples);
            foreach (KeyValuePair<String, String> ns in this._namespaces)
            {
                g.NamespaceMap.AddNamespace(ns.Key, UriFactory.Create(ns.Value));
            }
        }
    }
}


#endif