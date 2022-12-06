using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

//Варіант 11
//Метод перетворення матриці випадкових чисел:
//усі елементи замінюються сумою косинуса та синуса
//відповідного елемента матриці.
namespace lab2
{
    internal class Program
    {
        public delegate Matrix TaskDelegate1(Matrix matrix);
        public delegate Matrix TaskDelegate2(Matrix matrix, int a, int b);
        static void Main(string[] args)
        {
            Console.Write("Input number of rows: ");
            int m = Convert.ToInt32(Console.ReadLine());
            Console.Write("Input number of cols: ");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.Write("Input number of threads: ");
            int k = Convert.ToInt32(Console.ReadLine());
            Matrix matrix1 = new Matrix(m, n);
            Matrix matrix2 = new Matrix(matrix1);


            TaskDelegate1 firstExperiment = new TaskDelegate1(matrixTransformation);
            var experiment1 = Stopwatch.StartNew();
            IAsyncResult res1 = firstExperiment.BeginInvoke(matrix1, 
                new AsyncCallback(ResultInfo), "matrixTransformation() is complete!");
            experiment1.Stop();
            firstExperiment.EndInvoke(res1);
            Console.WriteLine($"\nOne thread(Main thread): {experiment1.Elapsed}");

            Console.WriteLine();
            TaskDelegate2 secondExperiment = new TaskDelegate2(matrixTransformationByRow);
            IAsyncResult[] asyncResults = new IAsyncResult[k];
            int rowsForEachThread = matrix2.M / k;
            int rowsForLastThread = rowsForEachThread + matrix2.M % k;
            var experiment2 = Stopwatch.StartNew();
            for (int i = 0; i < k; i++)
            {
                if ((i + 1) * rowsForEachThread + rowsForLastThread <= matrix2.M)
                {
                    asyncResults[i] = secondExperiment.BeginInvoke(matrix2, i * rowsForEachThread, 
                        (i + 1) * rowsForEachThread - 1, new AsyncCallback(ResultInfo), 
                        "matrixTransformationByRow() is complete!");
                }
                else
                {
                    asyncResults[i] = secondExperiment.BeginInvoke(matrix2, matrix2.M - rowsForLastThread, matrix2.M - 1,
                        new AsyncCallback(ResultInfo), "matrixTransformationByRow() is complete!");
                };
            }
            experiment2.Stop();
            foreach (var res in asyncResults)
            {
                secondExperiment.EndInvoke(res);
            }
            Console.WriteLine($"\n{k} threads: {experiment2.Elapsed}");

            /*
            Висновок:
            на невеликих розмірах m і n(0-100) різниця у часі непомітна, проте з використанням небагато потоків(до 100)
            часу витрачається на ~10% менше
            причому немає істотної різниці чи стовпців у багато разів більше чи рядків.
            на більших матрицях (1000-10 000) швидше набагато справляються потоки. різниці між 2 і 10 
            або потоків майже немає. на 1000 потоках часу витрачається трохи більше ніж на 1 потоці.
            якщо потоків багато (1000+) то часу витрачається більше ніж на 1. 
            на 10 000 потоках швидкість падає, бо треба більше часу на створення такої к-сті потоків.  
            наприклад
            1000 потоків - час 0.007с
            10 000 потоків - час 0.07с
            
             */
        }
        public static void ResultInfo(IAsyncResult asyncResult)
        {
            string message = (string)asyncResult.AsyncState;
            //Console.WriteLine($"{message} in thread {Thread.CurrentThread.ManagedThreadId}.");
        }
        static Matrix matrixTransformation(Matrix matrix)
        {
            for (int i = 0; i < matrix.M; i++)
            {
                for (int j = 0; j < matrix.N; j++)
                {
                    matrix[i,j] = Math.Round(Math.Sin(matrix[i, j]) + Math.Cos(matrix[i, j]),2);
                }
            }
            return matrix;
        }
        static Matrix matrixTransformationByRow(Matrix matrix, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                for (int j = 0; j < matrix.N; j++)
                {
                    matrix[i,j] = Math.Round(Math.Sin(matrix[i, j]) + Math.Cos(matrix[i, j]),2);
                }
            }
            return matrix;
        }

    }
}
