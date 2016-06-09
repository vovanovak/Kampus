#include <iostream>
#include <stdio.h>
#include <iomanip>
#include <math.h>

using namespace std;

const unsigned int rmax = 10, gmax = 12, bmax = 23;//����������� �� ���� ������
const int n = 10;//���������� ����� � �������

				 //��������� � �������, ������� ������ � �������
struct rgb
{
	unsigned int r, g, b;
};

enum menu { avtomatically = 1, byhand = 2 };//����� � ���� ������� ����� (�������������/�������)

void creat_table(rgb[]);//��������� ������� �������������
void input_table(rgb[]);//��������� ������� �������
rgb find_minimum(rgb[]);//����� ������� � ������ ������� �������
void output_table(rgb[]);//������� �������

						 //������� �������-----------------------------------------------------------------------------------
int main()
{
	int method;
	bool check = 0;
	rgb table[n];//�������, ������� �������� ������, �������� � ������

	setlocale(LC_CTYPE, "Russian");
	srand((unsigned int)time(NULL));

	cout << "���������� ��������� �������. �������� ������:\n1.�������������\n2.�������" << endl;

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
		else cout << "�������� ��������! ������� 1 ��� 2 ��� ������." << endl;
	}

	output_table(table);
	system("pause");
}
//��������� ������� �������������-------------------------------------------------------------------
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
//��������� ������� �������-------------------------------------------------------------------------
void input_table(rgb table[])
{
	system("cls");
	bool check = 0;
	printf("����������� �� ����: r<%d g<%d b<%d\n\n", rmax, gmax, bmax);
	for (int i = 0; i<n; i++)
	{
		while (check == 0)
		{
			cout << "������� �������� ��� " << i + 1 << "-�� ����� �������: " << endl;
			cout << "r="; cin >> table[i].r;
			cout << "g="; cin >> table[i].g;
			cout << "b="; cin >> table[i].b;
			if (table[i].r<rmax&&table[i].g<gmax&&table[i].b<bmax)
			{
				check = 1;
			}
			else cout << "�������� ��������! ���������� �����������." << endl;
		}
		check = 0;
	}
	system("cls");
}
//����� ������� � ������ ������� �������------------------------------------------------------------
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
//������� �������-----------------------------------------------------------------------------------
void output_table(rgb table[])
{
	printf("�����������: r<%d g<%d b<%d\n\n", rmax, gmax, bmax);
	cout << "���� �������:\n--------------------------\n�\tr\tg\tb\n--------------------------" << endl;
	for (int i = 0; i<n; i++)
	{
		printf("%d \t%d \t%d \t%d", i + 1, table[i].r, table[i].g, table[i].b);
		cout << endl;
	}
	rgb minimum = find_minimum(table);
	cout << "--------------------------" << endl;
	printf("min \t%d \t%d \t%d\n--------------------------\n", minimum.r, minimum.g, minimum.b);
}
