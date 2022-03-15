using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
        /// Name
        /// </summary>

        public string FacetName { get; internal set; }


        /// <summary>
        /// Columns
        /// </summary>

        public int Columns { get; internal set; }

        /// <summary>
        /// GridLevel
        /// </summary>

        public int GridLevel { get; internal set; }

        /// <summary>
        /// MaxX
        /// </summary>

        public double MaxX { get; internal set; }

        /// <summary>
        /// MaxY
        /// </summary>

        public double MaxY { get; internal set; }

        /// <summary>
        /// MinX
        /// </summary>

        public double MinX { get; internal set; }

        /// <summary>
        /// MinY
        /// </summary>

        public double MinY { get; internal set; }

        /// <summary>
        /// Rows
        /// </summary>

        public int Rows { get; internal set; }

        /// <summary>
        /// Répartition des counts
        /// </summary>

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
            var heatmapFacetNode = node.Elements("lst")
    .Where(X.AttrEq("name", "facet_heatmaps"));

            if (heatmapFacetNode is object)
            {
                foreach (var fieldNode in heatmapFacetNode.Elements())
                {
                    var name = fieldNode.Attribute("name").Value;
                    d[name] = new HeatmapFacetingResult(fieldNode);
                }
            }

            return d;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node">heatmap node</param>
        internal HeatmapFacetingResult(XElement node)
        {
            var heatmapFacetNode = node.Elements("lst")
    .Where(X.AttrEq("name", "facet_heatmaps"));

            FacetName = node.Attribute("name").Value;

            GridLevel = ReadIntAttribute(node, "gridLevel");
            Columns = ReadIntAttribute(node, "columns");
            Rows = ReadIntAttribute(node, "rows");

            MinX = ReadDoubleAttribute(node, "minX");
            MaxX = ReadDoubleAttribute(node, "maxX");
            MinY = ReadDoubleAttribute(node, "minY");
            MaxY = ReadDoubleAttribute(node, "maxY");

            CountsArrays = new int?[Rows][];

            int rowIndex = 0;
            foreach (XElement row in node.Elements("arr").First(e => e.Attribute("name")?.Value == "counts_ints2D").Elements())
            {
                if (row.Name == "null")
                {
                    rowIndex++;
                    continue;
                }

                CountsArrays[rowIndex++] = row.Elements("int").Select(e => (int?)int.Parse(e.Value, CultureInfo.InvariantCulture)).ToArray();
            }
        }

        private static int ReadIntAttribute(XElement root, string attributeName) => int.Parse(root.Elements().FirstOrDefault(e => e.Attribute("name")?.Value == attributeName)?.Value, CultureInfo.InvariantCulture);
        private static double ReadDoubleAttribute(XElement root, string attributeName) => double.Parse(root.Elements().FirstOrDefault(e => e.Attribute("name")?.Value == attributeName)?.Value, CultureInfo.InvariantCulture);
    }
}
