using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace grafs
{
    // Класс сохранения графов 
    class GraphSaver
    {
        // Сохранение в файл в виде списка рёбер/вершин
        public static void SaveGraphCodeFile(string path, List<GraphVert> graph, List<GraphEdge> edges) 
        {
            // Установка англ. формата, чтобы числа с плав. точкой сохранялись как 16.1, а не как 16,1
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            StreamWriter sw = new StreamWriter(path); 
            foreach (var vert in graph) 
            {
                // Запись всеъ вершин
                sw.WriteLine("Vertex{" + vert.VertName + "(" + vert.VertVisual.x + ", " + vert.VertVisual.y + ")}");
            }
            sw.WriteLine("Edges\n{"); // Начало записи рёбер
            bool first = true; // Первая ли запись
            foreach (var edge in edges) 
            {
                if (first) // Если первая запись, то не ставим запятую в конце предыдущей строки
                    first = false;
                else
                    sw.Write(",\n");
                // Запись ребра
                sw.Write("\t" + edge.EdgeName + "(" + edge.EdgeWeight.ToString() + ", " + edge.Route.VertName + ", " + edge.ConnectedVert.VertName + ")");
                if (!edge.IsDirected) 
                {
                    // Если ребро не направленое, то запись еще одного такого же, где начало и конец меняются местами   
                    sw.Write(",\n");
                    sw.Write("\t" + edge.EdgeName + "(" + edge.EdgeWeight.ToString() + ", " + edge.ConnectedVert.VertName + ", " + edge.Route.VertName + ")");
                }
            }

            sw.WriteLine("\n}");
            sw.Flush(); // Запись всего содержимого буфера в файл
            sw.Close(); // Закрытие файла
        }
        // Сохранение матрицы смежности
        public static void SaveAjacentyMatFile(string path, List<GraphVert> graph) 
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write("0"); // Первая ячейка (пустая)
            foreach (var vert in graph)
                sw.Write(" "+vert.VertName); // Запись всех верши
            sw.Write("\n");
            foreach (var vert in graph) 
            {
                sw.Write(vert.VertName); // Имя вершины в начале строки
                foreach (var sub in graph)
                {
                    // Нахождение смежную вершину
                    GraphEdge ed = vert.ConnectedEdges.Find(x => { return (x.ConnectedVert == x.Route || sub != vert) && (x.ConnectedVert == sub || (x.Route == sub && !x.IsDirected)); });
                    if (ed != null) // Если смежная - запись веса
                        sw.Write(" " + ed.EdgeWeight.ToString());
                    else
                        sw.Write(" 0");
                }
                sw.Write("\n");
            }
            sw.Flush();
            sw.Close();
        }
        // Сохранение матрицы инцедентности
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
