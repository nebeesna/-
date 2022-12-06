using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lab4
{
    internal class Method
    {
        public static double e = 0.01;
        public static int k = 0;
        public class RelaxationMethod
        {
            public static void Start(Object o)
            {
                Data d = (Data)o;
                Matrix param1 = d.m;
                double[] param2 = d.b;
                if (param1.M == param1.N && param2.Length == param1.M)
                {
                    Matrix matrix = new Matrix(param1.M, param1.N);
                    double[] b = new double[param2.Length];
                    int n = param1.N; //ширина, довжина матриці
                    //готуємо матрицю до релаксації
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            matrix[i, j] = -param1[i, j] / param1[i, i];
                        }
                        b[i] = param2[i] / param1[i, i];
                    }
                    //готово
                    //початкове наближення нехай буде х=(0,0,...,0)
                    double[] x = new double[n];
                    double[] x_res = new double[n];
                    double[] r = new double[n];
                    int indexOfMaxVal = 0;
                    double bx;
                    double sum;

                    for (int i = 0; i < n; i++)
                    {
                        sum = 0;
                        for (int j = 0; j < n; j++)
                        {
                            if(j != i)
                                sum += matrix[i, j] * x[j];
                        }
                        r[i] = b[i] - x[i] + sum;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (Math.Abs(r[i]) >= Math.Abs(r[indexOfMaxVal]))
                        {
                            indexOfMaxVal = i;
                        }
                    }
                    do
                    {
                        bx = r[indexOfMaxVal];

                        for (int i = 0; i < n; i++)
                        {
                            if (i != indexOfMaxVal)
                            {
                                r[i] = r[i] + matrix[i, indexOfMaxVal] * bx;
                                x[i] = 0;
                            }
                            else
                            {
                                x[indexOfMaxVal] += r[indexOfMaxVal];
                                r[indexOfMaxVal] = 0;
                                x_res[i] += x[indexOfMaxVal];
                            }
                        }
                       
                        indexOfMaxVal = 0;
                        for (int i = 0; i < n; i++)
                        {
                            if (Math.Abs(r[i]) >= Math.Abs(r[indexOfMaxVal]))
                            {
                                indexOfMaxVal = i;
                            }
                        }
                    } while (Math.Abs(r[indexOfMaxVal]) > e);
                    //return x_res;
                }
                else throw new ArgumentException("Dimensions of matrix or array are not equal!");
            }
        }
        public class RelaxationMethodParalel
        {
            static public Matrix matrix;
            static public double[] b;
            static public int n;
            static public double[] x;
            static public double[] x_res;
            static public double[] r;
            static public int indexOfMaxVal;
            static public double bx;
            static public double sum;
            public static void Constructor(Object o)
            {
                Data d = (Data)o;
                Matrix param1 = d.m;
                double[] param2 = d.b;

                matrix = new Matrix(param1.M, param1.N);
                b = new double[param2.Length];
                n = param1.N;
                x = new double[n];
                x_res = new double[n];
                r = new double[n];

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        matrix[i, j] = -param1[i, j] / param1[i, i];
                    }
                    b[i] = param2[i] / param1[i, i];
                }

                for (int i = 0; i < n; i++)
                {
                    sum = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j != i)
                            sum += matrix[i, j] * x[j];
                    }
                    r[i] = b[i] - x[i] + sum;
                }
            }
            public static void findIndexOfMaxval()
            {
                indexOfMaxVal = 0;
                for (int i = 0; i < n; i++)
                {
                    if (Math.Abs(r[i]) >= Math.Abs(r[indexOfMaxVal]))
                    {
                        indexOfMaxVal = i;
                    }
                }
            }
            public static void Start(Object o)
            {
                Data d = (Data)o;
                int start = d.start;
                int end = d.end;

                findIndexOfMaxval();
                
                bx = r[indexOfMaxVal];

                for (int i = start; i < end; i++)
                {
                    if (i != indexOfMaxVal)
                    {
                        r[i] = r[i] + matrix[i, indexOfMaxVal] * bx;
                        x[i] = 0;
                    }
                    else
                    {
                        x[indexOfMaxVal] += r[indexOfMaxVal];
                        r[indexOfMaxVal] = 0;
                        x_res[i] += x[indexOfMaxVal];
                    }
                }

            }

        }
    }
}
