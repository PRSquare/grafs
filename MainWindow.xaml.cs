using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;

namespace grafs
{
    public partial class MainWindow : Window
    {
        //GraphsVisual gv;

        LinkedList<undoBuffer> UndoBuffers = new LinkedList<undoBuffer>();

        List<GraphVert> gVerts = new List<GraphVert>();
        List<GraphEdge> gEdges = new List<GraphEdge>();

        AppModelView mw;

        AjTable ajtab;

        public MainWindow()
        {
            InitializeComponent();

            ajtab = new AjTable();

            DataContext = ajtab;
            //mw = new AppModelView();
            //DataContext = mw;
            //this.DataContext = gv;
        }
    
        private String readFileFromOpenFileDialog() 
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Text files (*.txt)|*.txt";
            String fileName = "";
            if (fd.ShowDialog() == true)
                fileName = fd.FileName;

            String buff = FileIntput.ReadFile(fileName);
            return buff;
        }

        //LinkedList<List<GraphVert>> UndoBuffer = new LinkedList<List<GraphVert>>();

        struct undoBuffer 
        {
            public List<GraphVert> gVerts;
            public List<GraphEdge> gEdges;
        }

        //enum ToolStates { VertexCreation, EdgeCreation, VertexSelection }
        //ToolStates CurToolState = 
        delegate void ToolFunc(MouseButtonEventArgs e);
        ToolFunc CurToolFunc = null;


        public void PutInBuffer() 
        {
            List<GraphVert> addList = new List<GraphVert>();
            foreach (var vert in gVerts)
                addList.Add((GraphVert)vert.Clone());


            foreach ( var vert in addList) 
            {
                foreach( var edge in vert.ConnectedEdges) 
                {
                    GraphVert connected = addList.Find(x => x.VertName == edge.ConnectedVert.VertName);
                    GraphVert route = addList.Find(x => x.VertName == edge.Route.VertName);
                    if (connected != null)
                        edge.ConnectedVert = connected;
                    if (route != null)
                        edge.Route = route;
                }
            }
            

            List<GraphEdge> addEdges = genEdgesFromGraphVertList(addList);

            string buff1 = "";

            foreach (var a in addEdges)
                buff1 += a.EdgeName + " ";
           

            undoBuffer buff;
            buff.gVerts = addList;
            buff.gEdges = addEdges;

            UndoBuffers.AddLast(buff);

            if (UndoBuffers.Count() > 10)
                UndoBuffers.RemoveFirst();
        }

        private undoBuffer UndoGraph() 
        {
            if (UndoBuffers.Count() > 0)
            {
                var ret = UndoBuffers.Last();
                UndoBuffers.RemoveLast();
                return ret;
            }
            else 
            {
                /// Exception
                throw new Exception("(((");
            }
        }

        public void undo_button_clicked(Object sender, RoutedEventArgs e) 
        {
            undoBuffer newVerts;
            try
            {
                newVerts = UndoGraph();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("End of undo stack");
                return;
            }
            gVerts = newVerts.gVerts;
            gEdges = newVerts.gEdges;

            inicEdges();

            UpdateGraphic();
            
        }

        public void create_vertex_button_clicked(Object sender, RoutedEventArgs e) 
        {
            CurToolFunc = vertexCreatrion;
        }

        public void create_edge_button_clicked(Object sender, RoutedEventArgs e)
        {
            CurToolFunc = edgesCreation;
        }

        public void select_vertex_or_edge_button_clicked(Object sender, RoutedEventArgs e)
        {
            CurToolFunc = vertexSelection;
        }
        

        private void inicEdges() 
        {
            gEdges = genEdgesFromGraphVertList(gVerts);
            
            foreach (var edge in gEdges) 
            {
                GraphEdge multipEdge = gEdges.Find(x => { return x != edge && x.Route == edge.Route && x.ConnectedVert == edge.ConnectedVert && x.Visual.EdgeNumber == edge.Visual.EdgeNumber; });
                while( multipEdge != null) 
                {
                    multipEdge.Visual.EdgeNumber++;
                    multipEdge = gEdges.Find(x => { return x != edge && x.Route == edge.Route && x.ConnectedVert == edge.ConnectedVert && x.Visual.EdgeNumber == edge.Visual.EdgeNumber; });
                }
            }
        }

