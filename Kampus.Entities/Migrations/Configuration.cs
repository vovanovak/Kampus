using System.Collections.Generic;

namespace Kampus.Entities.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Kampus.Entities.KampusContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Kampus.Entities.KampusContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var userRoles = new List<UserRole>()
            {
                new UserRole() { Name = "User" },
                new UserRole() { Name = "Admin"}
            };
            userRoles.RemoveAll(r => r.Id > 0);
            userRoles.ForEach(r => context.UserRoles.AddOrUpdate(r));

            context.SaveChanges();

            var categories = new List<TaskCategory> 
            { 
                new TaskCategory  {Name = "�������������", Description = "������� ���� �������������"},
                new TaskCategory  {Name = "�����������", Description = "������� �� ������ ������ ����������� �� ������������"},
                new TaskCategory  {Name = "�����������", Description = "������� ��������� ������ �� ���� ������� ����� �������"},
                new TaskCategory  {Name = "����������", Description = "������� �� ������ ��������� � ����������"},
                new TaskCategory  {Name = "��������", Description = "������� ��������� � ��������"},
                new TaskCategory  {Name = "�������������", Description = "������� ���������, �� ���������� ������������ �� ����������� ������������ ����� � �����"},
                new TaskCategory  {Name = "��������� ���������", Description = "������� ��������� ������������ ��������"},
                new TaskCategory  {Name = "����", Description = "������� �� ���� ���������, �� �� ������ � �������� �������"}
                
            };
            categories.ForEach(c => context.TaskCategories.AddOrUpdate(c));

            context.SaveChanges();

            var subcategories = new List<TaskSubcat>
            {
               new TaskSubcat {Name = "C", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� '�'"},
               new TaskSubcat {Name = "C++", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� '�++'"},
               new TaskSubcat {Name = "C#", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� '�#'"},
               new TaskSubcat {Name = "Pascal", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Pascal'"},
               new TaskSubcat {Name = "Objective C", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Objective C'"},
               new TaskSubcat {Name = "Visual Basic", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Visual Basic'"},
               new TaskSubcat {Name = "T-SQL", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'T-SQL'"},
               new TaskSubcat {Name = "Java", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Java'"},
               new TaskSubcat {Name = "JavaScript", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Java Script'"},
               new TaskSubcat {Name = "Delphi", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Delphi'"},
               new TaskSubcat {Name = "PHP", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'PHP'"},
               new TaskSubcat {Name = "HTML+CSS", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ��� ������� 'HTML+CSS'"},
               new TaskSubcat {Name = "1C", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� '1C'"},
               new TaskSubcat {Name = "Clips", TaskCategoryId = 1, Description = "�������� �� �������, �� ���������� ���� ������������� 'Clips'"},
               new TaskSubcat {Name = "��������-����������� ����������� (���)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� '��������-����������� �����������'"},
               new TaskSubcat {Name = "����������� �������� (��)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� '����������� ��������'"},
               new TaskSubcat {Name = "�������� ������ ����������� ���������� ������� (�����)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� '�������� ������ ����������� ���������� �������'"},
               new TaskSubcat {Name = "����������� ��������� ����������� (���)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� '�������� ������ ����������� ������������� ������'"},
               new TaskSubcat {Name = "����������� ��������� �����������. ������������ ����� (�����)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� '�������� ������ ����������� ������������� ������. ������������ �����'"},
               new TaskSubcat {Name = "������� ������� ��������� ����� (����)", TaskCategoryId = 2, Description = "�������� �� �������, �� ���������� ���������� 'C������ ������� ��������� �����'"},
               new TaskSubcat {Name = "������������� ������������� ������. ������ ������������� ������", TaskCategoryId = 3, Description = "�������� �� �������, �� ���������� ���������� '������������� ������������� ������. ������ ������������� ������'"},
               new TaskSubcat {Name = "������������� ������������� ������. ��������� �������", TaskCategoryId = 3, Description = "�������� �� �������, �� ���������� ���������� '������������� ������������� ������. ��������� �������'"},
               new TaskSubcat {Name = "������������� ������������� ������. ����'����� �����", TaskCategoryId = 3, Description = "�������� �� �������, �� ���������� ���������� '������������� ������������� ������. ���� ����� �����'"},
               new TaskSubcat {Name = "�����������", TaskCategoryId = 3, Description = "�������� �� �������, �� ���������� ���������� '�����������'"},
               new TaskSubcat {Name = "���������� ��� ���������", TaskCategoryId = 4, Description = "�������� �� �������, �� ���������� ���������� '���������� ��� ���������'"},
               new TaskSubcat {Name = "���������� ��� ��������� (����� ��������� �� ����������� ����������)", TaskCategoryId = 4, Description = "�������� �� �������, �� ���������� ���������� '���������� ��� ��������� (����� ��������� �� ����������� ����������)'"},
               new TaskSubcat {Name = "����������", TaskCategoryId = 4, Description = "�������� �� �������, �� ���������� ���������� '����������'"},
               new TaskSubcat {Name = "����� ���������� ������� (���)", TaskCategoryId = 4, Description = "�������� �� �������, �� ���������� ���������� '����� ���������� ������� (���)'"},
               new TaskSubcat {Name = "�������", TaskCategoryId =  4, Description = "�������� �� �������, �� ���������� ���������� '�������'"},
               new TaskSubcat {Name = "��������", TaskCategoryId = 4, Description = "�������� �� �������, �� ���������� ���������� '��������'"},
               new TaskSubcat {Name = "��������� ������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '��������� ������'"},
               new TaskSubcat {Name = "�������������� ����", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '�������������� ����'"},
               new TaskSubcat {Name = "����� �� ������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '����� �� ������'"},
               new TaskSubcat {Name = "�������� ����������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '�������� ����������'"},
               new TaskSubcat {Name = "������ �������� �� ��������� �����", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '������ �������� �� ��������� �����'"},
               new TaskSubcat {Name = "�������������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '�������������'"},
               new TaskSubcat {Name = "̳�����������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '̳�����������'"},
               new TaskSubcat {Name = "���������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '���������'"},
               new TaskSubcat {Name = "����������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '����������'"},
               new TaskSubcat {Name = "̳�������� ��������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '̳�������� ��������'"},
               new TaskSubcat {Name = "����������� ��������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '����������� ��������'"},
               new TaskSubcat {Name = "�������� �������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '�������� �������'"},
               new TaskSubcat {Name = "Գ�����", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� 'Գ�����'"},
               new TaskSubcat {Name = "���������� ��������", TaskCategoryId = 5, Description = "�������� �� �������, �� ���������� ���������� '���������� ��������'"},
               new TaskSubcat {Name = "������ ��������������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "������ ��������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ��������� ����� ������'"},
               new TaskSubcat {Name = "������ ����� ��� ���������� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ����� ��� ���������� ������'"},
               new TaskSubcat {Name = "������ ����������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ����������� ����� ������'"},
               new TaskSubcat {Name = "������ ������� ��������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ������� ��������� ����� ������'"},
               new TaskSubcat {Name = "������ ����������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ������������ ����� ������'"},
               new TaskSubcat {Name = "������ ��������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ��������� ����� ������'"},
               new TaskSubcat {Name = "������ ��������� �����", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ��������� �����'"},
               new TaskSubcat {Name = "������ ������-�������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ������-�������� ����� ������'"},
               new TaskSubcat {Name = "������ ��������������� ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "������ ������������ ����� ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '������ ������������ ����� ������'"},
               new TaskSubcat {Name = "��� � �������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '��� � �������'"},
               new TaskSubcat {Name = "�����������, ���������� �� ���� ������������ ������", TaskCategoryId = 6, Description = "�������� �� �������, �� ���������� ���������� '�����������, ���������� �� ���� ������������ ������'"},
               new TaskSubcat {Name = "�������� ����", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "���������� �������� �������� ����", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "��������� ���� �� ���������� ������������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "��������� ����", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������ ��������������� ����� ������'"},
               new TaskSubcat {Name = "������������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������������'"},
               new TaskSubcat {Name = "����� �� ��������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '����� �� �������'"},
               new TaskSubcat {Name = "������ ������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '������ ������'"},
               new TaskSubcat {Name = "�������� ������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '�������� ������'"},
               new TaskSubcat {Name = "��������� �� ���������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '��������� �� ���������'"},
               new TaskSubcat {Name = "Գ�������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� 'Գ�������'"},
               new TaskSubcat {Name = "��������� ���������", TaskCategoryId = 7, Description = "�������� �� �������, �� ���������� ���������� '��������� ���������'"},
               new TaskSubcat {Name = "���������", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� '���������'"},
               new TaskSubcat {Name = "������������� �����", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� '������������� �����'"},
               new TaskSubcat {Name = "Գ����� ���������", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� 'Գ����� ���������'"},
               new TaskSubcat {Name = "Գ����", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� 'Գ����'"},
               new TaskSubcat {Name = "������", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� '������'"},
               new TaskSubcat {Name = "ճ��", TaskCategoryId = 8, Description = "�������� �� �������, �� ���������� ���������� 'ճ��'"}

            };

            subcategories.ForEach(c => context.TaskSubcats.AddOrUpdate(c));

            context.SaveChanges();

            var cities = new List<City>()
            {
                new City() { Name = "���" },
                new City() { Name = "����" },
                new City() { Name = "г���" },
                new City() { Name = "��������������" },
                new City() { Name = "�����" },
                new City() { Name = "�����" },
                new City() { Name = "�����" },
                new City() { Name = "�������" },
                new City() { Name = "ʳ��������" },
                new City() { Name = "����" },
                new City() { Name = "�������" },
                new City() { Name = "�������" },
                new City() { Name = "��������" },
                new City() { Name = "�����-���������" },
                new City() { Name = "³�����" },
                new City() { Name = "�������" },
                new City() { Name = "������" },
                new City() { Name = "��������" },
                new City() { Name = "�������" },
                new City() { Name = "������" }
            };
            cities.ForEach(c => context.Cities.AddOrUpdate(c));

            context.SaveChanges();

            var universities = new List<University>()
            {
                new University() {Name = "�ϲ"},
                new University() {Name = "�ϲ"},
                new University() {Name = "���"}
            };

            universities.ForEach(u => context.Universities.AddOrUpdate(u));
            context.SaveChanges();

            var universityFaculties = new List<UniversityFaculty>()
            {
                new UniversityFaculty() { UniversityId = 1, Name = "������������ ��������� - ��" },
                new UniversityFaculty() { UniversityId = 1, Name = "���������-�i������ ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "���������-�i�i���� ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� ��������� ������� - ��̲" },
                new UniversityFaculty() { UniversityId = 1, Name = "����������i���� ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "���i�����i���� ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "����������������� ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� ��i��i���� i ����i���� ������ - ����" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� �i���������i� i �i�����i�� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� �����������������i�� �� ���������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� ��������i�� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� i���������� �� ������������� ����i�� - Բ��" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� �i���i����� - ��" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� ����������� �� ���������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� �������㳿 � ����� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "��������� ��������� ���������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "�i����-������������ ��������� - ���" },
                new UniversityFaculty() { UniversityId = 1, Name = "�i�i��-��������i���� ��������� - ���" },
                new UniversityFaculty() { UniversityId = 2, Name = "�����������" },
                new UniversityFaculty() { UniversityId = 2, Name = "���������� �� ������� �������" },
                new UniversityFaculty() { UniversityId = 2, Name = "�����糿" },
                new UniversityFaculty() { UniversityId = 2, Name = "����������� �� ���������� ����" },
                new UniversityFaculty() { UniversityId = 2, Name = "������������� ��������" },
                new UniversityFaculty() { UniversityId = 2, Name = "�����㳿, ��������������� �������� �� �������" },
                new UniversityFaculty() { UniversityId = 2, Name = "�������� � �����������" },
                new UniversityFaculty() { UniversityId = 2, Name = "���������� �� ������ ���������" },
                new UniversityFaculty() { UniversityId = 2, Name = "��������� ������� �� ����������" },
                new UniversityFaculty() { UniversityId = 2, Name = "����'������� ���� �� ������������� ���������" },
                new UniversityFaculty() { UniversityId = 2, Name = "����'������� ���������, ���������� �� �������㳿" },
                new UniversityFaculty() { UniversityId = 2, Name = "̳��������� �������� �����, �������� �� ������ � ��������" },
                new UniversityFaculty() { UniversityId = 2, Name = "ϳ����������� �� ������������� ���������" },
                new UniversityFaculty() { UniversityId = 2, Name = "ϳ����������� �����" },
                new UniversityFaculty() { UniversityId = 2, Name = "����� �� �������㳿" },
                new UniversityFaculty() { UniversityId = 2, Name = "��������� ���������� �� ��������������� ����" },
                new UniversityFaculty() { UniversityId = 2, Name = "��������������, �������������� �� ���������� ������" },
                new UniversityFaculty() { UniversityId = 2, Name = "ճ쳿 �� ������� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "������������ ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "���������� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "���������� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "�������-������������ ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� ������������� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� ����������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� �������� ��������� ��������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� �������㳿" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� ����������, ���������� �� ����'������� ������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� �������㳿" },
                new UniversityFaculty() { UniversityId = 3, Name = "Գ������ ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "Գ���������� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "ճ����� ���������" },
                new UniversityFaculty() { UniversityId = 3, Name = "��������� ���������" }
            };

            universityFaculties.ForEach(f => context.Faculties.AddOrUpdate(f));
            context.SaveChanges();


        }
    }
}                           