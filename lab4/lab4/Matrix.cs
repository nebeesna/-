using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4
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
                    this.array[i, j] = other.array[i, j];
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
        public void setZeros()
        {
            for (int i = 0; i < this.m; i++)
            {
                for (int j = 0; j < this.n; j++)
                {
                    this.array[i, j] = 0;
                }
            }
        }
        public void makeSymmetrical()
        {
            for (int i = 0; i < this.m; i++)
            {
                for (int j = i; j < this.n; j++)
                {
                    array[j, i] = array[i, j];
                }
            }
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
        public static bool isSymmetrical(Matrix matrix)
        {
            if(matrix.M == matrix.N)
            {
                for (int i = 0; i < matrix.M; i++)
                {
                    for (int j = 0; j < matrix.N; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }  
                        else if (matrix[j, i] != matrix[i, j])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else return false;
        }

        public Matrix Transpose()
        {
            Matrix transposedMatrix = new Matrix(this.N, this.M);
            for (int i = 0; i < this.N; i++)
            {
                for (int j = 0; j < this.M; j++)
                {
                    transposedMatrix[i,j] = this[j,i];
                }
            }
            return transposedMatrix;
        }
        public double this[int x, int y]
        {
            get { return array[x, y]; }
            set { array[x, y] = value; }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj != null && obj is Matrix)
            {
                Matrix otherMatrix = (Matrix)obj;
                if (this.m == otherMatrix.m && this.n == otherMatrix.n)
                {
                    for (int i = 0; i < this.m; i++)
                        for (int j = 0; j < this.n; j++)
                            if (this.array[i, j] != otherMatrix.array[i, j])
                                return false;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        static public bool operator ==(Matrix other, Matrix another)
        {
            return other.Equals(another);
        }
        static public bool operator !=(Matrix other, Matrix another)
        {
            return !other.Equals(another);
        }
        static public Matrix operator +(Matrix other, Matrix another)
        {
            if (other.m == another.m && other.n == another.n)
            {
                Matrix result = new Matrix(other.m, other.n);
                for (int i = 0; i < result.m; i++)
                    for (int j = 0; j < result.n; j++)
                        result.array[i, j] = other.array[i, j] + another.array[i, j];
                return result;
            }
            else
                throw new InvalidOperationException("invalid matrixes sizes");
        }
        static public Matrix operator -(Matrix other, Matrix another)
        {
            if (other.m == another.m && other.n == another.n)
            {
                Matrix result = new Matrix(other.m, other.n);
                for (int i = 0; i < result.m; i++)
                    for (int j = 0; j < result.n; j++)
                        result.array[i, j] = other.array[i, j] - another.array[i, j];
                return result;
            }
            else
                throw new InvalidOperationException("invalid matrixes sizes");
        }
        static public Matrix operator *(Matrix other, int num)
        {
            Matrix result = new Matrix(other.m, other.n);
            for (int i = 0; i < result.m; i++)
                for (int j = 0; j < result.n; j++)
                    result.array[i, j] = other.array[i, j] * num;
            return result;
        }
        static public Matrix operator *(int num, Matrix other)
        {
            return other * num;
        }
        static public Matrix operator *(Matrix other, Matrix another)
        {
            if (other.m == another.n)
            {
                Matrix result = new Matrix(other.m, another.n);
                for (int i = 0; i < other.m; i++)
                    for (int j = 0; j < another.n; j++)
                        for (int k = 0; k < other.n; k++)
                            result.array[i, j] += other.array[i, k] * another.array[k, j];
                return result;
            }
            else throw new InvalidOperationException("invalid matrixes sizes");
        }
    }
}