        private List<GraphEdge> genEdgesFromGraphVertList(List<GraphVert> glist) 
        {
            List<GraphEdge> retEdges = new List<GraphEdge>();
            foreach (var gv in glist)
            {
                foreach (var ge in gv.ConnectedEdges)
                    if (retEdges.Find(x => { return x.HasEqualValues(ge); }) == null)
                        retEdges.Add(ge);
            }
            return retEdges;
        }

        private void open_aj_mat_file(Object sender, RoutedEventArgs e)
        {
            String buff = readFileFromOpenFileDialog();
            if (buff != null) 
            {
                try {
                    GraphVert[] verts = GraphImport.CreateGraphFromAdjacentyMatrix(buff);
                    Update(verts);
                } catch (Exception ex) {
                    MessageBox.Show("Wrong format");
                }
            }
        }

        private void open_inc_mat_file(Object sender, RoutedEventArgs e) 
        {
            String buff = readFileFromOpenFileDialog();
            if (buff != null)
            {
                try
                {
                    GraphVert[] verts = GraphImport.CreateGraphFromIncidenceMatrix(buff);
                    Update(verts);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wrong format");
                }
            }
}

        private void open_graph_code_file(Object sender, RoutedEventArgs e)
        {
            String buff = readFileFromOpenFileDialog();
            if (buff != null)
            {
                try
                {
                    GraphVert[] verts = GraphImport.CreateGraphGromEdgVertList(buff);
                    Update(verts);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wrong format");
                }
            }
        } 

        private void button_click(Object sender, RoutedEventArgs e) 
        {
            string buff = "";
            foreach (var gv in gEdges) 
            {
                buff += gv.EdgeName + "[";
                buff += gv.EdgeWeight.ToString() + "] ";
                if (gv.IsDirected)
                    buff += "(Directed): ";
                else
                    buff += "(Not directed): ";
                buff += gv.Route.VertName + " => ";
                buff += gv.ConnectedVert.VertName;
                buff += "\n";
            }
            MessageBox.Show(buff);
        }

        public void obj_name_changed(object sender, TextChangedEventArgs e) 
        {
            if(targetVert != null) 
            {
                string name = ((TextBox)sender).Text;
                if (!gVerts.Exists(x => { return x.VertName == name && x != targetVert; }))
                {
                    targetVert.SetVertName(name);
                    ((TextBox)sender).Background = Brushes.White;
                }
                else
                    ((TextBox)sender).Background = Brushes.Red;
            }
            if (targetEdge != null)
            {
                string tempName = targetEdge.EdgeName;
                targetEdge.EdgeName = ((TextBox)sender).Text;
                if (targetEdge.Route.ConnectedEdges.Exists(x => { return x.HasEqualValues(targetEdge) && x != targetEdge; }))
                {
                    targetEdge.EdgeName = tempName;
                    ((TextBox)sender).Background = Brushes.Red; 
                }
                else
                    ((TextBox)sender).Background = Brushes.White;

            }
            UpdateGraphic();
        }

        public void edge_is_directed_checked(object sender, RoutedEventArgs e)
        {
            if (targetEdge != null)
            {
                targetEdge.IsDirected = true;
                targetEdge.RemoveFromConnected();
            }
            UpdateGraphic();
        }
        public void edge_is_directed_unchecked(object sender, RoutedEventArgs e)
        {
            if (targetEdge != null)
            {
                targetEdge.IsDirected = false;
                targetEdge.AddAdjacenty();
            }    
            UpdateGraphic();
        }

        public void weight_changed(object sender, RoutedEventArgs e) 
        {
            if (targetEdge != null) 
            {
                try {
                    int tempWeight = targetEdge.EdgeWeight;
                    targetEdge.EdgeWeight = int.Parse(((TextBox)sender).Text);
                    if (targetEdge.Route.ConnectedEdges.Exists(x => { return x.HasEqualValues(targetEdge) && x != targetEdge; }))
                    {
                        targetEdge.EdgeWeight = tempWeight;
                        throw new Exception("Route already contains same edge!");
                    }
                    ((TextBox)sender).Background = Brushes.White;
                } catch (Exception ex) {
                    ((TextBox)sender).Background = Brushes.Red;
                }

            }
        }

