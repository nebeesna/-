#include <iostream>
#include <vector>
#include <algorithm>
#include <ctime>
#include <chrono>
#include <thread>

using namespace std;
//Варіант 11
//Знайти скалярний добуток двох випадкових векторів.

//ВИСНОВОК
/*
* Загалом виконання задання в потоках завжди виконується швидше ніж в головному потоці
* при невеликих розмірах n(0 - 100) різниця у часі приблизно у декілька разів, у потоках 
* виконується швидше
* наприклад при n = 100, k = 100 час у головному 0.00153 мс а у потоках 0.0076 мс
* різниці між у часі при 2 чи 10 потоках майже немає
* при більших розмірностях теж виграють потоки, так само у декілька разів, в залежності 
* від к-сті потоків
* але якщо потоків "забагато" то ефективність трошки зменшується
* при n = 10 мільйонів, k = 100 час у головному 376 мс, а у потоках 56 мс
* але при k = 10 000 час у потоках стає приблизно 264 мс, 
* бо треба більше часу на створення такої к - сті потоків.
* 
*/
int res_paralel = 0;
struct  Data
{
    vector<int> & a;
    vector<int> & b;
    int start;
    int end;
};
void randomize_vector(vector<int>& _vector) {
    generate(_vector.begin(), _vector.end(), []() { return rand() % 10; });
}
int scalar_product(vector<int> a, vector<int> b) {
    int product = 0;
    for (int i = 0; i <= a.size() - 1; i++)
        product += (a[i]) * (b[i]);
    return product;

}
void scalar_product_paralel(Data d) {
    for (int i = d.start; i < d.end; i++)
        res_paralel += (d.a[i]) * (d.b[i]);
}
int main()
{
    srand(time(NULL));
    int size;
    cout << "Enter size of vectors:\n";
    cin >> size;
    int count_of_threads;
    cout << "Input count of threads:\n";
    cin >> count_of_threads;

    vector<int> vector_a(size, 0);
    vector<int> vector_b(size, 0);
    randomize_vector(vector_a);
    randomize_vector(vector_b);

    auto start_time = chrono::high_resolution_clock::now();
    int res = scalar_product(vector_a, vector_b);
    auto end_time = chrono::high_resolution_clock::now();

    cout << "------------------------------------------";
    cout << "\nTime in main thread: " << chrono::duration<double, milli>(end_time - start_time).count() << " ms";
    
    vector<thread> threads;
    Data d = { vector_a, vector_b, 0, 0};

    int rows_for_each = size / count_of_threads;
    int rows_for_last = rows_for_each + size % (count_of_threads);

    start_time = std::chrono::high_resolution_clock::now();

    for (int i = 0; i < count_of_threads; i++)
    {
        if ((i + 1) * rows_for_each + rows_for_last <= size)
        {
            d.start = i * rows_for_each;
            d.end = (i + 1) * rows_for_each;
            threads.push_back(thread(scalar_product_paralel, d));
            if(count_of_threads <= 10)
                cout << "\nRun in thread " << threads[i].get_id();
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
    for (int i = 0; i < count_of_threads; i++) {
        threads[i].join();
    }

    end_time = chrono::high_resolution_clock::now();

    cout << "\nElapsed time for " << count_of_threads << " threads: " << chrono::duration<double, std::milli>(end_time - start_time).count() << " ms";

    //system("pause");
    return 0;
}


