﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Expressions.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.XPath.Cast
{
    /// <summary>
    /// Class representing an XPath Date Time Cast Function
    /// </summary>
    public class DateTimeCast
        : BaseCast
    {
        /// <summary>
        /// Creates a new XPath Date Time Cast Function Expression
        /// </summary>
        /// <param name="expr">Expression to be cast</param>
        public DateTimeCast(ISparqlExpression expr) 
            : base(expr) { }

        /// <summary>
        /// Casts the value of the inner Expression to a Date Time
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingID">Binding ID</param>
        /// <returns></returns>
        public override IValuedNode Evaluate(SparqlEvaluationContext context, int bindingID)
        {
            IValuedNode n = this._expr.Evaluate(context, bindingID);//.CoerceToDateTime();

            if (n == null)
            {
                throw new RdfQueryException("Cannot cast a Null to a xsd:dateTime");
            }

            //New method should be much faster
            //if (n is DateTimeNode) return n;
            //if (n is DateNode) return new DateTimeNode(n.Graph, n.AsDateTime());
            //return new DateTimeNode(null, n.AsDateTime());

            switch (n.NodeType)
            {
                case NodeType.Blank:
                case NodeType.GraphLiteral:
                case NodeType.Uri:
                    throw new RdfQueryException("Cannot cast a Blank/URI/Graph Literal Node to a xsd:dateTime");

                case NodeType.Literal:
                    if (n is DateTimeNode) return n;
                    if (n is DateNode) return new DateTimeNode(n.Graph, n.AsDateTime());
                    //See if the value can be cast
                    ILiteralNode lit = (ILiteralNode)n;
                    if (lit.DataType != null)
                    {
                        string dt = lit.DataType.ToString();
                        if (dt.Equals(XmlSpecsHelper.XmlSchemaDataTypeDateTime))
                        {
                            //Already a xsd:dateTime
                            DateTimeOffset d;
                            if (DateTimeOffset.TryParse(lit.Value, out d))
                            {
                                //Parsed OK
                                return new DateTimeNode(lit.Graph, d);
                            }
                            else
                            {
                                throw new RdfQueryException("Invalid lexical form for xsd:dateTime");
                            }
                            
                        }
                        else if (dt.Equals(XmlSpecsHelper.XmlSchemaDataTypeString))
                        {
                            DateTimeOffset d;
                            if (DateTimeOffset.TryParse(lit.Value, out d))
                            {
                                //Parsed OK
                                return new DateTimeNode(lit.Graph, d);
                            }
                            else
                            {
                                throw new RdfQueryException("Cannot cast the value '" + lit.Value + "' to a xsd:double");
                            }
                        }
                        else
                        {
                            throw new RdfQueryException("Cannot cast a Literal typed <" + dt + "> to a xsd:dateTime");
                        }
                    }
                    else
                    {
                        DateTimeOffset d;
                        if (DateTimeOffset.TryParse(lit.Value, out d))
                        {
                            //Parsed OK
                            return new DateTimeNode(lit.Graph, d);
                        }
                        else
                        {
                            throw new RdfQueryException("Cannot cast the value '" + lit.Value + "' to a xsd:dateTime");
                        }
                    }
                default:
                    throw new RdfQueryException("Cannot cast an Unknown Node to a xsd:string");
            }
        }

        /// <summary>
        /// Gets the String representation of the Expression
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + XmlSpecsHelper.XmlSchemaDataTypeDateTime + ">(" + this._expr.ToString() + ")";
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return XmlSpecsHelper.XmlSchemaDataTypeDateTime;
            }
        }

        /// <summary>
        /// Transforms the Expression using the given Transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        public override ISparqlExpression Transform(IExpressionTransformer transformer)
        {
            return new DateTimeCast(transformer.Transform(this._expr));
        }
    }
}
