using ELTE.Map.Navigation.Utils.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ELTE.Map.Navigation.Utils.Calculate
{
    public class ConstructAdjacencyMatrix
    {
        public ConstructAdjacencyMatrix()
        {
        }
        /// <summary>
        /// A kapott szakasz lista pontjaiból szomszédossági mátrixot alakít,
        /// ha i és j pont szomszédos, a mátrixba az (i,j) indexű helyre a távolságuk kerül, 
        /// megszorozva a kapott konstans szorzóval
        /// ha i és j pont nem szomszédos, a mátrixba az (i,j) indexű helyre 0 kerül.
        /// </summary>
        /// <param name="receivedList">szakaszok listája</param>
        /// <param name="myConst">szorzó konstans</param>
        /// <returns></returns>
        public static float[,] ConstructTheMatrix(List<LineSegment> receivedList, float myConst)
        {

            List<PointF> list = ConstructListOfPoints(receivedList);

            float[,] matrix = new float[list.Count, list.Count];

            foreach (LineSegment ls in receivedList)
            {
                int m = list.IndexOf(ls.StartPoint);
                int n = list.IndexOf(ls.EndPoint);

                matrix[m, n] = matrix[n, m] = Util.TwoPointsDistance(ls.StartPoint, ls.EndPoint) * myConst;
            }

            return matrix;
        }

        public static float[,] ConstructTheMatrixTwo(List<LineSegment> receivedList, float myConst)
        {

            List<PointF> list = ConstructListOfPoints(receivedList);

            int count = receivedList.Count;
            float[,] matrix = new float[count, count];

            foreach (LineSegment ls in receivedList)
            {
                int m = list.IndexOf(ls.StartPoint);
                int n = list.IndexOf(ls.EndPoint);

                matrix[m, n] = matrix[n, m] = Util.TwoPointsDistance(ls.StartPoint, ls.EndPoint) * myConst;
            }

            return matrix;
        }

        /// <summary>
        /// A kapott szakasz lista összes előforduló pontjából pontok listáját alakítja,
        /// a pontok sorrendje a szakaszok sorrendjétől függ
        /// </summary>
        /// <param name="receivedList">a szakaszok beérkező listája</param>
        /// <returns>az előforduló pontok listája</returns>
        public static List<PointF> ConstructListOfPoints(List<LineSegment> receivedList)
        {
            List<PointF> list = new List<PointF>();

            foreach (LineSegment ls in receivedList)
            {
                if (!list.Contains(ls.StartPoint))
                {
                    list.Add(ls.StartPoint);
                }

                if (!list.Contains(ls.EndPoint))
                {
                    list.Add(ls.EndPoint);
                }
            }

            return list;
        }
    }
}
