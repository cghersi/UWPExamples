using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleImageComparisonClassLibrary.ExtensionMethods
{
    public static class DoubleArrayMethods
    {


        /// <summary>
        /// Helpermethod to print a doublearray of 
        /// </summary>
        /// <typeparam name="T">The type of doublearray</typeparam>
        /// <param name="doubleArray">The doublearray to print</param>
        public static void ToConsole<T>(this T[,] doubleArray)
        {
            for (int y = 0; y < doubleArray.GetLength(0); y++)
            {
                Console.Write("[");
                for (int x = 0; x < doubleArray.GetLength(1); x++)
                {
                    Console.Write(string.Format("{0,3},", doubleArray[x, y]));
                }
                Console.WriteLine("]");
            }
        }


        public static IEnumerable<byte> All(this byte[,] doubleArray)
        {
            int size = doubleArray.GetLength(0) * doubleArray.GetLength(1);
            for (int y = 0; y < doubleArray.GetLength(0); y++)
            {
                for (int x = 0; x < doubleArray.GetLength(1); x++)
                {
                    yield return doubleArray[x, y];
                }
            }
        }

        public static float GetAverage(this byte[] doubleArray)
        {
            return (float) doubleArray.Average(item => item);
        }


        public static byte[] GetDifferences(this byte[] firstGray, byte[] secondGray)
        {
            byte[] differences = new byte[firstGray.Length];

            for (int y = 0; y < firstGray.Length; y++)
            {
                    differences[y] = (byte)Math.Abs(firstGray[y] - secondGray[y]);
            }
            return differences;
        }
    }
}