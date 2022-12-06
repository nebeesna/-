using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lab4
{
    //Варіант 11
    //метод релаксації
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine("\n-----Method of relaxation-----\n");
            Console.Write("Enter number of equations: ");
            int n = Int32.Parse(Console.ReadLine());
            Console.Write("Enter number of threads: ");
            int k = Int32.Parse(Console.ReadLine());

            Matrix matrix = new Matrix(n, n);
            double[] b_ = new double[n];
            for (int i = 0; i < n; i++)
            {
                b_[i] = random.Next(-100, 100);
            }
            var forConstructor = new Data() { m = matrix, b = b_ };
            WaitCallback oneThread = new WaitCallback(Method.RelaxationMethod.Start);
            var experiment1 = System.Diagnostics.Stopwatch.StartNew();
            ThreadPool.QueueUserWorkItem(oneThread, forConstructor);
            experiment1.Stop();
            Console.WriteLine($"\nOne thread(Main thread): {experiment1.Elapsed}");


            var forStart = new Data() { start = 0, end = 0 };
            int rowsForEach = n / (k - 1);
            int rowsForLast = rowsForEach + n % (k - 1);
            var experiment2 = System.Diagnostics.Stopwatch.StartNew();
            Data forCtor = new Data() { start = 0, end = 0, b = b_, m = matrix };
            WaitCallback ctor = new WaitCallback(Method.RelaxationMethodParalel.Constructor);
            WaitCallback method = new WaitCallback(Method.RelaxationMethodParalel.Start);
            ThreadPool.QueueUserWorkItem(ctor, forCtor);
            for (int i = 0; i < k - 1; i++)
            {
                if ((i + 1) * rowsForEach + rowsForLast <= n)
                {
                    forCtor.start = i * rowsForEach;
                    forCtor.end = (i + 1) * rowsForEach;
                    ThreadPool.QueueUserWorkItem(method, forCtor);
                }
                else
                {
                    forCtor.start = n - rowsForLast;
                    forCtor.end = n;
                    ThreadPool.QueueUserWorkItem(method, forCtor);
                }
            }
            experiment2.Stop();
            Console.WriteLine($"\n{k} threads: {experiment2.Elapsed}");
        }

        /*
         ВИСНОВОК:
        метод релаксації в головному потоці справляється швидко, прямопропорційно 
        збільшується час з збільшенням розмірів масивів
        при n = 10, час 0.0000019, n = 100, час 0.0000054
        n = 1000, час = 0.000065, n = 10000, час = 0.00023

        при розпаралененні, чим бліьше потоків тим більшший час, теж приблизно пропорційний к-сті потоків
        при невеликих розмірах, до n = 500, час 2-10 потоків приблизно такий ж як і в головному потоці
        десь менший вдвічі, десь більший, але то мізерний час
        якщо на таких розмірах паралелити на багато потоків, що більший ніж розмір матриці, час на 
        створення, запуск потоків є більшим ніж сам час розрахунків, і білшим ніж час в головному потоці
        тому на таких розмірах оптимально паралелити до ~20 потоків

        на великих масивах, n = 1000+, розпараленення до ~500 потоків по часу справляється краще ніж головний потік
        коли уже 1000 потоків приблизно, швидшим є головний потік
         
         */
    }
}
