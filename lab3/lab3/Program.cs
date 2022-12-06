using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lab3
{
    //Варіант 11	
    //Метод квадратного кореня	та  Метод релаксації
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine("\n-----Method of square roots-----\n");
            Console.Write("Enter number of equations: ");
            int n = Int32.Parse(Console.ReadLine());
            Console.Write("Enter number of threads: ");
            int k = Int32.Parse(Console.ReadLine());

            Matrix matrix = new Matrix(n, n);
            matrix.makeSymmetrical();
            double[] b_ = new double[n];
            for (int i = 0; i < n; i++)
            {
                b_[i] = random.Next(-100, 100);
            }

            Thread oneThread = new Thread(Method.SquareRootMethod.StartMethod);
            Data data = new Data { m = matrix, b = b_};
            var experiment1 = System.Diagnostics.Stopwatch.StartNew();
            oneThread.Start(data);
            oneThread.Join();
            experiment1.Stop();
            Console.WriteLine($"\nOne thread(Main thread): {experiment1.Elapsed}");


            Console.WriteLine();
            Thread[] threads = new Thread[k/2];
            Thread[] threadsForX = new Thread[k - k/2]; //(Method.SquareRootMethodParalel.findXParalel);
            DataForParalel dataForSecond = new DataForParalel();
            int rowsForEachThread = n / (threads.Length);
            int rowsForLastThread = rowsForEachThread + n % (threads.Length);
            int rowsForEachThreadForX = n / (threadsForX.Length);
            int rowsForLastThreadForX = rowsForEachThread + n % (threadsForX.Length);
            var experiment2 = System.Diagnostics.Stopwatch.StartNew();
            Method.SquareRootMethodParalel.Constructor(matrix, b_);
            for (int i = 0; i < threads.Length; i++)
            {
                if ((i + 1) * rowsForEachThread + rowsForLastThread <= n)
                {
                    threads[i] = new Thread(Method.SquareRootMethodParalel.startMethodByParalel);
                    dataForSecond.start = i * rowsForEachThread;
                    dataForSecond.end = (i+1) * rowsForEachThread -1;
                    threads[i].Start(dataForSecond);
                }
                else
                {
                    threads[i] = new Thread(Method.SquareRootMethodParalel.startMethodByParalel);
                    dataForSecond.start = n - rowsForLastThread;
                    dataForSecond.end = n - 1;
                    threads[i].Start(dataForSecond);
                };
            }
            foreach (var t in threads)
            {
                t.Join();
            }
            for (int i = 0; i < threadsForX.Length; i++)
            {
                if ((i + 1) * rowsForEachThreadForX + rowsForLastThreadForX <= n)
                {
                    threadsForX[i] = new Thread(Method.SquareRootMethodParalel.findXParalel);
                    dataForSecond.start = i * rowsForEachThreadForX;
                    dataForSecond.end = (i + 1) * rowsForEachThreadForX - 1;
                    threadsForX[i].Start(dataForSecond);
                }
                else
                {
                    threadsForX[i] = new Thread(Method.SquareRootMethodParalel.findXParalel);
                    dataForSecond.start = n - rowsForLastThreadForX;
                    dataForSecond.end = n - 1;
                    threadsForX[i].Start(dataForSecond);
                };
            }
            foreach (var t in threadsForX)
            {
                t.Join();
            }
            experiment2.Stop();
            Console.WriteLine($"\n{k} threads: {experiment2.Elapsed}");
        }
        /*
         ВИСНОВКИ
        для невеликих матриць до ~50 рівнянь виконання на потоках трошки програє, на декілька десятих мілісеекунд
        і неважливо скільки потоків 2 чи 100
        для матриць розміру ~100х100 при певному числі потоків виграють саме вони
        2 потоки працюють так само як і 1, бо за моїм методом розпараленення всеодно виходить що перший та другий ідуть 
        послідовно і сенсу в тому немає
        найкращою к-стю потоків є ~10, час 0.0032с, якщо збільшуючи/зменшуючи к-сть потоків час є більшим
        тобто від 2 до 10 час змешується, 10+ потоків час росте
        на ~1000 рівнянь виграють потоки, якщо їх не 1000)
        найкращий час коливається в межах 20-27 секунд! для 10-100 потоків.
        найкращим було використання 20 потоків
        тоді як в 1 потоці, 2 або 1000 час ~1хв
        більші матриці просто надто важко було тестувати бо мій ноут трохи слабий
        10 000 рівнянь
        більше 7 хв рахувало головний потік і не дорахувало, бо чекати довше я не хотіла)
        заглянула в task manager. ці обрахунки використовували макс 30% потужності процесору і 2.3гб оперативки
        100 потоків рахувало так само довго і я не дочекаласт закінчення
        але, використовувало практично 100% cpu і стільки ж пам'яті
                   
         */
    }
}