        private UIElement targetElement = null;
        private Point targetPoint;
        private GraphVert targetVert = null;
        private GraphEdge targetEdge = null;

        private void ResetSelection() 
        {
            foreach (var gv in gVerts)
            {
                gv.VertVisual.VertElipse.Fill = new SolidColorBrush(Colors.Yellow);
                foreach (var ed in gv.ConnectedEdges)
                    ed.Visual.VisualEdge.Stroke = Brushes.Black;
            }
        }

        private void vertexSelection(MouseButtonEventArgs e) 
        {
            if(e.RightButton == MouseButtonState.Pressed) 
            {
                targetEdge = null;
                targetElement = null;
                targetVert = null;
                ResetSelection();
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (targetElement == null)
                {
                    targetElement = Mouse.DirectlyOver as UIElement;
                    if (targetElement != null)
                    {
                        // targetPoint = e.GetPosition(targetElement);
                        foreach (var gv in gVerts)
                        {
                            if (gv.VertVisual.VertElipse == targetElement)
                            {
                                targetVert = gv;
                                ResetSelection();
                                targetVert.VertVisual.VertElipse.Fill = new SolidColorBrush(Colors.Red);
                                ObjectName.Text = gv.VertName;
                                string buffer = "Connected edges:\n";
                                foreach (var ge in gv.ConnectedEdges)
                                    buffer += ge.EdgeName + "(" + ge.EdgeWeight.ToString() + ")\n";
                                ObjectDescription.Text = buffer;
                                return;
                            }
                        }
                        foreach (var ge in gEdges)
                        {
                            if (ge.Visual.VisualEdge == targetElement)
                            {
                                targetEdge = ge;
                                ResetSelection();
                                targetEdge.Visual.VisualEdge.Stroke = Brushes.Red;
                                ObjectName.Text = ge.EdgeName;
                                ObjectDescription.Text = "Weight: " + ge.EdgeWeight.ToString() + "\n" +
                                    "IsDirected: " + ge.IsDirected.ToString() + "\nRoute: " + ge.Route.VertName +
                                    "\nChild: " + ge.ConnectedVert.VertName + "\n";

                                CheckBox isDirected = new CheckBox();
                                isDirected.Margin = new Thickness(0, 0, 0, 10);
                                isDirected.Content = "Is directed";
                                isDirected.IsChecked = ge.IsDirected;
                                isDirected.Checked += new RoutedEventHandler(edge_is_directed_checked);
                                isDirected.Unchecked += new RoutedEventHandler(edge_is_directed_unchecked);

                                ObjectSettings.Children.Add(isDirected);

                                TextBlock weightText = new TextBlock();
                                weightText.Text = "Weight: ";
                                weightText.Margin = new Thickness(0, 11, 10, 10);

                                ObjectSettings.Children.Add(weightText);

                                TextBox vertWeight = new TextBox();
                                vertWeight.Margin = new Thickness(62, 10, 10, 386);
                                vertWeight.Text = ge.EdgeWeight.ToString();
                                vertWeight.TextChanged += weight_changed;

                                ObjectSettings.Children.Add(vertWeight);

                                return;
                            }
                        }
                        targetElement = null;
                    }
                }
                else
                {
                    if (targetVert != null)
                    {
                        PutInBuffer();
                        var pos = e.GetPosition(GraphCanvas);
                        targetVert.VertVisual.SetPos(pos.X, pos.Y);
                        UpdateGraphic();
                    }
                    targetVert = null;
                    targetEdge = null;
                    ObjectName.Text = "";
                    ObjectDescription.Text = "";
                    targetElement = null;
                    ObjectSettings.Children.Clear();
                    ResetSelection();
                    return;

                }
            }
        }

        private void vertexCreatrion(MouseButtonEventArgs e) 
        {
            PutInBuffer();

            string vertName = AutoNamer.GetName();

            while (gVerts.Exists(x => x.VertName == vertName))
                vertName = AutoNamer.GetName();
            
            GraphVert addedVert = new GraphVert(vertName);
            var pos = e.GetPosition(GraphCanvas);
            addedVert.VertVisual.SetPos(pos.X, pos.Y);
            gVerts.Add(addedVert);
            UpdateGraphic();
        }

        GraphVert targetVert1 = null, targetVert2 = null;

