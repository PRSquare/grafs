using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace grafs
{
    class GraphSaver
    {
        public static void SaveGraphCodeFile(string path, List<GraphVert> graph) 
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            StreamWriter sw = new StreamWriter(path);
            foreach (var vert in graph) 
            {
                sw.WriteLine("Vertex{" + vert.VertName + "(" + vert.VertVisual.x + ", " + vert.VertVisual.y + ")}");
            }
            sw.WriteLine("Edges\n{");
            bool first = true;
            foreach (var vert in graph) 
            {
                foreach (var edge in vert.ConnectedEdges) 
                {
                    if (first)
                        first = false;
                    else
                        sw.Write(",\n");
                    sw.Write("\t" + edge.EdgeName + "(" + edge.EdgeWeight.ToString() + ", " + edge.Route.VertName + ", " + edge.ConnectedVert.VertName + ")");
                }
            }
            sw.WriteLine("\n}");
            sw.Flush();
            sw.Close();
        }
        public static void SaveAjacentyMatFile(string path, List<GraphVert> graph) 
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write("0");
            foreach (var vert in graph)
                sw.Write(" "+vert.VertName);
            sw.Write("\n");
            foreach (var vert in graph) 
            {
                sw.Write(vert.VertName);
                foreach (var sub in graph)
                {
                    GraphEdge ed = vert.ConnectedEdges.Find(x => { return (x.ConnectedVert == x.Route || sub != vert) && (x.ConnectedVert == sub || (x.Route == sub && !x.IsDirected)); });
                    if (ed != null)
                        sw.Write(" " + ed.EdgeWeight.ToString());
                    else
                        sw.Write(" 0");
                }
                sw.Write("\n");
            }
            sw.Flush();
            sw.Close();
        }
        public static void SaveIncMatFile(string path, List<GraphVert> graph, List<GraphEdge> edges) 
        {
            StreamWriter sw = new StreamWriter(path);
            foreach (var vert in graph) 
            {
                foreach( var edge in edges) 
                {
                    GraphEdge ed = vert.ConnectedEdges.Find(x => x == edge);
                    if (ed != null)
                        sw.Write(ed.EdgeWeight + " ");
                    else
                        sw.Write("0 ");
                }
                sw.Write("\n");
            }
            sw.Flush();
            sw.Close();
        }
    }
}
