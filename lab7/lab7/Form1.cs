using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace lab7
{
    //ВАРІАНТ 11
    //На вхід всіх потоків передається спільний для всіх великий текстовий файл.
    //Потік будує вихідний файл із вхідного, заміняючи наперед задані окремі
    //символи якоюсь заданою непустою групою символів (тобто вихідний файл має
    //бути більшим за вхідний).
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox5.Text.Length < textBox6.Text.Length)
            {
                //ініціалізація вхідного файлика даними(числами)
                StringBuilder text = new StringBuilder();
                for (int i = 0; i < Int32.Parse(textBox2.Text); i++)
                {
                    text.Append(i + textBox5.Text);
                }
                using (StreamWriter writer = new StreamWriter(@"C:\code\c#\паралелки\lab7\lab7\start_file.txt"))
                {
                    for (int i = 0; i < Int32.Parse(textBox3.Text); i++)
                    {
                        writer.WriteLine(text.ToString());
                    }
                }
                string startPath = @"C:\code\c#\паралелки\lab7\lab7\start_file.txt";
                string resultPath = @"C:\code\c#\паралелки\lab7\lab7\result_file.txt";
                string resultParalelPath = @"C:\code\c#\паралелки\lab7\lab7\resultParalel_file.txt";

                string givenText = File.ReadAllText(startPath);

                //виконання в головному потоці
                var watch1 = Stopwatch.StartNew();
                string resultText = givenText.Replace(textBox5.Text, textBox6.Text);
                using (FileStream fs = File.Create(resultPath))
                {   }
                using (StreamWriter writer = new StreamWriter(resultPath))
                {
                    writer.WriteLine(resultText);
                }
                watch1.Stop();
                textBox1.Text = watch1.Elapsed.ToString();



                int countOfThreads = Convert.ToInt32(textBox4.Text);
                int countOfRows = Convert.ToInt32(textBox3.Text);
                int rowsForEach = countOfRows / countOfThreads;
                int rowsForLast = rowsForEach + countOfRows % countOfThreads;
               
                //виконання в паральних потоках
                var watch2 = Stopwatch.StartNew();
                Thread[] threads = new Thread[countOfThreads];
                Data data = new Data();
                data.text = new List<string>();
                foreach (string line in File.ReadLines(startPath))
                {
                    data.text.Add(line);
                }
                data.oldValue = textBox5.Text;
                data.newValue = textBox6.Text;
                using (FileStream fs = File.Create(resultParalelPath))
                { }
                using (StreamWriter writer = new StreamWriter(resultParalelPath))
                {
                    data.writer = writer;
                    
                    for (int i = 0; i < threads.Length; i++)
                    {
                        if ((i + 1) * rowsForEach + rowsForLast <= countOfRows)
                        {
                            threads[i] = new Thread(Action.Start);
                            data.start = i * rowsForEach;
                            data.end = (i + 1) * rowsForEach;
                            threads[i].Start(data);
                        }
                        else
                        {
                            threads[i] = new Thread(Action.Start);
                            data.start = countOfRows - rowsForLast;
                            data.end = countOfRows - 1;
                            threads[i].Start(data);
                        };
                    }
                    foreach (var t in threads)
                    {
                        t.Join();
                    }
                }
                watch2.Stop();
                textBox7.Text = watch2.Elapsed.ToString();
            }
            else throw new ArgumentException("Length of separator must be smaller then length of replaced separator");
        }
        //ВИСНОВОК
        
        /*
        При малій к-сті рядочків (до 100) швидше виконується завдання в головному 
        потоці (0.001с) ніж у мультипотоках
        більше витричається часу на створення самих потоків, і так важливо скільки
        самих потоків
        на більших файлах з 1000+ рядочків у рази швидше справляються мультипотоки
        0.001с у потоках і 0.003с у головному при 1000 рядках
        і неважливо скільки елементів у рядку, час приблизно однаковий
        для таких розмірностей оптимальна к-сть потоків приблизно 50
        більше/менше час стає більшим
        більше ніж 10 000 рядків так само виграють по часу мультипотоки
        в головному потоці 0.05с
        в мультипотоках приблизно 0.008с в кращих випадках, при оптимальній к-сті потоків
        в гірших 0.02с
         
         */
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
