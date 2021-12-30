using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace grafs
{
    class GraphImport
    {
        public static GraphVert[] CreateGraphFromAdjacentyMatrix(String adjMat)
        {
            GraphVert[] retGraph;
            Queue<String> lines = new Queue<String>(adjMat.Split('\n'));

            String line = lines.Dequeue();
            Queue<String> names = new Queue<String>(line.Split(' '));
            names.Dequeue();
            retGraph = new GraphVert[names.Count];
            int curArrayPosition = 0;
            foreach (var name in names)
            {
                retGraph[curArrayPosition] = new GraphVert(name);
                retGraph[curArrayPosition].VertVisual = new VisualGraphVert();
                curArrayPosition++;
            }

            curArrayPosition = 0;
            foreach (var l in lines)
            {
                Queue<String> adjs = new Queue<String>(l.Split(' '));
                string vertName = adjs.Dequeue();
                int pos = 0;
                foreach (var adj in adjs)
                {
                    if (adj != "0")
                    {
                        GraphEdge ge = new GraphEdge(retGraph[curArrayPosition], retGraph[pos]);
                        ge.AddAdjacenty();
                    }
                    ++pos;
                }
                curArrayPosition++;
            }

            return retGraph;
        }

        public static GraphVert[] CreateGraphFromIncidenceMatrix(String incMatrix)
        {
            GraphVert[] retGraph;

            LinkedList<String> lines = new LinkedList<String>(incMatrix.Split('\n'));

            GraphEdge[] edges = null;
            int edgesCount = 0;

            // Do auto namer
            char vertName = '1';

            lines.RemoveLast();
            int count = lines.Count();

            retGraph = new GraphVert[count];
            for (int i = 0; i < count; ++i)
            {
                retGraph[i] = new GraphVert(vertName.ToString());
                retGraph[i].VertVisual = new VisualGraphVert();
                vertName++;
            }

            int strN = 0;
            foreach (var line in lines)
            {
                Queue<String> qLine = new Queue<String>(line.Split(' '));
                if (edges == null)
                {
                    edgesCount = qLine.Count;
                    edges = new GraphEdge[edgesCount];
                    for (int i = 0; i < edgesCount; ++i)
                    {
                        edges[i] = new GraphEdge();
                    }
                }
                int pos = 0;
                foreach (var val in qLine)
                {
                    if (val != "0")
                    {
                        edges[pos].AddVert(retGraph[strN]);
                        edges[pos].EdgeWeight = Int32.Parse(val);
                    }
                    ++pos;
                }
                ++strN;
            }

            for (int i = 0; i < edgesCount; ++i)
            {
                edges[i].IsDirected = false;
                edges[i].AddAdjacenty();
            }

            return retGraph;
        }


        class values
        {
            public String edgeName;
            public int weight;
            public String vert1Name;
            public String vert2Name;

            private int valNum;

            public void Reset()
            {
                valNum = 0;
            }
            public void AddValue(String val)
            {
                switch (valNum)
                {
                    case 0:
                        edgeName = val;
                        break;
                    case 1:
                        weight = Int32.Parse(val);
                        break;
                    case 2:
                        vert1Name = val;
                        break;
                    case 3:
                        vert2Name = val;
                        break;
                }
                valNum++;
            }
            public GraphVert[] CreateVerts()
            {
                GraphEdge GE = new GraphEdge();
                GraphVert[] ret = new GraphVert[2];
                ret[0] = new GraphVert(vert1Name);
                ret[1] = new GraphVert(vert2Name);

                ret[0].VertVisual = new VisualGraphVert();
                ret[1].VertVisual = new VisualGraphVert();

                GE.AddVert(ret[0]);
                GE.AddVert(ret[1]);

                GE.AddAdjacentyToRootVert();

                return ret;
            }
        }

        private static List<GraphVert> scanEdges(List<GraphVert> vertList, string EVList) 
        {
            List<GraphVert> retVerts = vertList;

            string lastWord = "";
            string curWord = "";

            string edgeName = "Edge";
            int edgeWeight = 1;

            bool scanning = false;
            foreach (char a in EVList)
            {
                if (scanning == true)
                {
                    if (curWord != "")
                    {
                        edgeWeight = Int32.Parse(curWord);
                        scanning = false;
                    }
                }
                switch (a)
                {
                    case '(':

                        scanning = true;

                        edgeName = curWord;
                        lastWord = curWord;
                        curWord = "";
                        break;
                    case ')':

                        scanning = false;
                        // Create vert

                        GraphVert rootVert = retVerts.Find(x => x.VertName == lastWord);
                        if (rootVert == null)
                        {
                            retVerts.Add(new GraphVert(lastWord));
                            rootVert = retVerts.Last();
                        }
                        GraphVert conVert = retVerts.Find(x => x.VertName == curWord);
                        if (conVert == null)
                        {
                            retVerts.Add(new GraphVert(curWord));
                            conVert = retVerts.Last();
                        }
                        GraphEdge ed = new GraphEdge(rootVert, conVert, edgeWeight, edgeName);
                        ed.AddAdjacenty();


                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '{':

                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '}':
                        return retVerts;

                        lastWord = curWord;
                        curWord = "";
                        break;

                    case '\t':
                    case ',':
                        break;
                    case ' ':
                    case '\n':
                        lastWord = curWord;
                        curWord = "";
                        break;
                    default:
                        curWord += a;
                        break;
                }
            }
            return retVerts;
        }

        private class Cords 
        {
            private double x;
            private double y;
            private short state = 0;
            public void addValue(double value) 
            {
                switch (state) 
                {
                    case 0:
                        x = value;
                        ++state;
                        return;
                    case 1:
                        y = value;
                        ++state;
                        return;
                    default:
                        throw new Exception("Both vars are already set!");
                }
            }
            public double X() { return x; }
            public double Y() { return y; }

        }

        private static GraphVert scanVert(string EVList) 
        {
            GraphVert retVert = null;
            string lastWord = "";
            string curWord = "";

            string vertName = "Vertex";

            bool scanning = false;

            Cords vertCords = new Cords();            

            foreach (var a in EVList) 
            {
                switch (a)
                {
                    case '(':

                        scanning = true;

                        vertName = curWord;
                        lastWord = curWord;
                        curWord = "";
                        break;
                    case ')':

                        scanning = false;
                        // Create vert

                        vertCords.addValue(double.Parse(curWord));
                        
                        retVert = new GraphVert(vertName);
                        retVert.VertVisual.SetPos(vertCords.X(), vertCords.Y());


                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '{':

                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '}':
                        return retVert;

                    case ',':
                        lastWord = curWord;
                        vertCords.addValue(double.Parse(curWord));
                        curWord = "";
                        break;
                    case '\t':
                    case ' ':
                    case '\n':
                        break;
                    default:
                        curWord += a;
                        break;
                }
            }
            return retVert;
        }

        public static GraphVert[] CreateGraphGromEdgVertList(string EVList)
        {
            List<GraphVert> retVerts = new List<GraphVert>();

            short cirBrState = 0;
            short figBrState = 0;
            char lastSymbol = '\0';
            string lastWord = "";
            string curWord = "";

            bool isComment = false;

            string edgeName = "Edge";
            int edgeWeight = 1;

            bool scanning = false;

            for (int i = 0; i < EVList.Length; ++i)
            {
                if (isComment) 
                {
                    if (EVList[i] != '\n')
                        continue;
                    isComment = false;
                }
                if (scanning == true)
                {
                    if(curWord != "")
                    {
                        edgeWeight = Int32.Parse(curWord);
                        scanning = false;
                    }    
                }
                switch (EVList[i])
                {
                    case '{':
                        figBrState++;

                        lastWord = curWord;
                        if (lastWord == "Vertex") 
                            retVerts.Add(scanVert(EVList.Substring(i)));
                        if (lastWord == "Edges")
                            retVerts = scanEdges(retVerts, EVList.Substring(i));
                        curWord = "";
                        break;
                    case '}':
                        figBrState--;

                        lastWord = curWord;
                        curWord = "";
                        break;

                    case '\t':
                    case ',':
                    case ' ':
                    case '\n':
                        break;
                    case '%':
                        isComment = true;
                        break;
                    default:
                        curWord += EVList[i];
                        break;
                }

                lastSymbol = EVList[i];
            }
            return retVerts.ToArray();
        }
    }
}
