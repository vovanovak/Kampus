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
                new TaskCategory  {Name = "Програмування", Description = "Охоплює мови програмування"},
                new TaskCategory  {Name = "Моделювання", Description = "Охоплює всі можливі засоби моделювання та проектування"},
                new TaskCategory  {Name = "Інформатика", Description = "Охоплює операційні ситеми та інші додатки цього напряму"},
                new TaskCategory  {Name = "Математика", Description = "Охоплює всі можливі дисципліни з математики"},
                new TaskCategory  {Name = "Економіка", Description = "Охоплює дисципліни з економіки"},
                new TaskCategory  {Name = "Правознавство", Description = "Охоплює дисципліни, які стосуються теоретичного та практичного використання права в Україні"},
                new TaskCategory  {Name = "Гуманітарні дисципліни", Description = "Охоплює дисципліни гуманітарного напрямку"},
                new TaskCategory  {Name = "Інші", Description = "Охоплює всі інші дисципліни, які не увійшли у попередні категорії"}
                
            };
            categories.ForEach(c => context.TaskCategories.AddOrUpdate(c));

            context.SaveChanges();

            var subcategories = new List<TaskSubcat>
            {
               new TaskSubcat {Name = "C", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'С'"},
               new TaskSubcat {Name = "C++", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'С++'"},
               new TaskSubcat {Name = "C#", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'С#'"},
               new TaskSubcat {Name = "Pascal", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Pascal'"},
               new TaskSubcat {Name = "Objective C", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Objective C'"},
               new TaskSubcat {Name = "Visual Basic", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Visual Basic'"},
               new TaskSubcat {Name = "T-SQL", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'T-SQL'"},
               new TaskSubcat {Name = "Java", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Java'"},
               new TaskSubcat {Name = "JavaScript", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Java Script'"},
               new TaskSubcat {Name = "Delphi", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Delphi'"},
               new TaskSubcat {Name = "PHP", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'PHP'"},
               new TaskSubcat {Name = "HTML+CSS", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мов розмітки 'HTML+CSS'"},
               new TaskSubcat {Name = "1C", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування '1C'"},
               new TaskSubcat {Name = "Clips", TaskCategoryId = 1, Description = "Матеріали та послуги, які стосуються мови програмування 'Clips'"},
               new TaskSubcat {Name = "Економіко-математичне моделювання (ЕММ)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Економіко-математичне моделювання'"},
               new TaskSubcat {Name = "Моделювання економіки (МЕ)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Моделювання економіки'"},
               new TaskSubcat {Name = "Прикладні задачі моделювання економічних процесів (ПЗМЕП)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Прикладні задачі моделювання економічних процесів'"},
               new TaskSubcat {Name = "Математичне програмне забеспеченн (МПЗ)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Прикладні задачі моделювання інформаційних систем'"},
               new TaskSubcat {Name = "Математичне програмне забеспеченн. Математичний аналіз (МПЗМА)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Прикладні задачі моделювання інформаційних систем. Математичний аналіз'"},
               new TaskSubcat {Name = "Система пітримки прийняття рішень (СППР)", TaskCategoryId = 2, Description = "Матеріали та послуги, які стосуються напряпряму 'Cистема пітримки прийняття рішень'"},
               new TaskSubcat {Name = "Адміністрування інформаційних систем. Захист інформаційних систем", TaskCategoryId = 3, Description = "Матеріали та послуги, які стосуються напряпряму 'Адміністрування інформаційних систем. Захист інформаційних систем'"},
               new TaskSubcat {Name = "Адміністрування інформаційних систем. Операційні системи", TaskCategoryId = 3, Description = "Матеріали та послуги, які стосуються напряпряму 'Адміністрування інформаційних систем. Операційні системи'"},
               new TaskSubcat {Name = "Адміністрування інформаційних систем. Комп'ютерні мережі", TaskCategoryId = 3, Description = "Матеріали та послуги, які стосуються напряпряму 'Адміністрування інформаційних систем. Комп ютерні мережі'"},
               new TaskSubcat {Name = "Інформатика", TaskCategoryId = 3, Description = "Матеріали та послуги, які стосуються напряпряму 'Інформатика'"},
               new TaskSubcat {Name = "Математика для економістів", TaskCategoryId = 4, Description = "Матеріали та послуги, які стосуються напряпряму 'Математика для економістів'"},
               new TaskSubcat {Name = "Математика для економістів (теорія ймовірності та математична статистика)", TaskCategoryId = 4, Description = "Матеріали та послуги, які стосуються напряпряму 'Математика для економістів (теорія ймовірності та математична статистика)'"},
               new TaskSubcat {Name = "Статистика", TaskCategoryId = 4, Description = "Матеріали та послуги, які стосуються напряпряму 'Статистика'"},
               new TaskSubcat {Name = "Теорія випадкових процесів (ТВП)", TaskCategoryId = 4, Description = "Матеріали та послуги, які стосуються напряпряму 'Теорія випадкових процесів (ТВП)'"},
               new TaskSubcat {Name = "Алгебра", TaskCategoryId =  4, Description = "Матеріали та послуги, які стосуються напряпряму 'Алгебра'"},
               new TaskSubcat {Name = "Геометрія", TaskCategoryId = 4, Description = "Матеріали та послуги, які стосуються напряпряму 'Геометрія'"},
               new TaskSubcat {Name = "Банківська справа", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Банківська справа'"},
               new TaskSubcat {Name = "Бухгалтерський облік", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Бухгалтерський облік'"},
               new TaskSubcat {Name = "Гроші та кредит", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Гроші та кредит'"},
               new TaskSubcat {Name = "Економіка підприємства", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Економіка підприємства'"},
               new TaskSubcat {Name = "Історія економіки та економічної думки", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Історія економіки та економічної думки'"},
               new TaskSubcat {Name = "Макроекономіка", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Макроекономіка'"},
               new TaskSubcat {Name = "Мікроекономіка", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Мікроекономіка'"},
               new TaskSubcat {Name = "Маркетинг", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Маркетинг'"},
               new TaskSubcat {Name = "Менеджмент", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Менеджмент'"},
               new TaskSubcat {Name = "Міжнародна економіка", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Міжнародна економіка'"},
               new TaskSubcat {Name = "Національна економіка", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Національна економіка'"},
               new TaskSubcat {Name = "Політична економія", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Політична економія'"},
               new TaskSubcat {Name = "Фінанси", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Фінанси'"},
               new TaskSubcat {Name = "Регіональна економіка", TaskCategoryId = 5, Description = "Матеріали та послуги, які стосуються напряпряму 'Регіональна економіка'"},
               new TaskSubcat {Name = "Основи конституційного права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи конституційного права України'"},
               new TaskSubcat {Name = "Основи трудового права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи трудового права України'"},
               new TaskSubcat {Name = "Основи права про соціальний захист", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи права про соціальний захист'"},
               new TaskSubcat {Name = "Основи екологічного права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи екологічного права України'"},
               new TaskSubcat {Name = "Основи осонови цивільного права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи осонови цивільного права України'"},
               new TaskSubcat {Name = "Основи фінансового права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи фінаннсового права України'"},
               new TaskSubcat {Name = "Основи житлового права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи житлового права України'"},
               new TaskSubcat {Name = "Основи трудового права", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи трудового права'"},
               new TaskSubcat {Name = "Основи шлюбно-сімейного права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи шлюбно-сімейного права України'"},
               new TaskSubcat {Name = "Основи адміністративного права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи адміністративного права України'"},
               new TaskSubcat {Name = "Основи кримінального права України", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи кримінального права України'"},
               new TaskSubcat {Name = "Суд і нотаріат", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Суд і нотаріат'"},
               new TaskSubcat {Name = "Прокуратура, адвокатура та інші правоохоронні органи", TaskCategoryId = 6, Description = "Матеріали та послуги, які стосуються напряпряму 'Прокуратура, адвокатура та інші правоохоронні органи'"},
               new TaskSubcat {Name = "Іноземна мова", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи конституційного права України'"},
               new TaskSubcat {Name = "Поглиблене вивчення іноземної мови", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи конституційного права України'"},
               new TaskSubcat {Name = "Українська мова за професійним спрямуванням", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи конституційного права України'"},
               new TaskSubcat {Name = "Українська мова", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Основи конституційного права України'"},
               new TaskSubcat {Name = "Культурологія", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Культурологія'"},
               new TaskSubcat {Name = "Етика та естетика", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Етика та естетик'"},
               new TaskSubcat {Name = "Історія України", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Історія України'"},
               new TaskSubcat {Name = "Всесвітня історія", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Всесвітня історія'"},
               new TaskSubcat {Name = "Психологія та педагогіка", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Психологія та педагогіка'"},
               new TaskSubcat {Name = "Філософія", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Філософія'"},
               new TaskSubcat {Name = "Українська література", TaskCategoryId = 7, Description = "Матеріали та послуги, які стосуються напряпряму 'Українська література'"},
               new TaskSubcat {Name = "Політологія", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Політологія'"},
               new TaskSubcat {Name = "Інформаційний бізнес", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Інформаційний бізнес'"},
               new TaskSubcat {Name = "Фізичне виховання", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Фізичне виховання'"},
               new TaskSubcat {Name = "Фізика", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Фізика'"},
               new TaskSubcat {Name = "Біолгія", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Біолгія'"},
               new TaskSubcat {Name = "Хімія", TaskCategoryId = 8, Description = "Матеріали та послуги, які стосуються напряпряму 'Хімія'"}

            };

            subcategories.ForEach(c => context.TaskSubcats.AddOrUpdate(c));

            context.SaveChanges();

            var cities = new List<City>()
            {
                new City() { Name = "Київ" },
                new City() { Name = "Львів" },
                new City() { Name = "Рівне" },
                new City() { Name = "Дніпропетровськ" },
                new City() { Name = "Харків" },
                new City() { Name = "Луцьк" },
                new City() { Name = "Одеса" },
                new City() { Name = "Ужгород" },
                new City() { Name = "Кіровоград" },
                new City() { Name = "Суми" },
                new City() { Name = "Полтава" },
                new City() { Name = "Житомир" },
                new City() { Name = "Тернопіль" },
                new City() { Name = "Івано-Франківськ" },
                new City() { Name = "Вінниця" },
                new City() { Name = "Чернівці" },
                new City() { Name = "Чернігів" },
                new City() { Name = "Запоріжжя" },
                new City() { Name = "Миколаїв" },
                new City() { Name = "Херсон" }
            };
            cities.ForEach(c => context.Cities.AddOrUpdate(c));

            context.SaveChanges();

            var universities = new List<University>()
            {
                new University() {Name = "КПІ"},
                new University() {Name = "ЛПІ"},
                new University() {Name = "КНУ"}
            };

            universities.ForEach(u => context.Universities.AddOrUpdate(u));
            context.SaveChanges();

            var universityFaculties = new List<UniversityFaculty>()
            {
                new UniversityFaculty() { UniversityId = 1, Name = "Зварювальний факультет - ЗФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Інженерно-фiзичний факультет - ІФФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Інженерно-хiмiчний факультет - ІХФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет біомедичної інженерії - ФБМІ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Приладобудiвний факультет - ПБФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Радiотехнiчний факультет - РТФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Теплоенергетичний факультет - ТЕФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет авiацiйних i космiчних систем - ФАКС" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет бiотехнологiї i бiотехнiки - ФБТ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет електроенерготехнiки та автоматики - ФЕА" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет електронiки - ФЕЛ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет iнформатики та обчислювальної технiки - ФІОТ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет лiнгвiстики - ФЛ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет менеджменту та маркетингу - ФММ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет соціології і права - ФСП" },
                new UniversityFaculty() { UniversityId = 1, Name = "Факультет прикладної математики - ФПМ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Фiзико-математичний факультет - ФМФ" },
                new UniversityFaculty() { UniversityId = 1, Name = "Хiмiко-технологiчний факультет - ХТФ" },
                new UniversityFaculty() { UniversityId = 2, Name = "Архітектури" },
                new UniversityFaculty() { UniversityId = 2, Name = "Будівництва та інженерії довкілля" },
                new UniversityFaculty() { UniversityId = 2, Name = "Геодезії" },
                new UniversityFaculty() { UniversityId = 2, Name = "Гуманітарних та соціальних наук" },
                new UniversityFaculty() { UniversityId = 2, Name = "Дистанційного навчання" },
                new UniversityFaculty() { UniversityId = 2, Name = "Екології, природоохоронної діяльності та туризму" },
                new UniversityFaculty() { UniversityId = 2, Name = "Економіки і менеджменту" },
                new UniversityFaculty() { UniversityId = 2, Name = "Енергетики та систем керування" },
                new UniversityFaculty() { UniversityId = 2, Name = "Інженерної механіки та транспорту" },
                new UniversityFaculty() { UniversityId = 2, Name = "Комп'ютерних наук та інформаційних технологій" },
                new UniversityFaculty() { UniversityId = 2, Name = "Комп'ютерних технологій, автоматики та метрології" },
                new UniversityFaculty() { UniversityId = 2, Name = "Міжнародний інститут освіти, культури та зв’язків з діаспорою" },
                new UniversityFaculty() { UniversityId = 2, Name = "Підприємництва та перспективних технологій" },
                new UniversityFaculty() { UniversityId = 2, Name = "Післядипломної освіти" },
                new UniversityFaculty() { UniversityId = 2, Name = "Права та психології" },
                new UniversityFaculty() { UniversityId = 2, Name = "Прикладної математики та фундаментальних наук" },
                new UniversityFaculty() { UniversityId = 2, Name = "Телекомунікацій, радіоелектроніки та електронної техніки" },
                new UniversityFaculty() { UniversityId = 2, Name = "Хімії та хімічних технологій" },
                new UniversityFaculty() { UniversityId = 3, Name = "Географічний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Економічний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Історичний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Механіко-математичний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет інформаційних технологій" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет кібернетики" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет навчання іноземних громадян" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет психології" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет радіофізики, електроніки та комп'ютерних систем" },
                new UniversityFaculty() { UniversityId = 3, Name = "Факультет соціології" },
                new UniversityFaculty() { UniversityId = 3, Name = "Фізичний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Філософський факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Хімічний факультет" },
                new UniversityFaculty() { UniversityId = 3, Name = "Юридичний факультет" }
            };

            universityFaculties.ForEach(f => context.Faculties.AddOrUpdate(f));
            context.SaveChanges();


        }
    }
}                           