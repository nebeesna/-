using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab5_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /*
        ВИСНОВОК:
        на невеликій к-сті вузлів(до 100) у головному потоці
        алгоритм працює приблизно за однаковий час, приблизно 0.0006с
        тим часом як розпаралелений алгоритм працює приблизно в 10 разів
        повільніше, причому на різній к-сті потоків, час приблизно 0.007с
        зі збільшенням к-сті вузлів ця різниця зменшується
        коли к-сть вузлів більше ~500, час виконання в головному потоці зростає
        на 1000 вузлів час виконання в гол. потоці 0.02с а в багатьох потоках
        0.06с, у 3 рази довше
        і чим більша к-сть вузлів, тим менша різниця
        на 10тис вузлів час виконання на головному потоці 1.86сек,
        на 2 потоках 3.206, і чим більше потоків тим швидше, 
        але завжди швидше виконується в 1 потоці

         */

        private void button1_Click(object sender, EventArgs e)
        {
            int verticesCount = Convert.ToInt32(textBox1.Text);
            int startVertice = Convert.ToInt32(textBox3.Text);
            DijkstraMethod.NotParallel first = new DijkstraMethod.NotParallel(verticesCount);

            var watch1 = Stopwatch.StartNew();
            first.Start(startVertice);
            watch1.Stop();
            textBox4.Text = watch1.Elapsed.ToString();

            int k = Convert.ToInt32(textBox2.Text);
            int verticesForEach = verticesCount / k;
            int verticesForLast = verticesForEach + verticesCount % k;

            var watch2 = Stopwatch.StartNew();
            List<int> vetrticles = new List<int>();
            for (int i = 0; i < k; i++)
            {
                if (i != k - 1)
                {
                    vetrticles.Add(i * verticesForEach);
                }
                else
                {
                    vetrticles.Add(verticesCount - verticesForLast);
                }
            }
            DijkstraMethod.Parallel.Constructor(first.matrix, startVertice);
            Parallel.ForEach(vetrticles, data =>
            {
                DijkstraMethod.Parallel.StartParallel(data, data + verticesForLast);
            });

            watch2.Stop();
            textBox5.Text = watch2.Elapsed.ToString();
        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
