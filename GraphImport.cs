using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace grafs
{
    // Класс создания графа
    class GraphImport
    {
        // Создание из матрицы смежности
        public static GraphVert[] CreateGraphFromAdjacentyMatrix(String adjMat)
        {
            GraphVert[] retGraph; // Переменная для записи графа
            Queue<String> lines = new Queue<String>(adjMat.Split('\n')); // Строки

            String line = lines.Dequeue(); // Убирание из очереди первой строки (строка с именами вершин)
            Queue<String> names = new Queue<String>(line.Split(' ')); // Список имён вершин
            names.Dequeue(); // Уничтожение первого (0)
            retGraph = new GraphVert[names.Count]; // Установка кол-ва вершин в графе
            int curArrayPosition = 0; // Текущая позиция в массиве
            foreach (var name in names)
            {
                // Инициализация всех вершин
                retGraph[curArrayPosition] = new GraphVert(name);
                retGraph[curArrayPosition].VertVisual = new VisualGraphVert();
                curArrayPosition++;
            }

            curArrayPosition = 0;
            foreach (var l in lines)
            {
                // Построчное считывание строк матрицы
                Queue<String> adjs = new Queue<String>(l.Split(' '));
                string vertName = adjs.Dequeue(); // Имя вершины вначале строки
                int pos = 0;
                foreach (var adj in adjs)
                {
                    if (adj != "0")
                    {
                        // Если есть связь, то создаётся ребро
                        GraphEdge ge = new GraphEdge(retGraph[curArrayPosition], retGraph[pos]);
                        ge.AddAdjacenty(); // Добавление ребра в вершину
                    }
                    ++pos;
                }
                curArrayPosition++;
            }

            return retGraph;
        }

        // Создание из матрицы инцидентности
        public static GraphVert[] CreateGraphFromIncidenceMatrix(String incMatrix)
        {
            GraphVert[] retGraph;

            LinkedList<String> lines = new LinkedList<String>(incMatrix.Split('\n')); // Строки матрицы

            GraphEdge[] edges = null;
            int edgesCount = 0;

            // Do auto namer
            char vertName = '1';

            lines.RemoveLast(); // Удаление последней пустой строки (перенос)
            int count = lines.Count();

            retGraph = new GraphVert[count];
            for (int i = 0; i < count; ++i)
            {
                // Инициализация вершин
                retGraph[i] = new GraphVert(vertName.ToString());
                retGraph[i].VertVisual = new VisualGraphVert();
                vertName++;
            }

            int strN = 0;
            foreach (var line in lines)
            {
                // Обход матрицы
                Queue<String> qLine = new Queue<String>(line.Split(' ')); // Разделение строки матрицы
                if (edges == null)
                {
                    // Инициализация edges, если не инициализированы
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
                        // Если есть связь, то заносим её в соответсв. ребро
                        edges[pos].AddVert(retGraph[strN]); // Функция, добавляющая значения на пустое место (Route либо ConnectedVert)
                        edges[pos].EdgeWeight = Int32.Parse(val); // Считывание веса
                    }
                    ++pos;
                }
                ++strN;
            }

            for (int i = 0; i < edgesCount; ++i)
            {
                edges[i].IsDirected = false;
                edges[i].AddAdjacenty(); // Запись ребра в ConnectedEdges соединённой вершины
            }

            return retGraph;
        }

        // Метод сканировани Edges
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
                        edgeWeight = Int32.Parse(curWord); // Считвание веса вершины
                        scanning = false;
                    }
                }
                switch (a)
                {
                    case '(':
                        // На следующем проходе запишнтся имя ребра
                        scanning = true; 

                        edgeName = curWord;
                        lastWord = curWord;
                        curWord = "";
                        break;
                    case ')':
                        // Создание ребра
                        scanning = false;
                        // Create vert

                        GraphVert rootVert = retVerts.Find(x => x.VertName == lastWord); // Поиск уже существующей начальной вершины
                        if (rootVert == null)
                        {
                            // Создание, если не существует
                            retVerts.Add(new GraphVert(lastWord));
                            rootVert = retVerts.Last();
                        }
                        GraphVert conVert = retVerts.Find(x => x.VertName == curWord); //  Поиск уже существующей конечной вершины
                        if (conVert == null)
                        {
                            // Создание, если не существует
                            retVerts.Add(new GraphVert(curWord));
                            conVert = retVerts.Last();
                        }
                        GraphEdge ed = new GraphEdge(rootVert, conVert, edgeWeight, edgeName); // Создание ребра
                        ed.AddAdjacenty(); // Присоединение ребра к вершине


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
                        // Запись символа к последнему слову
                        curWord += a;
                        break;
                }
            }
            return retVerts;
        }

        // Класс координат вершины
        private class Cords 
        {
            private double x;
            private double y;
            private short state = 0;
            public void addValue(double value) // Метод присваивает значение не инициализированной координате
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

        // Метод сканирования Vertex
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

                        vertName = curWord; // Запись имени вершины
                        lastWord = curWord;
                        curWord = "";
                        break;
                    case ')':
                        // Создание вершины
                        scanning = false;
                        // Create vert

                        vertCords.addValue(double.Parse(curWord)); // Запись координаты

                        retVert = new GraphVert(vertName);
                        retVert.VertVisual.SetPos(vertCords.X(), vertCords.Y()); // Установка координат


                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '{':

                        lastWord = curWord;
                        curWord = "";
                        break;
                    case '}':
                        return retVert; // Возвращение вершины

                    case ',':
                        lastWord = curWord;
                        vertCords.addValue(double.Parse(curWord)); // Запись координаты
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

        // Метод создания графа из списка рёбер/вершин
        public static GraphVert[] CreateGraphGromEdgVertList(string EVList)
        {
            List<GraphVert> retVerts = new List<GraphVert>();

            short cirBrState = 0;
            short figBrState = 0;
            char lastSymbol = '\0';
            string lastWord = "";
            string curWord = "";

            bool isComment = false;

            for (int i = 0; i < EVList.Length; ++i)
            {
                if (isComment) 
                {
                    // Если комментарий - пропускаем
                    if (EVList[i] != '\n')
                        continue;
                    isComment = false;

                }
                switch (EVList[i])
                {
                    case '{':
                        figBrState++;

                        lastWord = curWord;
                        if (lastWord == "Vertex") // Если Vertex начинаем считывать 
                            retVerts.Add(scanVert(EVList.Substring(i)));
                        if (lastWord == "Edges") // Если Edges начинаем считывать
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
