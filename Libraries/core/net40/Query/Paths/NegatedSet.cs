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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Query.Patterns;

namespace VDS.RDF.Query.Paths
{
    /// <summary>
    /// Represents a Negated Property Set
    /// </summary>
    public class NegatedSet : ISparqlPath
    {
        private List<Property> _properties = new List<Property>();
        private List<Property> _inverseProperties = new List<Property>();

        /// <summary>
        /// Creates a new Negated Property Set
        /// </summary>
        /// <param name="properties">Negated Properties</param>
        /// <param name="inverseProperties">Inverse Negated Properties</param>
        public NegatedSet(IEnumerable<Property> properties, IEnumerable<Property> inverseProperties)
        {
            this._properties.AddRange(properties);
            this._inverseProperties.AddRange(inverseProperties);
        }

        /// <summary>
        /// Gets the Negated Properties
        /// </summary>
        public IEnumerable<Property> Properties
        {
            get
            {
                return this._properties;
            }
        }

        /// <summary>
        /// Gets the Inverse Negated Properties
        /// </summary>
        public IEnumerable<Property> InverseProperties
        {
            get
            {
                return this._inverseProperties;
            }
        }

        /// <summary>
        /// Converts a Path into its Algebra Form
        /// </summary>
        /// <param name="context">Path Transformation Context</param>
        /// <returns></returns>
        public ISparqlAlgebra ToAlgebra(PathTransformContext context)
        {
            if (this._properties.Count > 0 && this._inverseProperties.Count == 0)
            {
                return new NegatedPropertySet(context.Subject, context.Object, this._properties);
            }
            else if (this._properties.Count == 0 && this._inverseProperties.Count > 0)
            {
                return new NegatedPropertySet(context.Object, context.Subject, this._inverseProperties, true);
            }
            else
            {
                PathTransformContext lhsContext = new PathTransformContext(context);
                PathTransformContext rhsContext = new PathTransformContext(context);
                lhsContext.AddTriplePattern(new PropertyPathPattern(lhsContext.Subject, new NegatedSet(this._properties, Enumerable.Empty<Property>()), lhsContext.Object));
                rhsContext.AddTriplePattern(new PropertyPathPattern(rhsContext.Subject, new NegatedSet(Enumerable.Empty<Property>(), this._inverseProperties), rhsContext.Object));
                ISparqlAlgebra lhs = lhsContext.ToAlgebra();
                ISparqlAlgebra rhs = rhsContext.ToAlgebra();
                return new Union(lhs, rhs);
            }
        }

        /// <summary>
        /// Gets the String representation of the Path
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append('!');
            if (this._properties.Count + this._inverseProperties.Count > 1) output.Append('(');

            for (int i = 0; i < this._properties.Count; i++)
            {
                output.Append(this._properties[i].ToString());
                if (i < this._properties.Count - 1 || this._inverseProperties.Count > 0)
                {
                    output.Append(" | ");
                }
            }
            for (int i = 0; i < this._inverseProperties.Count; i++)
            {
                output.Append(this._inverseProperties[i].ToString());
                if (i < this._inverseProperties.Count - 1)
                {
                    output.Append(" | ");
                }
            }

            if (this._properties.Count + this._inverseProperties.Count > 1) output.Append(')');

            return output.ToString();
        }
    }
}
