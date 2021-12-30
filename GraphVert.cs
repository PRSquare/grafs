using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace grafs
{
    public class GraphVert : ICloneable
    {
        public double x { get; set; }
        public double y { get; set; }

        public bool IsPosed = false;

        public List<GraphEdge> ConnectedEdges;

        public VisualGraphVert VertVisual = null;

        public string VertName;

        public GraphVert(string name)
        {
            VertName = name;
            ConnectedEdges = new List<GraphEdge>();
            VertVisual = new VisualGraphVert();
            VertVisual.VertName.Text = VertName;
        }

        public object Clone() 
        {
            GraphVert ret = new GraphVert(this.VertName);
            foreach (var edge in this.ConnectedEdges) 
            {
                GraphEdge conEdge = new GraphEdge(edge.Route, edge.ConnectedVert, edge.EdgeWeight, edge.EdgeName);
                GraphEdge conEdgeCopy = edge.ConnectedVert.ConnectedEdges.Find(x => { return x.EdgeName == edge.EdgeName && x.EdgeWeight == edge.EdgeWeight && !edge.IsDirected; });                
                conEdge.IsDirected = edge.IsDirected;
                ret.ConnectedEdges.Add(conEdge);
            }
            ret.VertVisual.SetPos(this.VertVisual.x, this.VertVisual.y);
            return ret;
        }

        public void SetVertName(string name) 
        {
            VertName = name;
            VertVisual?.SetName(name);
        }

        // Delete later
        public String GetTextInfo()
        {
            String buff = "";
            buff = VertName + ": ";
            foreach (var e in ConnectedEdges)
            {
                buff += (e.EdgeName + "(" + e.ConnectedVert.VertName + "), ");
            }
            buff += "\n";
            return buff;
        }

        public bool HasEqualValues( GraphVert vert ) 
        {
            return this.VertName == vert.VertName;
        }
    }
}
