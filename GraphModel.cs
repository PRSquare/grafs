using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// Не закончено!

namespace grafs
{

    public class GraphModel : INotifyPropertyChanged
    {
        private List<GraphVert> gVerts;
        private List<GraphEdge> gEdges;
        public List<GraphVert> GVerts { get { return gVerts; } set { gVerts = value; ObsGVerts = new ObservableCollection<GraphVert>(value); } }
        public List<GraphEdge> GEdges { get { return gEdges; } set { gEdges = value; ObsGEdges = new ObservableCollection<GraphEdge>(value); } }

        private ObservableCollection<GraphVert> obsGVerts;
        private ObservableCollection<GraphEdge> obsGEdges;

        public ObservableCollection<GraphVert> ObsGVerts
        {
            get { return obsGVerts; }
            set
            {
                obsGVerts = value;
                OnPropertyChanged("ObsGVerts");
            }
        }
        public ObservableCollection<GraphEdge> ObsGEdges
        {
            get { return obsGEdges; }
            set
            {
                obsGEdges = value;
                OnPropertyChanged("ObsGEdges");
            }
        }

        LinkedList<undoBuffer> UndoBuffers = new LinkedList<undoBuffer>();

        struct undoBuffer
        {
            public List<GraphVert> gVerts;
            public List<GraphEdge> gEdges;
        }
        delegate void ToolFunc(double x, double y);
        ToolFunc CurToolFunc = null;

        public void PutInBuffer()
        {
            List<GraphVert> addList = new List<GraphVert>();
            foreach (var vert in GVerts)
                addList.Add((GraphVert)vert.Clone());


            foreach (var vert in addList)
            {
                foreach (var edge in vert.ConnectedEdges)
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

        public void Undo()
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
            GVerts = newVerts.gVerts;
            GEdges = newVerts.gEdges;

            inicEdges();


        }

        public GraphModel()
        {
            GVerts = new List<GraphVert>();
            GEdges = new List<GraphEdge>();
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

        private void Update(GraphVert[] verts)
        {
            AutoNamer.ResetDynamicPart();
            GVerts = new List<GraphVert>(verts);
            int count = GVerts.Count();
            double offset = (3.14 * 2) / count;

            for (int i = 0; i < count; ++i)
            {
                if (!GVerts[i].IsPosed)
                {
                    GVerts[i].x = 200 + Math.Cos(offset * i) * 100;
                    GVerts[i].y = 200 + Math.Sin(offset * i) * 100;
                }
            }

            inicEdges();
        }

        private void inicEdges()
        {
            GEdges = genEdgesFromGraphVertList(GVerts);

            foreach (var edge in GEdges)
            {
                GraphEdge multipEdge = GEdges.Find(x => { return x != edge && x.Route == edge.Route && x.ConnectedVert == edge.ConnectedVert && x.Visual.EdgeNumber == edge.Visual.EdgeNumber; });
                while (multipEdge != null)
                {
                    multipEdge.Visual.EdgeNumber++;
                    multipEdge = GEdges.Find(x => { return x != edge && x.Route == edge.Route && x.ConnectedVert == edge.ConnectedVert && x.Visual.EdgeNumber == edge.Visual.EdgeNumber; });
                }
            }
        }

        public void ShowIt(object e)
        {
            string buff = "";
            foreach (var vert in GVerts)
                buff += vert.VertName + "\n";
            MessageBox.Show(buff);
        }

        private string readFileFromOpenFileDialog()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Text files (*.txt)|*.txt";
            String fileName = "";
            if (fd.ShowDialog() == true)
                fileName = fd.FileName;

            String buff = FileIntput.ReadFile(fileName);
            return buff;
        }

        public void openAjMatFile(Object param)
        {
            String buff = readFileFromOpenFileDialog();
            if (buff != null)
            {
                GraphVert[] verts = GraphImport.CreateGraphFromAdjacentyMatrix(buff);
                GVerts = new List<GraphVert>(verts);
                Update(verts);
            }
        }

        public void openIncMatFile(Object param)
        {
            String buff = readFileFromOpenFileDialog();
            GraphVert[] verts = GraphImport.CreateGraphFromIncidenceMatrix(buff);
            GVerts = new List<GraphVert>(verts);
            Update(verts);
        }

        public void openGraphCodeFile(Object param)
        {
            String buff = readFileFromOpenFileDialog();
            GraphVert[] verts = GraphImport.CreateGraphGromEdgVertList(buff);
            GVerts = new List<GraphVert>(verts);
            Update(verts);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
