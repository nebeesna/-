using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    internal class Matrix
    {
        private double[,] array;
        private int m; //rows
        private int n; //cols
        private static Random random = new Random();

        public Matrix()
        {
            m = 0;
            n = 0;
            array = new double[m, n];
        }
        public Matrix(int a, int b)
        {
            m = a;
            n = b;
            array = new double[m, n];

            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    this.array[i, j] = GetRandomNumbers(-100, 100);
                }
            }
        }
        public Matrix(Matrix other)
        {
            m = other.m;
            n = other.n;
            array = new double[m, n];
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    this.array[i, j] = other.array[i,j];
                }
            }
        }
        public int M
        {
            get { return m; }
        }
        public int N
        {
            get { return n; }
        }
        public double[,] Array
        {
            get { return array; }
        }
        static private int GetRandomNumbers(int min, int max)
        {
            return random.Next(min, max);
        }
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    str += array[i, j] + "\t";
                }
                str += "\n";
            }
            return str;
        }
        public double this[int x, int y]
        {
            get { return array[x, y]; }
            set { array[x, y] = value; }
        }

    }
}
