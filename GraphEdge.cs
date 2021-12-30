using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace grafs
{
    public class GraphEdge
    {
        private String edgeName;
        public String EdgeName { get { return edgeName; } set { edgeName = value; Visual?.SetName(value); } }
        public GraphVert[] Verts = new GraphVert[2];

        private int edgeWeight;
        public int EdgeWeight { get { return edgeWeight; } set { edgeWeight = value; Visual?.SetWeight(value); } }

        public GraphVert Route;
        public GraphVert ConnectedVert;

        public VisualGraphEdge Visual;

        public bool IsDirected = true;

        public GraphEdge(GraphVert route = null, GraphVert connection = null, int weight = 1, string name = "Edge") 
        {
            Route = route;
            if (connection != null)
                AddConnection(connection);

            Visual = new VisualGraphEdge(new Point(0, 0), new Point(0, 0));
            
            EdgeWeight = weight;
            EdgeName = name;

        }

        ~GraphEdge()
        {
            Route?.ConnectedEdges?.Remove(this);
        }

        public void RemoveFromRoute() 
        {
            Route.ConnectedEdges.Remove(this);
        }

        public void RemoveFromConnected()
        {
            ConnectedVert.ConnectedEdges.Remove(this);
        }

        public void UpdateCords() 
        {
            Visual.SetNewCords(Route.VertVisual.x, Route.VertVisual.y, ConnectedVert.VertVisual.x, ConnectedVert.VertVisual.y);
            if (IsDirected == false && Route.VertName != ConnectedVert.VertName)
                Visual.SetArcSize(0, 0);
        }

        public void AddRoute(GraphVert vert) 
        {
            Route = vert;
        }
        public void AddConnection(GraphVert vert)
        {
            ConnectedVert = vert;
        }

        public void AddVert(GraphVert vert)
        {
            if (Route == null)
            {
                Route = vert;
            }
            else if (ConnectedVert == null)
            {
                ConnectedVert = vert;
            }
            else
            {
                Console.WriteLine("All vers were set!");
                // Exception
                return;
            }
        }

        public void AddAdjacenty()
        {
            if (Route != null && ConnectedVert != null)
            {
                if(!Route.ConnectedEdges.Exists( x => x == this ))
                    Route.ConnectedEdges.Add(this);

                if (IsDirected == true)
                {
                    GraphEdge CopyEdge = ConnectedVert.ConnectedEdges.Find(x => x.ConnectedVert == this.Route &&
                    x.edgeName == this.edgeName &&
                    x.edgeWeight == this.EdgeWeight);
                    if (CopyEdge != null)
                    {
                        IsDirected = false;
                        ConnectedVert.ConnectedEdges.Remove(CopyEdge);
                    }
                }
                if (IsDirected == false)  // NOT ELSE!!!
                {
                    ConnectedVert.ConnectedEdges.Add( this );
                }
            }
            else
            {
                // Exception
                Console.WriteLine("Verts unset!");
            }
        }

        public void AddAdjacentyToRootVert()
        {
            //
            // Outdated!
            //
            if (Route != null && ConnectedVert != null)
            {
                Route.ConnectedEdges.Add(this);
            }
            else
            {
                // Exception
                Console.WriteLine("Verts unset!");
            }
        }
        public void AddAdjacentyToConnectedVert()
        {
            //
            // Outdated!
            //
            if (Route != null && ConnectedVert != null)
            {
                ConnectedVert.ConnectedEdges.Add(new GraphEdge(ConnectedVert, Route, EdgeWeight, EdgeName));
            }
            else
            {
                // Exception
                Console.WriteLine("Verts unset!");
            }
        }
        public bool HasEqualValues(GraphEdge edge)
        {
            return this.edgeName == edge.edgeName && this.edgeWeight == edge.edgeWeight && this.Route.HasEqualValues(edge.Route) && this.ConnectedVert.HasEqualValues(edge.ConnectedVert) && this.IsDirected == edge.IsDirected;
        }
    }
}
