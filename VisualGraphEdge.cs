using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace grafs
{
    public class VisualGraphEdge
    {
        public Path VisualEdge;
        public TextBlock EdgeName;
        public TextBlock EdgeWeight;
        public double x1, x2, y1, y2;

        private PathFigure pthf;
        private ArcSegment arc;
        private PathGeometry pg;
        private Point arcBegin;
        private Point arcEnd;

        public int EdgeNumber = 1;

        public VisualGraphEdge(Point begin, Point end)
        {
            arcBegin = begin;
            arcEnd = end;

            EdgeName = new TextBlock();
            EdgeWeight = new TextBlock();

            pthf = new PathFigure();
            pthf.StartPoint = arcBegin;

            arc = new ArcSegment();
            arc.Point = arcEnd;
            arc.Size = new Size(0, 0);

            pthf.Segments.Add(arc);


            pg = new PathGeometry();
            pg.Figures.Add(pthf);

            VisualEdge = new Path();
            VisualEdge.Stroke = Brushes.Black;
            VisualEdge.StrokeThickness = 1;
            VisualEdge.Data = pg;
        }

        public void SetArcSize(double sizex, double sizey)
        {
            arc.Size = new Size(sizex, sizey);
        }

        public void SetName(string name)
        {
            EdgeName.Text = name;
        }
        public void SetWeight(int weight)
        {
            EdgeWeight.Text = weight.ToString();
        }

        public void SetNewCords(Point begin, Point end)
        {
            begin.X += 5;
            begin.Y += 5;
            end.X += 5;
            end.Y += 5;
            arcBegin = begin;
            arcEnd = end;

            pthf.StartPoint = arcBegin;
            arc.Point = arcEnd;
            double length = Math.Sqrt((end.X - begin.X) * (end.X - begin.X) + (end.Y - begin.Y) * (end.Y - begin.Y));
            if (Math.Abs(begin.X - end.X) < 10 && Math.Abs(begin.Y - end.Y) < 10)
            {
                arc.IsLargeArc = true;
                arc.Point = new Point(arc.Point.X + 1, arc.Point.Y);
                arc.Size = new Size(10, 10);
            }
            else
            {
                arc.IsLargeArc = false;
                arc.Size = new Size(2.0 / ((double)EdgeNumber * 0.25) * length, 2.0 / ((double)EdgeNumber * 0.25) * length);
            }

            double coefX = (begin.X - end.X) / ((Math.Abs((begin.X - end.X)) * 10) != 0 ? (Math.Abs((begin.X - end.X)) * 10) : 1);
            double coefY = 0;

            EdgeName.Margin = new Thickness(begin.X + (end.X - begin.X) / 2 + coefX, begin.Y + (end.Y - begin.Y) / 2 + coefY, 0, 0);
            EdgeWeight.Margin = new Thickness(begin.X + (end.X - begin.X) / 2 + coefX, begin.Y + (end.Y - begin.Y) / 2 + coefX + 10, 0, 0);


        }
        public void SetNewCords(double x1, double y1, double x2, double y2)
        {
            SetNewCords(new Point(x1, y1), new Point(x2, y2));
        }

        public void AddOnCanvas(Canvas canv)
        {
            canv.Children.Add(VisualEdge);
            canv.Children.Add(EdgeName);
            canv.Children.Add(EdgeWeight);
        }
    }
}
