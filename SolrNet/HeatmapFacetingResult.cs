using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using SolrNet.Utils;

namespace SolrNet
{
    /// <summary>
    /// Heatmap Facet Result
    /// </summary>
    public class HeatmapFacetingResult
    {
        #region Public Properties

        /// <summary>
        /// Columns
        /// </summary>
        [JsonProperty(PropertyName = "columns")]
        public int Columns { get; internal set; }

        /// <summary>
        /// GridLevel
        /// </summary>
        [JsonProperty(PropertyName = "gridLevel")]
        public int GridLevel { get; internal set; }

        /// <summary>
        /// MaxX
        /// </summary>
        [JsonProperty(PropertyName = "maxX")]
        public double MaxX { get; internal set; }

        /// <summary>
        /// MaxY
        /// </summary>
        [JsonProperty(PropertyName = "maxY")]
        public double MaxY { get; internal set; }

        /// <summary>
        /// MinX
        /// </summary>
        [JsonProperty(PropertyName = "minX")]
        public double MinX { get; internal set; }

        /// <summary>
        /// MinY
        /// </summary>
        [JsonProperty(PropertyName = "minY")]
        public double MinY { get; internal set; }

        /// <summary>
        /// Rows
        /// </summary>
        [JsonProperty(PropertyName = "rows")]
        public int Rows { get; internal set; }

        /// <summary>
        /// Répartition des counts
        /// </summary>
        [JsonProperty(PropertyName = "counts_ints2D", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int?[][] CountsArrays { get; internal set; }

        #endregion Public Properties

        /// <summary>
        /// Parse the facet_heatmaps node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static IDictionary<string, HeatmapFacetingResult> ParseFacetHeatmaps(XElement node)
        {
            var d  =new Dictionary<string, HeatmapFacetingResult>();
            var heatmapFacetNode = node?.Elements("lst")
    .Where(X.AttrEq("name", "facet_heatmaps"));

            if (heatmapFacetNode is object)
            {
                foreach (var fieldNode in heatmapFacetNode.Elements())
                {
                    var name = fieldNode.Attribute("name").Value;
                    d[name] = HeatmapFacetingResult.ParseHeatmapFacetingNode(fieldNode);
                }
            }

            return d;
        }

        /// <summary>
        /// Parsing d'un noeud Heatmapface
        /// </summary>
        /// <param name="node">XElement</param>
        /// <returns></returns>
        private static HeatmapFacetingResult ParseHeatmapFacetingNode(XElement node)
        {
            int rows = ReadIntAttribute(node, "rows");
            var hmFacet = new HeatmapFacetingResult()
            {
                GridLevel = ReadIntAttribute(node, "gridLevel"),
                Columns = ReadIntAttribute(node, "columns"),
                Rows = rows,

                MinX = ReadDoubleAttribute(node, "minX"),
                MaxX = ReadDoubleAttribute(node, "maxX"),
                MinY = ReadDoubleAttribute(node, "minY"),
                MaxY = ReadDoubleAttribute(node, "maxY"),
            };

            int rowIndex = 0;

            var arrayNode = node.Elements("arr").FirstOrDefault(X.AttrEq("name", "counts_ints2D"));
            if (arrayNode is null)
                return hmFacet;

            // Init array
            hmFacet.CountsArrays = new int?[rows][];
            foreach (XElement row in arrayNode.Elements())
            {
                if (row.Name == "null")
                {
                    rowIndex++;
                    continue;
                }

                hmFacet.CountsArrays[rowIndex++] = row.Elements("int").Select(e => (int?)int.Parse(e.Value, CultureInfo.InvariantCulture)).ToArray();
            }

            return hmFacet;
        }

        private static int ReadIntAttribute(XElement root, string attributeName) => int.Parse(root.Elements().FirstOrDefault(X.AttrEq("name", attributeName))?.Value, CultureInfo.InvariantCulture);
        private static double ReadDoubleAttribute(XElement root, string attributeName) => double.Parse(root.Elements().FirstOrDefault(X.AttrEq("name", attributeName))?.Value, CultureInfo.InvariantCulture);
    }
}
