using ELTE.Map.Navigation.Utils.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ELTE.Map.Navigation.Utils.Calculate
{
    internal class MiniDistanceAndPath
    {

        private static readonly int NO_PARENT = -1;
        /// <summary>
        /// Egy adott szomszédossági mátrixban (a mátrix elemei gráf csomópontok, a térkép pontjai az
        /// indexeikkel vannak reprezentálve) 
        /// megkeresi a start-csomópont pont és a cél-csomópont között a legrövidebb utat
        /// </summary>
        /// <param name="adjacencyMatrixA">szomszédossági mátrix, gráf-csomópontok</param>
        /// <param name="startVertex">kezdő csomópont, a kezdő pont indexe</param>
        /// <param name="endVertex">cél csomópont, a cél pont indexe</param>
        /// <returns>a legrövidebb út hosszát adja vissza</returns>
        public static float dijkstraDistance(float[,] adjacencyMatrixA,
                                                int startVertex,
                                                int endVertex)
        {

            int nVertices = adjacencyMatrixA.GetLength(0);
            float[] shortestDistances = new float[nVertices];
            bool[] added = new bool[nVertices];

            for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
            {
                shortestDistances[vertexIndex] = float.MaxValue;
                added[vertexIndex] = false;
            }

            shortestDistances[startVertex] = 0;

            int[] parents = new int[nVertices];
            parents[startVertex] = NO_PARENT;
            PointF[] parentPoints = new PointF[nVertices];

            for (int i = 1; i < nVertices; i++)
            {
                int nearestVertex = -1;
                float shortestDistance = float.MaxValue;
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    if (!added[vertexIndex] &&
                        shortestDistances[vertexIndex] <
                        shortestDistance)
                    {
                        nearestVertex = vertexIndex;
                        shortestDistance = shortestDistances[vertexIndex];
                    }
                }
                if (nearestVertex != -1)
                {
                    added[nearestVertex] = true;
                }
                else Console.WriteLine("üres mátrixszal nem tudunk dolgozni");

                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    float edgeDistance = adjacencyMatrixA[nearestVertex, vertexIndex];

                    if (edgeDistance > 0
                        && ((shortestDistance + edgeDistance) <
                            shortestDistances[vertexIndex]))
                    {
                        parents[vertexIndex] = nearestVertex;

                        shortestDistances[vertexIndex] = shortestDistance +
                                                        edgeDistance;
                    }
                }
            }

            return shortestDistances[endVertex];
        }
        /// <summary>
        /// Egy adott szomszédossági mátrixban (a mátrix elemei gráf csomópontok, a térkép pontjai az
        /// indexeikkel vannak reprezentálve) 
        /// megkeresi a start-csomópont pont és a cél-csomópont között a legrövidebb utat
        /// </summary>
        /// <param name="adjacencyMatrix">szomszédossági mátrix, gráf-csomópontok</param>
        /// <param name="startVertex">kezdő csomópont, a kezdő pont indexe</param>
        /// <param name="endVertex">cél csomópont, a cél pont indexe</param>
        /// <param name="pointsA">a térkép pontjainak listája</param>
        /// <param name="str">egy sztring, mely megmondja, hogy egy szintes a keresés: "",
        /// liftet kell használni: "lifttel", vagy lépcsőt kell használni: "lépcsőn"</param>
        /// <param name="L">a szintek közötti különbség</param>
        /// <returns>a legrövideb utat képező szakaszok listáját adja vissza</returns>
        public static List<RouteSection> dijkstraPath(float[,] adjacencyMatrix,
                                    int startVertex,
                                    int endVertex,
                                    List<PointF> pointsA,
                                    string str,
                                    int L)
        {
            int nVertices = adjacencyMatrix.GetLength(0);
            float[] shortestDistances = new float[nVertices];
            bool[] added = new bool[nVertices];

            for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
            {
                shortestDistances[vertexIndex] = float.MaxValue;
                added[vertexIndex] = false;
            }

            shortestDistances[startVertex] = 0;

            int[] parents = new int[nVertices];
            parents[startVertex] = NO_PARENT;
            PointF[] parentPoints = new PointF[nVertices];

            for (int i = 1; i < nVertices; i++)
            {
                int nearestVertex = -1;
                float shortestDistance = float.MaxValue;
                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    if (!added[vertexIndex] &&
                        shortestDistances[vertexIndex] <
                        shortestDistance)
                    {
                        nearestVertex = vertexIndex;
                        shortestDistance = shortestDistances[vertexIndex];
                    }
                }
                if (nearestVertex != -1)
                {
                    added[nearestVertex] = true;
                }
                else Console.WriteLine("üres mátrixszal nem tudunk dolgozni");

                for (int vertexIndex = 0;
                        vertexIndex < nVertices;
                        vertexIndex++)
                {
                    float edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];       //nearestvertex: -1 vertexindex:0

                    if (edgeDistance > 0
                        && ((shortestDistance + edgeDistance) <
                            shortestDistances[vertexIndex]))
                    {
                        parents[vertexIndex] = nearestVertex;
                        parentPoints[vertexIndex] = pointsA[nearestVertex];
                        shortestDistances[vertexIndex] = shortestDistance +
                                                        edgeDistance;
                    }
                }
            }

            List<RouteSection> RouteList = new List<RouteSection>();
            List<PointF> myList = setSolution(startVertex, endVertex, parents, pointsA);

            if (startVertex != endVertex)
            {
                String flag = "N";
                String flagOrient = "";
                RouteSection rs = new RouteSection();
                RouteList.Add(rs);
                RouteList[0].setOrder(1);
                RouteList[0].setStartPoint(myList[0]);

                int m = pointsA.IndexOf(myList[0]);
                int n = pointsA.IndexOf(myList[1]);
                float dist = adjacencyMatrix[m, n];

                RouteList[0].setDistance(dist);
                RouteList[0].setMessage(" Haladjon előre " + dist + " métert -> ");

                if (myList[0].X - myList[1].X > 0) flag = "W";
                else if (myList[0].X - myList[1].X < 0) flag = "E";
                else if (myList[0].X - myList[1].X < 0) flag = "N";
                else if (myList[0].X - myList[1].X > 0) flag = "S";

                for (int i = 1; i < myList.Count - 1; i++)
                {
                    RouteSection ro_se = new RouteSection();
                    RouteList.Add(ro_se);
                    RouteList[i].setOrder(i + 1);
                    RouteList[i].setStartPoint(myList[i]);
                    RouteList[i - 1].setEndPoint(myList[i]);

                    float fx = myList[i + 1].X - myList[i].X;
                    float fy = myList[i + 1].Y - myList[i].Y;

                    switch (flag)
                    {
                        case "N":
                            if (fx < 0) { flag = "W"; flagOrient = " Forduljon balra."; }
                            else if (fx > 0) { flag = "E"; flagOrient = " Forduljon jobbra."; }
                            if (fy < 0) { flag = "N"; flagOrient = ""; }
                            break;
                        case "S":
                            if (fx < 0) { flag = "W"; flagOrient = " Forduljon jobbra."; }
                            else if (fx > 0) { flag = "E"; flagOrient = " Forduljon balra."; }
                            if (fy > 0) { flag = "S"; flagOrient = ""; }
                            break;
                        case "E":
                            if (fx > 0) { flag = "E"; flagOrient = ""; }
                            if (fy < 0) { flag = "N"; flagOrient = " Forduljon balra."; }
                            else if (fy > 0) { flag = "S"; flagOrient = " Forduljon jobbra."; }
                            break;
                        case "W":
                            if (fx < 0) { flag = "W"; flagOrient = ""; }
                            if (fy < 0) { flag = "N"; flagOrient = " Forduljon jobbra."; }
                            else if (fy > 0) { flag = "S"; flagOrient = " Forduljon balra."; }
                            break;
                    }

                    m = pointsA.IndexOf(myList[i]);
                    n = pointsA.IndexOf(myList[i + 1]);
                    dist = adjacencyMatrix[m, n];

                    RouteList[i].setDistance(dist);
                    RouteList[i].setMessage(" " + flagOrient + " Haladjon előre " + dist + " métert -> ");

                }
                if (!str.Equals(""))
                {
                    string strUtil;
                    if (L > 0)
                    {
                        strUtil = "fel";
                    }
                    else
                    {
                        strUtil = "le";
                        L = -L;
                    }
                    RouteList[myList.Count - 2].setEndPoint(myList[myList.Count - 1]);
                    string myString =
                            " Menjen " + strUtil + " a " + str + " " + L + " emeletet. ";
                    RouteList[myList.Count - 2].setMessage(RouteList[myList.Count - 2].Message + myString);
                }

                else
                {
                    RouteList[myList.Count - 2].setEndPoint(myList[myList.Count - 1]);
                    RouteList[myList.Count - 2].setMessage(RouteList[myList.Count - 2].Message +
                        " Megérkezett a célhoz! ");
                }
            }
            else
            {
                string strStayInOnePlace;
                if (!str.Equals(""))
                {
                    string strUtil;
                    if (L > 0)
                    {
                        strUtil = "fel";
                    }
                    else
                    {
                        strUtil = "le";
                        L = -L;
                    }
                    strStayInOnePlace = " Menjen " + strUtil + " a " + str + " " + L + " emeletet. ";
                    RouteSection rs = new RouteSection(1, pointsA[startVertex], pointsA[startVertex], 0, strStayInOnePlace);
                    rs.setMessage(strStayInOnePlace);
                    rs.setOrder(1);
                    RouteList.Add(rs);
                }

                else
                {
                    strStayInOnePlace = " Megérkezett a célhoz! ";
                    RouteSection rs = new RouteSection(1, pointsA[startVertex], pointsA[startVertex], 0, strStayInOnePlace);
                    rs.setMessage(strStayInOnePlace);
                    rs.setOrder(1);
                    RouteList.Add(rs);
                }
            }

            return RouteList;
        }

        public static List<PointF> setSolution(int startVertex,
                                        int endVertex,
                                        int[] parents,
                                        List<PointF> parentPoints)
        {
            int vertexIndex = endVertex;

            List<PointF> workList = new List<PointF>();

            if (vertexIndex != startVertex)
            {
                setPath(vertexIndex, parents, parentPoints, workList);
            }
            else
            {

            }

            return workList;
        }


        /// <summary>
        /// segéd (util) metódus, gyakorlatilag egy getter
        /// </summary>
        /// <param name="routeList"></param>
        /// <returns></returns>
        public static List<PointF> getRouteList(List<PointF> routeList)
        {

            return routeList;
        }
        /// <summary>
        /// rekurzióban visszafelé kalkulálja a célponttól a
        /// szülő-pontokat a startpontig
        /// </summary>
        /// <param name="currentVertex">aktuális gráf-csomópont</param>
        /// <param name="parents">a gráf-csomópontok szülő csomópontjai</param>
        /// <param name="parentPoints">a pontok (PointF) szülő pontjai</param>
        /// <param name="listToReturn">az útvonal PointF csomópontokban kifejezve</param>
        public static void setPath(int currentVertex,
                                    int[] parents,
                                    List<PointF> parentPoints,
                                    List<PointF> listToReturn)
        {

            if (currentVertex == NO_PARENT)
            {
                getRouteList(listToReturn);
                return;
            }

            setPath(parents[currentVertex], parents, parentPoints, listToReturn);
            listToReturn.Add(parentPoints[currentVertex]);
        }


    }
}
