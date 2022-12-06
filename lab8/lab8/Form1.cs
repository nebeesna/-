using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab8
{
    //Варіант 11
    /*
     * На вході всіх потоків є два файли із числовими матрицями (не менше 500 елементів). 
     * Потоки (кратності 3) будують суму, різницю та добуток цих двох матриць у свій вихідний файл.	
     * Порядок виводу:
     * n/2 -потік, n/2-1-потік, …, перший потік, n/2+1-потік, n/2+2-потік,…, n-потік
     */
    public partial class Form1 : Form
    {
        private static string[] matriсesPathes = new string[2];
        private static string resultMatriсesPath;
        //private static List<string> matrix1 = new List<string>();
        //private static List<string> matrix2 = new List<string>();
        private static List<List<int>> matrix1 = new List<List<int>>();
        private static List<List<int>> matrix2 = new List<List<int>>();
        private static int countOfThreads;
        private static int size;
        private static Random random = new Random();
        public Form1()
        {
            InitializeComponent();
            matriсesPathes[0] = @"C:\code\c#\паралелки\lab8\lab8\matrix1.txt";
            matriсesPathes[1] = @"C:\code\c#\паралелки\lab8\lab8\matrix2.txt";
            resultMatriсesPath = @"C:\code\c#\паралелки\lab8\lab8\result.txt";
        }
        //Create matrixes
        private void button1_Click(object sender, EventArgs e)
        {
            //По умові мін 500 елементів, але я написала min value 20
            // бо 20 звучить зрозуміліше і вийде матриця з 400 елементів, думаю не критично
            
            //Створюємо матриці вказаного розміру у наперед заданих шляхах
            size = Int32.Parse(textBox1.Text);
            if (size >= 20)
            {
                CreateMatrix(matriсesPathes[0]);
                CreateMatrix(matriсesPathes[1]);
                label6.Text = "Matrixes are generated!";
                //записуємо дані з файликів для зручнішої роботи з тими даними
                foreach (string line in File.ReadLines(matriсesPathes[0]))
                {
                    matrix1.Add(Array.ConvertAll(line.Split(), int.Parse).ToList());
                }
                foreach (string line in File.ReadLines(matriсesPathes[1]))
                {
                    matrix2.Add(Array.ConvertAll(line.Split(), int.Parse).ToList());

                }
            }
            else throw new ArgumentException("Size must be greater than or equal to 20!");
        }

        //Start 
        private void button2_Click(object sender, EventArgs e)
        {

            //результат для виводу що потік працював, бо виводити весь результуючий
            //рядок немає сенсу
            //створюємо файл для результатів
            using (FileStream fs = File.Create(resultMatriсesPath))
            { }
            countOfThreads = Int32.Parse(textBox2.Text);
            int rowsForEach = size / countOfThreads/3;
            int rowsForLast = rowsForEach + size % (countOfThreads/3);

            Thread[] threads = new Thread[countOfThreads];
            int[] results = new int[countOfThreads];
            Data d = new Data();
            d.start = 0;
            d.end = 0;
            for (int i = 0; i < countOfThreads/3; i++)
            {
                if ((i + 1) * rowsForEach + rowsForLast <= size)
                {
                    d.start = i * rowsForEach;
                    d.end = (i + 1) * rowsForEach; 
                    threads[i] = new Thread(() => results[i] = MatricesAdditionParallel(d));
                }
                else
                {
                    d.start = size - rowsForLast;
                    d.end = size - 1;
                    threads[i] = new Thread(() => results[i] = MatricesAdditionParallel(d));
                };
            }
            for (int i = countOfThreads / 3; i < countOfThreads - countOfThreads / 3; i++)
            {
                if ((i + 1) * rowsForEach + rowsForLast <= size)
                {
                    d.start = i * rowsForEach;
                    d.end = (i + 1) * rowsForEach; 
                    threads[i] = new Thread(() => results[i] = MatricesSubtractionParallel(d));
                }
                else
                {
                    d.start = size - rowsForLast;
                    d.end = size - 1;
                    threads[i] = new Thread(() => results[i] = MatricesSubtractionParallel(d));
                };
            }
            for (int i = countOfThreads - countOfThreads / 3; i < countOfThreads; i++)
            {
                if ((i + 1) * rowsForEach + rowsForLast <= size)
                {
                    d.start = i * rowsForEach;
                    d.end = (i + 1) * rowsForEach; 
                    threads[i] = new Thread(() => results[i] = MatricesMultiplicationParallel(d));
                }
                else
                {
                    d.start = size - rowsForLast;
                    d.end = size - 1;
                    threads[i] = new Thread(() => results[i] = MatricesMultiplicationParallel(d));
                };
            }
            foreach(var t in threads)
            {
                if(t.ManagedThreadId % 3 == 0)
                {
                    t.Start();
                }
            }
            foreach(var t in threads)
            {
                t.Join();
            }
            for (int i = countOfThreads/2; i <= 0; i--)
            {
                richTextBox1.AppendText($"thread {threads[i].ManagedThreadId}: {results[i]}");
            }
            for (int i = countOfThreads/2 +1; i < countOfThreads; i++)
            {
                richTextBox1.AppendText($"thread {threads[i].ManagedThreadId}: {results[i]}");
            }


        }
        public static void CreateMatrix(string path)
        {
            StringBuilder data = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if(j!= size - 1)
                    {
                        data.Append(random.Next(0,100) + "\t");
                    }
                    else
                    {
                        data.Append(random.Next(0, 100));
                    }
                }
                if(i != size - 1)
                {
                    data.Append("\n");
                }
            }
            //створюємо файл куди записувати
            using (FileStream fs = File.Create(path))
            { }
            //записуємо у створений файлик
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(data.ToString());
            }
        }
        private static int MatricesAdditionParallel(Object o)
        {
            Data data = (Data) o;
            int start = data.start;
            int end = data.end;
            StringBuilder res = new StringBuilder();
            int sum;
            int intermediateRes = 0;
            using (StreamWriter writer = File.AppendText(resultMatriсesPath))
            {
                writer.WriteLine("\nMatrices Addition Result:\n");
                for (int i = start; i < end; i++)
                {
                    res.Clear();
                    for (int j = 0; j < size; j++)
                    {
                        sum = matrix1[i][j] + matrix2[i][j];
                        intermediateRes += sum;
                        res.Append(sum + "\t");
                    }
                    writer.WriteLine(res.ToString());
                }
            }
            return intermediateRes;
        }
        private static int MatricesSubtractionParallel(Object o)
        {
            Data data = (Data)o;
            int start = data.start;
            int end = data.end;
            StringBuilder res = new StringBuilder();
            int sub;
            int intermediateRes = 0;
            using (StreamWriter writer = File.AppendText(resultMatriсesPath))
            {
                writer.WriteLine("\nMatrices Subtraction Result:\n");
                for (int i = start; i < end; i++)
                {
                    res.Clear();
                    for (int j = 0; j < size; j++)
                    {
                        sub = matrix1[i][j] - matrix2[i][j];
                        intermediateRes += sub;
                        res.Append(sub + "\t");
                    }
                    writer.WriteLine(res.ToString());
                }
            }
            return intermediateRes;
        }
        private static int MatricesMultiplicationParallel(Object o)
        {
            Data data = (Data)o;
            int start = data.start;
            int end = data.end;
            StringBuilder res = new StringBuilder();
            int mult;
            int intermediateRes = 0;
            using (StreamWriter writer = File.AppendText(resultMatriсesPath))
            {
                writer.WriteLine("\nMatrices Multiplication Result:\n");
                for (int i = start; i < end; i++)
                {
                    res.Clear();
                    for (int j = 0; j < size; j++)
                    {
                        mult = 0;
                        for (int k = 0; k < size; k++)
                        {
                            mult += matrix1[i][k] * matrix2[k][j];
                        }
                        intermediateRes += mult;
                        res.Append(mult + "\t");
                    }
                    writer.WriteLine(res.ToString());
                }
            }
            return intermediateRes;
        } 
        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
