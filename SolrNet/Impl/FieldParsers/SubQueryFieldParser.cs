#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.FieldParsers
{
    /// <summary>
    /// Parser de subqueries
    /// Voir https://solr.apache.org/guide/7_6/transforming-result-documents.html#subquery
    /// </summary>
    public class SubQueryFieldParser : ISolrFieldParser
    {
        private readonly ISolrFieldParser valueParser;
        private readonly SolrDictionaryDocumentResponseParser docParser;

        /// <summary>
        /// Parses 1-dimensional fields
        /// </summary>
        public SubQueryFieldParser(ISolrFieldParser valueParser)
        {
            this.valueParser = valueParser;
            docParser = new SolrDictionaryDocumentResponseParser(valueParser);
        }

        /// <inheritdoc />
        public bool CanHandleSolrType(string solrType)
        {
            return solrType == "result";
        }

        /// <inheritdoc />
        public bool CanHandleType(Type t)
        {
            return t != typeof(string) &&
                   typeof(IEnumerable).IsAssignableFrom(t) &&
                   !typeof(IDictionary).IsAssignableFrom(t) &&
                   !TypeHelper.IsGenericAssignableFrom(typeof(IDictionary<,>), t);
        }

        /// <inheritdoc />
        public object Parse(XElement results, Type t)
        {
            // On renvoie la grappe xml du résultat de sous-requête
            return results;
        }
    }
}
