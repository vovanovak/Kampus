#include <iostream>
#include <stdio.h>
#include <iomanip>
#include <math.h>

using namespace std;

const unsigned int rmax = 10, gmax = 12, bmax = 23;//Ограничение на ввод данных
const int n = 10;//Количество строк в таблице

				 //Структура с данными, которые входят в таблицу
struct rgb
{
	unsigned int r, g, b;
};

enum menu { avtomatically = 1, byhand = 2 };//Выбор в меню способа ввода (автоматически/вручную)

void creat_table(rgb[]);//Заполнить таблицу автоматически
void input_table(rgb[]);//Заполнить таблицу вручную
rgb find_minimum(rgb[]);//Найти минимум в каждом столбце таблицы
void output_table(rgb[]);//Вывести таблицу

						 //Главная функция-----------------------------------------------------------------------------------
int main()
{
	int method;
	bool check = 0;
	rgb table[n];//Таблица, которая содержит данные, входящие в строки

	setlocale(LC_CTYPE, "Russian");
	srand((unsigned int)time(NULL));

	cout << "Необходимо заполнить таблицу. Выберите способ:\n1.Автоматически\n2.Вручную" << endl;

	while (check == 0)
	{
		cin >> method;
		if (method == avtomatically)
		{
			creat_table(table);
			check = 1;
		}
		else if (method == byhand)
		{
			input_table(table);
			check = 1;
		}
		else cout << "Неверное значение! введите 1 или 2 для выбора." << endl;
	}

	output_table(table);
	system("pause");
}
//Заполнить таблицу автоматически-------------------------------------------------------------------
void creat_table(rgb table[])
{
	system("cls");
	for (int i = 0; i<n; i++)
	{
		table[i].r = rand() % rmax;
		table[i].g = rand() % gmax;
		table[i].b = rand() % bmax;
	}
	system("cls");
}
//Заполнить таблицу вручную-------------------------------------------------------------------------
void input_table(rgb table[])
{
	system("cls");
	bool check = 0;
	printf("Ограничения на ввод: r<%d g<%d b<%d\n\n", rmax, gmax, bmax);
	for (int i = 0; i<n; i++)
	{
		while (check == 0)
		{
			cout << "Введите значения для " << i + 1 << "-го рядка таблицы: " << endl;
			cout << "r="; cin >> table[i].r;
			cout << "g="; cin >> table[i].g;
			cout << "b="; cin >> table[i].b;
			if (table[i].r<rmax&&table[i].g<gmax&&table[i].b<bmax)
			{
				check = 1;
			}
			else cout << "Неверное значение! Учитывайте ограничения." << endl;
		}
		check = 0;
	}
	system("cls");
}
//Найти минимум в каждом столбце таблицы------------------------------------------------------------
rgb find_minimum(rgb table[])
{
	rgb minimum;
	for (int i = 0; i<n; i++)
	{
		minimum.r = min(minimum.r, table[i].r);
		minimum.g = min(minimum.g, table[i].g);
		minimum.b = min(minimum.b, table[i].b);
	}
	return minimum;
}
//Вывести таблицу-----------------------------------------------------------------------------------
void output_table(rgb table[])
{
	printf("Ограничения: r<%d g<%d b<%d\n\n", rmax, gmax, bmax);
	cout << "Ваша таблица:\n--------------------------\n№\tr\tg\tb\n--------------------------" << endl;
	for (int i = 0; i<n; i++)
	{
		printf("%d \t%d \t%d \t%d", i + 1, table[i].r, table[i].g, table[i].b);
		cout << endl;
	}
	rgb minimum = find_minimum(table);
	cout << "--------------------------" << endl;
	printf("min \t%d \t%d \t%d\n--------------------------\n", minimum.r, minimum.g, minimum.b);
}
