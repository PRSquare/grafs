using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace grafs
{
    public class VisualGraphVert
    {
        public double x { get; set; }
        public double y { get; set; }
        public bool isPosed = false;
        public Ellipse VertElipse;
        public TextBlock VertName;
        public VisualGraphVert(double x = 0, double y = 0, bool isposed = false)
        {
            this.x = x;
            this.y = y;

            VertElipse = new Ellipse();
            VertName = new TextBlock();

            // Create a SolidColorBrush with a red color to fill the
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            VertElipse.Fill = mySolidColorBrush;
            VertElipse.StrokeThickness = 2;
            VertElipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            VertElipse.Width = 10;
            VertElipse.Height = 10;
            SetPos(x, y);
            isPosed = isposed;
        }
        public void SetName(string name) 
        {
            VertName.Text = name;
        }
        public void AddOnCanv(Canvas canvas)
        {
            canvas.Children.Add(VertElipse);
            canvas.Children.Add(VertName);
        }
        public void SetPos(double x, double y)
        {
            isPosed = true;
            this.x = x;
            this.y = y;
            VertElipse.Margin = new Thickness(x, y, 0, 0);
            VertName.Margin = new Thickness(x + 10, y + 10, 0, 0);
        }
    }

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
