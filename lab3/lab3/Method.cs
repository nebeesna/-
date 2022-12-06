using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lab3
{
    internal class Method
    {
        public class SquareRootMethodParalel
        {
            static public double[] y;
            static public double[] x;
            static public Matrix t;

            static public Matrix matrix;
            static public double[] b;
            static public int u = 0;

            public static void Constructor(Matrix param1, double[] param2)
            {
                matrix = new Matrix(param1);
                b = new double[param2.Length];
                x = new double[param2.Length];
                y = new double[param2.Length];
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = param2[i];
                }
                t = new Matrix(matrix.N, matrix.N);
                t.setZeros();
            }

            public static void startMethodByParalel(Object o)
            {
                DataForParalel d = (DataForParalel)o;
                int start = d.start;
                int end = d.end;
                if (Matrix.isSymmetrical(matrix))
                {
                    for (int i = start; i <= end; i++)
                    {
                        for (int j = 0; j < matrix.N; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                t[i, j] = Math.Sqrt(matrix[i, j]);
                            }
                            else if (i == 0 && j > i)
                            {
                                t[i, j] = matrix[i, j] / t[0, 0];
                            }
                            else if ((i == j) && (i != 0 && j != 0) && (0 < i && i < matrix.N))
                            {
                                double sum = 0;
                                for (int k = 0; k < i; k++)
                                {
                                    sum += Math.Pow(t[k, i], 2);
                                }
                                t[i, i] = Math.Sqrt(matrix[i, i] - sum);
                            }
                            else if (i < j)
                            {
                                double sum = 0;
                                for (int k = 0; k < i; k++)
                                {
                                    sum += t[k, i] * t[k, j];
                                }
                                t[i, j] = (matrix[i, j] - sum) / t[i, i];
                            }
                            else if (i > j)
                            {
                                t[i, j] = 0;
                            }
                        }
                    }
                    for (int i = 0; i < matrix.M; i++)
                    {
                        if (i == 0)
                        {
                            y[0] = b[0] / t[0, 0];
                        }
                        else if (i > 0)
                        {
                            double sum = 0;
                            for (int k = 0; k < i; k++)
                            {
                                sum += t[k, i] * y[k];
                            }
                            y[i] = (b[i] - sum) / t[i, i];
                        }
                    }
                }
                else throw new ArgumentException("The given matrix is not symetrical!");
            }
            public static void findXParalel(Object o)
            {
                DataForParalel d = (DataForParalel)o;
                for (int i = matrix.M - 1; i >= 0; i--)
                {
                    if (i == matrix.M - 1)
                    {
                        x[matrix.M - 1] = y[matrix.M - 1] / t[matrix.M - 1, matrix.M - 1];
                    }
                    else if (i < matrix.M - 1)
                    {
                        double sum = 0;
                        for (int k = i + 1; k < matrix.M; k++)
                        {
                            sum += t[i, k] * x[k];
                        }
                        x[i] = (y[i] - sum) / t[i, i];
                    }
                }
        
            }
        }
        public class SquareRootMethod
        {
            public static void StartMethod(Object o)
            {
                Data d = (Data)o;
                Matrix matrix = new Matrix(d.m);
                double[] b = new double[d.b.Length];
                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = d.b[i];
                }
                double[] x = new double[b.Length];
                double[] y = new double[b.Length];
                if (Matrix.isSymmetrical(matrix))
                {

                    Matrix t = new Matrix(matrix.M, matrix.N);
                    for (int i = 0; i < matrix.M; i++)
                    {
                        for (int j = 0; j < matrix.N; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                t[i, j] = Math.Sqrt(matrix[i, j]);
                            }
                            else if (i == 0 && j > i)
                            {
                                t[i, j] = matrix[i, j] / t[0, 0];
                            }
                            else if ((i == j) && (i != 0 && j != 0) && (0 < i && i < matrix.N))
                            {
                                double sum = 0;
                                for (int k = 0; k < i; k++)
                                {
                                    sum += Math.Pow(t[k, i], 2);
                                }
                                t[i, i] = Math.Sqrt(matrix[i, i] - sum);
                            }
                            else if (i < j)
                            {
                                double sum = 0;
                                for (int k = 0; k < i; k++)
                                {
                                    sum += t[k, i] * t[k, j];
                                }
                                t[i, j] = (matrix[i, j] - sum) / t[i, i];
                            }
                            else if (i > j)
                            {
                                t[i, j] = 0;
                            }
                        }
                    }
                    for (int i = 0; i < matrix.M; i++)
                    {
                        if (i == 0)
                        {
                            y[0] = b[0] / t[0, 0];
                        }
                        else if (i > 0)
                        {
                            double sum = 0;
                            for (int k = 0; k < i; k++)
                            {
                                sum += t[k, i] * y[k];
                            }
                            y[i] = (b[i] - sum) / t[i, i];
                        }
                    }
                    for (int i = matrix.M - 1; i >= 0; i--)
                    {
                        if (i == matrix.M - 1)
                        {
                            x[matrix.M - 1] = y[matrix.M - 1] / t[matrix.M - 1, matrix.M - 1];
                        }
                        else if (i < matrix.M - 1)
                        {
                            double sum = 0;
                            for (int k = i + 1; k < matrix.M; k++)
                            {
                                sum += t[i, k] * x[k];
                            }
                            x[i] = (y[i] - sum) / t[i, i];
                        }
                    }
                }
                else throw new ArgumentException("The given matrix is not symetrical!");
            }
        }
    }
}
