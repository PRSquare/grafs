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

            VertElipse.Fill = Brushes.Yellow;
            VertElipse.StrokeThickness = 2;
            VertElipse.Stroke = Brushes.Black;

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

}
