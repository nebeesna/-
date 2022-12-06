using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5_
{
    internal class DijkstraMethod
    {
        private static void setRandomValues(double[,] m, int n)
        {
            Random random = new Random();
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == j)
                    {
                        m[i, j] = 0;
                    }
                    else
                    {
                        m[i, j] = random.Next(0, 50);
                    }
                }
            }
        }
        public class NotParallel
        {
            public double[,] matrix;
            public int n;
            public NotParallel(int count)
            {
                n = count;
                matrix = new double[n, n];
                setRandomValues(matrix, n);
            }

            public void Start(int start)
            {
                double[] distance = new double[n];
                int index = 0, u = 0;
                bool[] visited = new bool[n];
                double min;

                for (int i = 0; i < n; i++)
                {
                    distance[i] = Double.MaxValue;
                }
                distance[start] = 0;

                for (int i = 0; i < n - 1; i++)
                {
                    min = Double.MaxValue;
                    for (int j = 0; j < n; j++)
                    {
                        if (!visited[j] && distance[j] <= min)
                        {
                            min = distance[j];
                            index = j;
                        }
                    }
                    u = index;
                    visited[u] = true;
                    for (int j = 0; j < n; j++)
                    {
                        if (!visited[j] && matrix[u, j] != 0 && distance[u] != Double.MaxValue &&
                            distance[u] + matrix[u, j] < distance[j])
                        {
                            distance[j] = distance[u] + matrix[u, j];
                        }
                    }
                }
            }
        }
        public class Parallel
        {
            public static double[,] matrix;
            public static int n;
            public static double[] distance;
            public static int index, u;
            public static bool[] visited;
            public static double min;
            static public void Constructor(int count, int start)
            {
                n = count;
                matrix = new double[count, count];
                setRandomValues(matrix, n);
                distance = new double[n];
                visited = new bool[n];
                index = 0;
                u = 0;
                for (int i = 0; i < n; i++)
                {
                    distance[i] = Double.MaxValue;
                }
                distance[start] = 0;
            }
            static public void Constructor(double[,] m, int start)
            {
                n = (int)Math.Sqrt(m.Length);
                matrix = new double[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        matrix[i, j] = m[i, j];
                    }
                }
                distance = new double[n];
                visited = new bool[n];
                index = 0;
                u = 0;
                for (int i = 0; i < n; i++)
                {
                    distance[i] = Double.MaxValue;
                }
                distance[start] = 0;
            }
            static public void StartParallel(int start, int end)
            {
                for (int i = start; i < end - 1; i++)
                {
                    min = Double.MaxValue;
                    for (int j = 0; j < n; j++)
                    {
                        if (!visited[j] && distance[j] <= min)
                        {
                            min = distance[j];
                            index = j;
                        }
                    }
                    u = index;
                    visited[u] = true;
                    for (int j = 0; j < n; j++)
                    {
                        if (!visited[j] && matrix[u, j] != 0 && distance[u] != Double.MaxValue &&
                            distance[u] + matrix[u, j] < distance[j])
                        {
                            distance[j] = distance[u] + matrix[u, j];
                        }
                    }
                }
            }
        }
    }
}