        private void edgesCreation(MouseButtonEventArgs e) 
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                targetVert1 = null;
                targetVert2 = null;
                ResetSelection();
            }
            if (targetVert1 == null || targetVert2 == null)
            {
                targetElement = Mouse.DirectlyOver as UIElement;
                if (targetElement != null)
                {
                    // targetPoint = e.GetPosition(targetElement);
                    foreach (var gv in gVerts)
                    {
                        if (gv.VertVisual.VertElipse == targetElement)
                        {
                            gv.VertVisual.VertElipse.Fill = Brushes.Green;
                            if (targetVert1 == null)
                                targetVert1 = gv;
                            else
                                targetVert2 = gv;
                        }
                    }
                }
            }
            if (targetVert1 != null && targetVert2 != null) // NOT ELSE!!!
            {
                PutInBuffer();
                GraphEdge graphEdge = new GraphEdge(targetVert1, targetVert2);
                if( !targetVert1.ConnectedEdges.Exists( x => x.HasEqualValues(graphEdge) ) )
                    targetVert1.ConnectedEdges.Add(graphEdge);
                targetVert1 = null;
                targetVert2 = null;
                ResetSelection();
                gEdges.Add(graphEdge);
                inicEdges();
                UpdateGraphic();
            }
        }

        private void mouse_down_on_canvas(Object sender, MouseButtonEventArgs e) 
        {
            if (CurToolFunc == null)
                CurToolFunc = vertexSelection;
            CurToolFunc(e);
        }

        private void key_events(object sender, KeyEventArgs e) 
        {
            if ( targetVert != null && e.Key == Key.Delete) 
            {
                PutInBuffer();

                foreach ( var gv in gVerts ) 
                {
                    if (gv.VertVisual.VertElipse == targetElement) 
                    {
                        List<GraphEdge> edgesToRemove = new List<GraphEdge>();
                        foreach (var ge in gEdges)
                        {
                            if (ge.ConnectedVert == gv || ge.Route == gv)
                            {
                                edgesToRemove.Add(ge);
                                ge.ConnectedVert.ConnectedEdges.Remove(ge);
                                ge.Route.ConnectedEdges.Remove(ge);
                            }
                        }
                        foreach (var edge in edgesToRemove)
                            gEdges.Remove(edge);

                        gVerts.Remove(gv);

                        targetVert = null;
                        targetElement = null;
                        break;
                    }
                }
                
                UpdateGraphic();
                return;
            }
            if (targetEdge != null && e.Key == Key.Delete) 
            {
                PutInBuffer();
                foreach (var ge in gEdges)
                {
                    if (ge.Visual.VisualEdge == targetElement)
                    {
                        ge.RemoveFromRoute();
                        gEdges.Remove(ge);
                        break;
                    }
                }

                targetEdge = null;
                targetElement = null;

                UpdateGraphic();
                return;
            }
        }

        private void Update(GraphVert[] verts)
        {
            AutoNamer.ResetDynamicPart();
            gVerts = new List<GraphVert>(verts);
            GraphCanvas.Children.Clear();
            int count = gVerts.Count();
            double offset = (3.14 * 2) / count;

            for (int i = 0; i < count; ++i)
            {
                if(!gVerts[i].VertVisual.isPosed)
                    gVerts[i].VertVisual.SetPos(200 + Math.Cos(offset * i) * 100, 200 + Math.Sin(offset * i) * 100);
                gVerts[i].VertVisual.VertName.Text = gVerts[i].VertName;
            }

            inicEdges();

            UpdateGraphic();
        }

        class textonly 
        {
            public string name;
            public textonly(string text) 
            {
                name = text;
            }
        }


        public void UpdateGraphic() 
        {
            GraphCanvas.Children.Clear();
            UpdateEdges();

            ajtab.GenTable(gVerts);

            foreach ( var gv in gVerts)
                gv.VertVisual.AddOnCanv(GraphCanvas);
        }

        private void UpdateEdges() 
        {
            List<GraphEdge> removalList = new List<GraphEdge>();
            foreach ( var ge in gEdges )
            {
                if(!gVerts.Exists( x => { return x == ge.Route || x == ge.ConnectedVert; })) 
                {
                    removalList.Add(ge);
                    continue;
                }
                ge.UpdateCords();
                ge.Visual.AddOnCanvas(GraphCanvas);
            }
            foreach (var edge in removalList) 
                gEdges.Remove(edge);
        }

        public void save_graph_code_file(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            if(fd.ShowDialog() == true)
                GraphSaver.SaveGraphCodeFile(fd.FileName, gVerts, gEdges);
        }

        public void save_aj_mat_file(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            if (fd.ShowDialog() == true)
                GraphSaver.SaveAjacentyMatFile(fd.FileName, gVerts);
        }

        public void save_inc_mat_file(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "txt|*.txt";
            if (fd.ShowDialog() == true)
                GraphSaver.SaveIncMatFile(fd.FileName, gVerts, gEdges);
        }


        public void about_program(Object sender, RoutedEventArgs e)
        {
            string buff = FileIntput.ReadFile("../../instructions.txt");
            MessageBox.Show(buff);
        }

        public void about_aughtor(Object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа создана студентами НИУ МАИ\n8 Факультет. Кафедра 31_.\nГруппа М8О-310Б-19.\nЛозница Иван, Смирнов Олег\n2021 год.");
        }
    }

    class AutoNamer 
    {
        private const string constNamePart = "Vertex_";
        private static uint dynamicNamePart = 0;

        private static AutoNamer namer = null;

        public static string GetName() 
        {
            string name = constNamePart + dynamicNamePart.ToString();
            dynamicNamePart++;
            return name;
        }

        public static void ResetDynamicPart()
        {
            dynamicNamePart = 0;
        }

        private AutoNamer() 
        {
            dynamicNamePart = 0;
        }

    }


    class BaseViewModel : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class obsCollection : BaseViewModel 
    {
        private ObservableCollection<string> _names;
        public ObservableCollection<string> Names
        {
            get { return _names; }
            set
            {
                _names = value;
                OnPropertyChanged("Names");
            }
        }
        public obsCollection() 
        {
            Names = new ObservableCollection<string>();
        }
        public obsCollection(List<string> str)
        {
            Names = new ObservableCollection<string>(str);
        }
    }

    class GridValue : BaseViewModel
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Name");
            }
        }
        public GridValue( string val) 
        {
            Value = val;
        }
    }
    class MyColum : BaseViewModel
    {
        private string _name;
        public string Name 
        {
            get { return _name; }
            set 
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        private ObservableCollection<GridValue> _vals;
        public ObservableCollection<GridValue> Vals
        {
            get { return _vals; }
            set
            {
                _vals = value;
                OnPropertyChanged("Vals");
            }
        }

        public MyColum() 
        {
            Vals = new ObservableCollection<GridValue>();
        }
        public MyColum(string name)
        {
            Name = name;
            Vals = new ObservableCollection<GridValue>();
        }

    }

    class AjTable : BaseViewModel 
    {
        // private ObservableCollection<obsCollection> _lines;
        private ObservableCollection<MyColum> _colums;

        /*public ObservableCollection< obsCollection > Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged("Lines");
            }
        }*/
        public ObservableCollection<MyColum> Colums
        {
            get { return _colums; }
            set
            {
                _colums = value;
                OnPropertyChanged("Colums");
            }
        }

        public AjTable() 
        {
            //Lines = new ObservableCollection<obsCollection>();
            Colums = new ObservableCollection<MyColum>();
        }

        public void GenTable(List<GraphVert> verts) 
        {
            Colums.Clear();
            MyColum col = new MyColum("0");
            foreach (var vert in verts)
                col.Vals.Add(new GridValue(vert.VertName));
            Colums.Add(col);
        }
        
    }

    class GraphsVisual : BaseViewModel
    {
        public GraphVert[] gVerts;
        private string _gt;
        public String GraphTextInfo { get => _gt; set { _gt = value; OnPropertyChanged(nameof(GraphTextInfo)); } }
        public GraphsVisual()
        {
            gVerts = null;
            GraphTextInfo = "TEST";
        }

        public void Update(GraphVert[] verts)
        {
            gVerts = verts;
            int count = gVerts.Length;
            GraphTextInfo = "";
            for (int i = 0; i < count; ++i)
            { 
                GraphTextInfo += verts[i].GetTextInfo();
            }
        }
    }
}
