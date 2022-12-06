using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab1
{
    /*
    Варіант 11
    Порахувати статистику матриці випадкових цілих чисел для стовпців: 
    список пар (число масиву, кількість входжень)
    Делегат бібліотечний
     */
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Input number of rows: ");
            int m = Convert.ToInt32(Console.ReadLine());
            Console.Write("Input number of cols: ");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.Write("Input number of threads: ");
            int k = Convert.ToInt32(Console.ReadLine());
            Matrix matrix = new Matrix(m, n);


            List<Dictionary<int, int>> res1 = new List<Dictionary<int, int>>();
            var experiment1 = System.Diagnostics.Stopwatch.StartNew();
            res1 = CalculateStats(matrix);
            experiment1.Stop();
            Console.WriteLine($"\nOne thread(Main thread): {experiment1.Elapsed}");
            //foreach (var item in res1)
            //{
            //    foreach (var item2 in item)
            //    {
            //        Console.Write(item2);
            //    }
            //    Console.WriteLine();
            //}

            Func<Matrix, int, int, List<Dictionary<int, int>>>[] threads = 
                new Func<Matrix, int, int, List<Dictionary<int, int>>>[k];
            int colsForEachThread = matrix.N / k;
            int colsForLastThread = colsForEachThread + matrix.N % k;
            var experiment2 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < k; i++)
            {
                threads[i] = CalculateStatsForColumns;
                if ((i + 1) * colsForEachThread + colsForLastThread <= matrix.N)
                {
                    threads[i].BeginInvoke(matrix, (i + 1) * colsForEachThread, (i + 2) * colsForEachThread - 1, null, null);
                }
                else break;
            }
            IAsyncResult lastThread = threads[k-1].
                BeginInvoke(matrix, matrix.N - colsForLastThread - 1, matrix.N - 1, null, null);
            while (!lastThread.AsyncWaitHandle.WaitOne(500,true)) {   }
            experiment2.Stop();
            Console.WriteLine($"\n{k} threads: {experiment2.Elapsed}");
        }
        public static List<Dictionary<int, int>> CalculateStatsForColumns(Matrix matrix, int startIndex, int endIndex)
        {
            List<Dictionary<int, int>> stats = new List<Dictionary<int, int>>();
            int val;
            if(startIndex == endIndex)
            {
                for (int i = 0; i < matrix.M; i++)
                {
                    if (stats[0].ContainsKey(matrix[i, startIndex]))
                    {
                        stats[0].TryGetValue(matrix[i, startIndex], out val);
                        stats[0].Remove(matrix[i, startIndex]);
                        stats[0].Add(matrix[i, startIndex], val + 1);
                    }
                    else
                    {
                        stats[0].Add(matrix[i, startIndex], 1);
                    }
                }
            }
            else
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    stats.Add(new Dictionary<int, int>());
                    if (stats[i - startIndex].ContainsKey(matrix[i, startIndex]))
                    {
                        stats[i - startIndex].TryGetValue(matrix[i, startIndex], out val);
                        stats[i - startIndex].Remove(matrix[i, startIndex]);
                        stats[i - startIndex].Add(matrix[i, startIndex], val + 1);
                    }
                    else
                    {
                        stats[i - startIndex].Add(matrix[i, startIndex], 1);
                    }
                }
            }
            
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            return stats;
        }

        public static List<Dictionary<int, int>> CalculateStats(Matrix matrix)
        {
            List<Dictionary<int, int>> stats = new List<Dictionary<int, int>>();
            int val;
            for (int i = 0; i < matrix.N; i++)
            {
                stats.Add(new Dictionary<int, int>());
                for (int j = 0; j < matrix.M; j++)
                {
                    if (stats[i].ContainsKey(matrix[j, i]))
                    {
                        stats[i].TryGetValue(matrix[j, i], out val);
                        stats[i].Remove(matrix[j, i]);
                        stats[i].Add(matrix[j, i], val + 1);
                    }
                    else
                    {
                        stats[i].Add(matrix[j, i], 1);
                    }
                }
            }
            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            return stats;
        }

    }
}
