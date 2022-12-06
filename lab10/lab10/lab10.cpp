#include <iostream>
#include <cmath>
#include <vector>
#include <algorithm>
#include <chrono>
#include <future>
#include <queue>
#include <thread>
using namespace std;
//Варіант 11
//Точний метод Крамера

//ВИСНОВОК:
/*
* метод крамера дуже ресурсозатратний та має велику складність
* лише самі визначники треба шукати n + 1 раз
* а у нього складність О(n!) 
* тому більше n == 10 я не можу запустити бо просто коп'ютер не справляється
* n == 2, k == 2    час у головному потоці 0.964 мс, у потоках 1.268мс
* n == 3, k == 2    час у головному потоці 2.254 мс, у потоках 2.142мс
* n == 3, k == 3    час у головному потоці 2.254 мс, у потоках 2.067мс
* n == 5, k == 2    час у головному потоці 31.728 мс, у потоках 26.769мс
* n == 5, k == 4    час у головному потоці 32.629 мс, у потоках 31.561мс
* загалом, виконання завдання у потоках майже непомітно швидше за виконня у головному потоці
* якщо потоків "забагато" то ефективність трошки зменшується
* при збільшенні к-сті потоків ефективність не дуже зростає, збільшується час на створення самих тредів
* Наприклад:
* n == 8, k == 2    час у головному потоці 9045 мс, у потоках 8001мс
* n == 8, k == 5    час у головному потоці 8932 мс, у потоках 9078мс
* тут видно що різниця у часі між 2 та 5 потоками є, бо 5 потоків довше створювати
* 
*/
int determinant(vector<vector<int>> matrix, int n);
vector<int> cramers_rule(vector<vector<int>> a, vector<int> b, int d);
vector<int> cramers_rule_paralel(vector<vector<int>> a, vector<int> b, int d, int st, int end);


void randomize_row(vector<int>& row)
{
    generate(row.begin(), row.end(), []() { return rand() % 10; });
}

void randomize_matrix(vector<vector<int>>& matrix)
{
    for_each(matrix.begin(), matrix.end(), randomize_row);
}
int main()
{
    int size;
    cout << "Enter count of equations: ";
    cin >> size;
    int count_of_threads;
    cout << "Input count of threads: ";
    cin >> count_of_threads;
    vector<vector<int>> a (size, vector<int>(size));
    vector<int> b(size);
    randomize_matrix(a);
    randomize_row(b);
    int det = determinant(a, size);
    if (det != 0) {
        auto start = chrono::high_resolution_clock::now();
        cramers_rule(a,b,det);
        auto end = chrono::high_resolution_clock::now();
        cout << "------------------------------------";
        cout << "\nTime in main thread: " << chrono::duration<double, milli>(end - start).count() << " ms"<< endl;

        start = chrono::high_resolution_clock::now();
        queue <future<int>> que;
        
        int rows_for_each = size / count_of_threads;
        int rows_for_last = rows_for_each + size % (count_of_threads);

        for (int i = 0; i < count_of_threads; i++)
        {
            if ((i + 1) * rows_for_each + rows_for_last <= size)
            {
                d.start = i * rows_for_each;
                d.end = (i + 1) * rows_for_each;
                que.push(async(launch::async, ))
            }
            else
            {
                d.start = size - rows_for_last;
                d.end = size;
                threads.push_back(thread(scalar_product_paralel, d));
                if (count_of_threads <= 10)
                    cout << "\nRun in thread " << threads[i].get_id();
            };
        }

        end = chrono::high_resolution_clock::now();
        cout << "\nTime in " << count_of_threads<<  " threads: " << chrono::duration<double, std::milli>(end - start).count() << " ms";
    }
    else throw new exception("Determinant == 0. Try run program again.");

}
int determinant(vector<vector<int>> matrix, int n) {
   int det = 0;
   vector<vector<int>> submatrix(10, vector<int>(10));
   if (n == 2)
   return ((matrix[0][0] * matrix[1][1]) - (matrix[1][0] * matrix[0][1]));
   else {
      for (int x = 0; x < n; x++) {
         int subi = 0;
         for (int i = 1; i < n; i++) {
            int subj = 0;
            for (int j = 0; j < n; j++) {
               if (j == x)
               continue;
               submatrix[subi][subj] = matrix[i][j];
               subj++;
            }
            subi++;
         }
         det = det + (pow(-1, x) * matrix[0][x] * determinant( submatrix, n - 1 ));
      }
   }
   return det;
}
vector<int> cramers_rule(vector<vector<int>> a, vector<int> b, int d) {

    vector<int> x(b.size());
    vector<vector<int>> matrix(a);
    cout << "Work in " << this_thread::get_id() << " thread" << endl;
    for (int i = 0; i < x.size(); i++)
    {
        matrix = a;
        for (int j = 0; j < matrix.size(); j++)
        {
            matrix[j][i] = b[j];
        }
        x[i] = determinant(matrix, matrix.size()) / d;
    }
    return x;
}
vector<int> cramers_rule_paralel(vector<vector<int>> a, vector<int> b, int d, int st, int end){

    vector<int> x(b.size());
    vector<vector<int>> matrix(a);
    cout << "Work in " << this_thread::get_id() << " thread" << endl;
    for (int i = st; i < end; i++)
    {
        matrix = a;
        for (int j = 0; j < matrix.size(); j++)
        {
            matrix[j][i] = b[j];
        }
        x[i] = determinant(matrix, matrix.size()) / d;
    }
    return x;
}