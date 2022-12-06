#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <ctime>
#include <chrono>
#include <thread>
#include <mutex>
using namespace std;
//Варіант 11	
//У файлі перераховані імена процесів/потоків для одночасного запуску 
//(аналог процедури із команд). 
//Процес/потік виконує певну роботу протягом заданого часу: 
//1) один просто циклить; 
//2) другий багаторазово зчитує час/дату і т.д. (не менше 7 своїх варіантів).	
//Порядок виводу: Послідовно з першого до останнього

//ВИСНОВОК
/*
* Загалом різниця у часі майже непомітна
* особливо враховуючи що у мультипотоці додатково витрачається час 
* для запису часу виконання кожного потоку
* якщо ж закоментити той запис часу то виконання в потоках стає швидшим
* і виходить що час виконання у потоках приблизно на 10-15% менший ніж у головному потоці
* але це у кращих випадках
* зазвичай до 10% і це майже непомітна різниця бо програма нескладна і 
* сама по собі виконується швидко
* наприклад приблизний час виконання у головному потоці 985 мс
* а у мультипотоках 927 мс
* як бачимо різниця невелика
* 
*/
void now_time();
void add();
void mult();
void waiting();
void random_arr();
void sub();
void string_from_nums();
void start(string path);
void check_names(string name);
void start_paralel(string path);

string path = "names.txt";
string res_path = "results.txt";
int main()
{
    auto start_time = chrono::high_resolution_clock::now();
    start(path);
    auto end_time = chrono::high_resolution_clock::now();

    cout << "\n\n------------------------------------------";
    cout << "\nTime in main thread: " << chrono::duration<double, milli>(end_time - start_time).count() << " ms\n";
    
    start_time = chrono::high_resolution_clock::now();
    start_paralel(path);
    end_time = chrono::high_resolution_clock::now();

    cout << "\n\n------------------------------------------";
    cout << "\nTime in multithreads: " << chrono::duration<double, milli>(end_time - start_time).count() << " ms\n";

}
void start(string path)
{
    fstream newfile;
    newfile.open(path, ios::in);
    if (newfile.is_open()) {
        string name;
        while (getline(newfile, name)) {
            check_names(name);
        }
        newfile.close();
    }
}
void start_paralel(string path)
{
    fstream newfile;
    vector<string> names;
    newfile.open(path, ios::in);
    if (newfile.is_open()) {
        string name;
        while (getline(newfile, name)) {
            names.push_back(name);
        }
        newfile.close();
    }
    int threads_count = names.size();
    vector<thread> threads(threads_count);
    auto start = chrono::high_resolution_clock::now();
    auto end = chrono::high_resolution_clock::now();
    ofstream res_file;
    res_file.open(res_path);
    for (int i = 0; i < names.size(); i++)
    {
        start = chrono::high_resolution_clock::now();
        threads[i] = thread(check_names, names[i]);
        threads[i].join();
        end = chrono::high_resolution_clock::now();
        res_file << "Thread["<<i<<"] run in " << chrono::duration<double, milli>(end - start).count() << " ms" << endl;
    }
    res_file.close();
}
/* Без запису результатів у результуючий файлик
void start_paralel(string path)
{
    fstream newfile;
    vector<string> names;
    newfile.open(path, ios::in);
    if (newfile.is_open()) {
        string name;
        while (getline(newfile, name)) {
            names.push_back(name);
        }
        newfile.close();
    }
    int threads_count = names.size();
    vector<thread> threads(threads_count);
    for (int i = 0; i < names.size(); i++)
    {
        threads[i] = thread(check_names, names[i]);
        threads[i].join();
    }
}
*/
void check_names(string name) {
    if (name == "now_time")
    {
        now_time();
    }
    else if(name == "add")
    {
        add();
    }
    else if(name == "mult")
    {
        mult();
    }
    else if(name == "waiting")
    {
        waiting();
    }
    else if(name == "random_arr")
    {
        random_arr();
    }
    else if(name == "sub")
    {
        sub();
    }
    else if(name == "string_from_nums")
    {
        string_from_nums();
    }
    else { waiting(); }
}
void now_time() {
    mutex m;
    m.lock();
    cout << "\nnow_time() run in thread " << this_thread::get_id();
    m.unlock();
    for (int i = 0; i < 1000; i++)
    {
        auto start = std::chrono::system_clock::now();
    }
}
void add() {
    mutex m;
    m.lock();
    cout << "\nadd() run in thread " << this_thread::get_id();
    m.unlock();
    srand(time(NULL));
    int sum = 0;
    for (int i = 0; i < 1000; i++)
    {
        sum += rand() % 10;
    }
}
void mult() {
    mutex m;
    m.lock();
    cout << "\nmult() run in thread " << this_thread::get_id();
    m.unlock();
    srand(time(NULL));
    int sum = 0;
    for (int i = 0; i < 1000; i++)
    {
        sum += rand() % 10 * rand() % 10;
    }
}
void waiting(){
    mutex m;
    m.lock();
    cout << "\nwaiting() run in thread " << this_thread::get_id();
    m.unlock();
    srand(time(NULL));
    this_thread::sleep_for(std::chrono::milliseconds(rand()%1000));
}
void random_arr() {
    mutex m;
    m.lock();
    cout << "\nrandom_arr() run in thread " << this_thread::get_id();
    m.unlock();
    srand(time(NULL));
    int arr[1000];
    for (int i = 0; i < 1000; i++)
    {
        arr[i] = rand() % 10;
    }
}
void sub() {
    mutex m;
    m.lock();
    cout << "\nsub() run in thread " << this_thread::get_id();
    m.unlock();
    int res = INT_MAX;
    for (int i = 0; i < 1000; i++)
    {
        res -= rand() % 10;
    }
}
void string_from_nums() {
    mutex m;
    m.lock();
    cout << "\nstring_from_nums() run in thread " << this_thread::get_id();
    m.unlock();
    string res;
    for (int i = 0; i < 1000; i++)
    {
        res += i%10;
    }
}

