using ELTE.Map.Navigation.Utils.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ELTE.Map.Navigation.Utils.Calculate
{
    public class LevelCalculation
    {
        PointF LiftPoint;
        PointF StairsPoint;

        RouteCalculate routeCalculate;
        /// <summary>
        /// Az átadott RouteCalculate osztály (ennek kell tartalmaznia a kezdőpontot, célpontot,
        /// a kezdőszintet és a célszintet) és
        /// a megadott szint mátrixa annak alapján, és az összes létező pont alapján        
        /// meghívja a legrövidebb útvonalat számoló dijkstraPath() metódust
        /// </summary>
        /// <param name="routeCalculate">a számolandó útvonal (tartalmazza a kezdőpontot, a célpontot,
        /// a kezdőszintet és a célszintet)</param>
        /// <param name="arrayA">a szint pontjai</param>
        /// <param name="szomszMatrixA">a szint pontjainak szomszédossági mátrixa</param>
        /// <returns></returns>
        public static List<RouteSection> calculateRoute(RouteCalculate routeCalculate,
                                            List<PointF> arrayA,
                                            float[,] szomszMatrixA)
        {

            int StartIndex = arrayA.IndexOf(routeCalculate.StartPoint);
            int EndIndex = arrayA.IndexOf(routeCalculate.EndPoint);

            string str = ""; int L = 0;
            List<RouteSection> finalList;

            finalList =
            MiniDistanceAndPath.dijkstraPath(szomszMatrixA, StartIndex, EndIndex, arrayA, str, L);

            return finalList;
        }

        /// <summary>
        /// Az átadott RouteCalculate osztály (ennek kell tartalmaznia a kezdőpontot, célpontot,
        /// a kezdőszintet és a célszintet) és
        /// két megadott szint mátrixa annak alapján, és az összes létező pont alapján
        /// illetve a megadott kiemelt pontok, úgy mint a liftek és a lépcsők koordinátái (szintén PointF)
        /// alapján meghatározza, hogy egy vagy több szint között kell mozogni és 
        /// meghívja a legrövidebb útvonalat számoló dijkstraPath() metódust
        /// </summary>
        /// <param name="routeCalculate">a számolandó útvonal (tartalmazza a kezdőpontot, a célpontot,
        /// a kezdőszintet és a célszintet)</param>
        /// <param name="arrayA">1. szint pontjai</param>
        /// <param name="szomszMatrixA">1. szint pontjainak szomszédossági mátrixa</param>
        /// <param name="arrayB">2. szint pontjai</param>
        /// <param name="szomszMatrixB">2. szint pontjainak szomszédossági mátrixa</param>
        /// <param name="liftPoint">a liftet meghatórozó PointF</param>
        /// <param name="stairsPoint">a lépcsőt meghatározó PointF</param>
        /// <param name="L">a két szint közötti különbség</param>
        /// <returns>a legrövidebb utat definiáló szakasz-lista</returns>
        public static List<RouteSection> calculateRoute(RouteCalculate routeCalculate,
                                            List<PointF> arrayA,
                                            float[,] szomszMatrixA,
                                            List<PointF> arrayB,
                                            float[,] szomszMatrixB,
                                            PointF liftPoint,
                                            PointF stairsPoint,
                                            int L)
        {

            if (routeCalculate.StartLevel == 2)
            {
                arrayA = arrayB;
                szomszMatrixA = szomszMatrixB;
            }

            if (routeCalculate.EndLevel == 1)
            { 
                arrayB = arrayA;
                szomszMatrixB = szomszMatrixA;
            }

            int StartIndex = arrayA.IndexOf(routeCalculate.StartPoint);
            int EndIndex = arrayA.IndexOf(routeCalculate.EndPoint);

            string str = "";
            List<RouteSection> finalList;
            if (routeCalculate.StartLevel != routeCalculate.EndLevel)
            {

                int LiftIndex = arrayA.IndexOf(liftPoint);
                int StairsIndex = arrayA.IndexOf(stairsPoint);

                float a = MiniDistanceAndPath.dijkstraDistance(szomszMatrixA, StartIndex, LiftIndex);
                float b = MiniDistanceAndPath.dijkstraDistance(szomszMatrixB, LiftIndex, EndIndex);

                float c = MiniDistanceAndPath.dijkstraDistance(szomszMatrixA, StartIndex, StairsIndex);
                float d = MiniDistanceAndPath.dijkstraDistance(szomszMatrixB, StairsIndex, EndIndex);

                if (a + b < c + d)
                {
                    str = "lifttel";
                    finalList =
                    MiniDistanceAndPath.dijkstraPath(szomszMatrixA, StartIndex, LiftIndex, arrayA, str, L);
                    int j = finalList.Count;
                    str = "";
                    List<RouteSection> finalListLevel2 =
                    MiniDistanceAndPath.dijkstraPath(szomszMatrixB, LiftIndex, EndIndex, arrayB, str, L);
                    for (int i = 0; i < finalListLevel2.Count; i++)
                    {
                        finalListLevel2[i].setOrder(j + finalListLevel2[i].Order);
                        finalList.Add(finalListLevel2[i]);
                    }

                }
                else
                {
                    str = "lépcsőn";
                    finalList =
                    MiniDistanceAndPath.dijkstraPath(szomszMatrixA, StartIndex, StairsIndex, arrayA, str, L);
                    int j = finalList.Count;
                    str = "";
                    List<RouteSection> finalListLevel2 =
                    MiniDistanceAndPath.dijkstraPath(szomszMatrixB, StairsIndex, EndIndex, arrayB, str, L);
                    for (int i = 0; i < finalListLevel2.Count; i++)
                    {
                        finalListLevel2[i].setOrder(j + finalListLevel2[i].Order);
                        finalList.Add(finalListLevel2[i]);
                    }

                }
            }
            else
            {
                finalList =
                MiniDistanceAndPath.dijkstraPath(szomszMatrixA, StartIndex, EndIndex, arrayA, str, L);

            }

            return finalList;
        }
    }

}
