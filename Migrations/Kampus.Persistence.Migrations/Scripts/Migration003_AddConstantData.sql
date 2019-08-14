﻿INSERT INTO [dbo].[TaskCategories]([Name], [Description])
VALUES
	('Програмування', 'Охоплює мови програмування'),
	('Моделювання', 'Охоплює всі можливі засоби моделювання та проектування'),
	('Інформатика', 'Охоплює операційні ситеми та інші додатки цього напряму'),
	('Математика', 'Охоплює всі можливі дисципліни з математики'),
	('Економіка', 'Охоплює дисципліни з економіки'),
	('Правознавство', 'Охоплює дисципліни, які стосуються теоретичного та практичного використання права в Україні'),
	('Гуманітарні дисципліни', 'Охоплює дисципліни гуманітарного напрямку'),
	('Інші', 'Охоплює всі інші дисципліни, які не увійшли у попередні категорії')
GO

INSERT INTO [dbo].[TaskSubcategories]([Name], [TaskCategoryId], [Description])
VALUES
	('C', 1, 'Матеріали та послуги, які стосуються мови програмування ''С'''),
	('C++', 1, 'Матеріали та послуги, які стосуються мови програмування ''С++'''),
	('C#', 1, 'Матеріали та послуги, які стосуються мови програмування ''С#'''),
	('Pascal', 1, 'Матеріали та послуги, які стосуються мови програмування ''Pascal'''),
	('Objective C', 1, 'Матеріали та послуги, які стосуються мови програмування ''Objective C'''),
	('Visual Basic', 1, 'Матеріали та послуги, які стосуються мови програмування ''Visual Basic'''),
	('T-SQL', 1, 'Матеріали та послуги, які стосуються мови програмування ''T-SQL'''),
	('Java', 1, 'Матеріали та послуги, які стосуються мови програмування ''Java'''),
	('JavaScript', 1, 'Матеріали та послуги, які стосуються мови програмування ''Java Script'''),
	('Delphi', 1, 'Матеріали та послуги, які стосуються мови програмування ''Delphi'''),
	('PHP', 1, 'Матеріали та послуги, які стосуються мови програмування ''PHP'''),
	('HTML+CSS', 1, 'Матеріали та послуги, які стосуються мов розмітки ''HTML+CSS'''),
	('1C', 1, 'Матеріали та послуги, які стосуються мови програмування ''1C'''),
	('Clips', 1, 'Матеріали та послуги, які стосуються мови програмування ''Clips'''),
	('Економіко-математичне моделювання (ЕММ)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Економіко-математичне моделювання'''),
	('Моделювання економіки (МЕ)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Моделювання економіки'''),
	('Прикладні задачі моделювання економічних процесів (ПЗМЕП)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Прикладні задачі моделювання економічних процесів'''),
	('Математичне програмне забеспеченн (МПЗ)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Прикладні задачі моделювання інформаційних систем'''),
	('Математичне програмне забеспеченн. Математичний аналіз (МПЗМА)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Прикладні задачі моделювання інформаційних систем. Математичний аналіз'''),
	('Система пітримки прийняття рішень (СППР)', 2, 'Матеріали та послуги, які стосуються напряпряму ''Cистема пітримки прийняття рішень'''),
	('Адміністрування інформаційних систем. Захист інформаційних систем', 3, 'Матеріали та послуги, які стосуються напряпряму ''Адміністрування інформаційних систем. Захист інформаційних систем'''),
	('Адміністрування інформаційних систем. Операційні системи', 3, 'Матеріали та послуги, які стосуються напряпряму ''Адміністрування інформаційних систем. Операційні системи'''),
	('Адміністрування інформаційних систем. Комп''ютерні мережі', 3, 'Матеріали та послуги, які стосуються напряпряму ''Адміністрування інформаційних систем. Комп ютерні мережі'''),
	('Інформатика', 3, 'Матеріали та послуги, які стосуються напряпряму ''Інформатика'''),
	('Математика для економістів', 4, 'Матеріали та послуги, які стосуються напряпряму ''Математика для економістів'''),
	('Математика для економістів (теорія ймовірності та математична статистика)', 4, 'Матеріали та послуги, які стосуються напряпряму ''Математика для економістів (теорія ймовірності та математична статистика)'''),
	('Статистика', 4, 'Матеріали та послуги, які стосуються напряпряму ''Статистика'''),
	('Теорія випадкових процесів (ТВП)', 4, 'Матеріали та послуги, які стосуються напряпряму ''Теорія випадкових процесів (ТВП)'''),
	('Алгебра',  4, 'Матеріали та послуги, які стосуються напряпряму ''Алгебра'''),
	('Геометрія', 4, 'Матеріали та послуги, які стосуються напряпряму ''Геометрія'''),
	('Банківська справа', 5, 'Матеріали та послуги, які стосуються напряпряму ''Банківська справа'''),
	('Бухгалтерський облік', 5, 'Матеріали та послуги, які стосуються напряпряму ''Бухгалтерський облік'''),
	('Гроші та кредит', 5, 'Матеріали та послуги, які стосуються напряпряму ''Гроші та кредит'''),
	('Економіка підприємства', 5, 'Матеріали та послуги, які стосуються напряпряму ''Економіка підприємства'''),
	('Історія економіки та економічної думки', 5, 'Матеріали та послуги, які стосуються напряпряму ''Історія економіки та економічної думки'''),
	('Макроекономіка', 5, 'Матеріали та послуги, які стосуються напряпряму ''Макроекономіка'''),
	('Мікроекономіка', 5, 'Матеріали та послуги, які стосуються напряпряму ''Мікроекономіка'''),
	('Маркетинг', 5, 'Матеріали та послуги, які стосуються напряпряму ''Маркетинг'''),
	('Менеджмент', 5, 'Матеріали та послуги, які стосуються напряпряму ''Менеджмент'''),
	('Міжнародна економіка', 5, 'Матеріали та послуги, які стосуються напряпряму ''Міжнародна економіка'''),
	('Національна економіка', 5, 'Матеріали та послуги, які стосуються напряпряму ''Національна економіка'''),
	('Політична економія', 5, 'Матеріали та послуги, які стосуються напряпряму ''Політична економія'''),
	('Фінанси', 5, 'Матеріали та послуги, які стосуються напряпряму ''Фінанси'''),
	('Регіональна економіка', 5, 'Матеріали та послуги, які стосуються напряпряму ''Регіональна економіка'''),
	('Основи конституційного права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи конституційного права України'''),
	('Основи трудового права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи трудового права України'''),
	('Основи права про соціальний захист', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи права про соціальний захист'''),
	('Основи екологічного права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи екологічного права України'''),
	('Основи осонови цивільного права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи осонови цивільного права України'''),
	('Основи фінансового права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи фінаннсового права України'''),
	('Основи житлового права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи житлового права України'''),
	('Основи трудового права', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи трудового права'''),
	('Основи шлюбно-сімейного права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи шлюбно-сімейного права України'''),
	('Основи адміністративного права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи адміністративного права України'''),
	('Основи кримінального права України', 6, 'Матеріали та послуги, які стосуються напряпряму ''Основи кримінального права України'''),
	('Суд і нотаріат', 6, 'Матеріали та послуги, які стосуються напряпряму ''Суд і нотаріат'''),
	('Прокуратура, адвокатура та інші правоохоронні органи', 6, 'Матеріали та послуги, які стосуються напряпряму ''Прокуратура, адвокатура та інші правоохоронні органи'''),
	('Іноземна мова', 7, 'Матеріали та послуги, які стосуються напряпряму ''Основи конституційного права України'''),
	('Поглиблене вивчення іноземної мови', 7, 'Матеріали та послуги, які стосуються напряпряму ''Основи конституційного права України'''),
	('Українська мова за професійним спрямуванням', 7, 'Матеріали та послуги, які стосуються напряпряму ''Основи конституційного права України'''),
	('Українська мова', 7, 'Матеріали та послуги, які стосуються напряпряму ''Основи конституційного права України'''),
	('Культурологія', 7, 'Матеріали та послуги, які стосуються напряпряму ''Культурологія'''),
	('Етика та естетика', 7, 'Матеріали та послуги, які стосуються напряпряму ''Етика та естетик'''),
	('Історія України', 7, 'Матеріали та послуги, які стосуються напряпряму ''Історія України'''),
	('Всесвітня історія', 7, 'Матеріали та послуги, які стосуються напряпряму ''Всесвітня історія'''),
	('Психологія та педагогіка', 7, 'Матеріали та послуги, які стосуються напряпряму ''Психологія та педагогіка'''),
	('Філософія', 7, 'Матеріали та послуги, які стосуються напряпряму ''Філософія'''),
	('Українська література', 7, 'Матеріали та послуги, які стосуються напряпряму ''Українська література'''),
	('Політологія', 8, 'Матеріали та послуги, які стосуються напряпряму ''Політологія'''),
	('Інформаційний бізнес', 8, 'Матеріали та послуги, які стосуються напряпряму ''Інформаційний бізнес'''),
	('Фізичне виховання', 8, 'Матеріали та послуги, які стосуються напряпряму ''Фізичне виховання'''),
	('Фізика', 8, 'Матеріали та послуги, які стосуються напряпряму ''Фізика'''),
	('Біолгія', 8, 'Матеріали та послуги, які стосуються напряпряму ''Біолoгія'''),
	('Біолгія', 8, 'Матеріали та послуги, які стосуються напряпряму ''Біолoгія'''),
	('Біолгія', 8, 'Матеріали та послуги, які стосуються напряпряму ''Біолoгія'''),
	('Хімія', 8, 'Матеріали та послуги, які стосуються напряпряму ''Хімія''')
GO

INSERT INTO [dbo].[Cities]([Name])
VALUES
	('Київ'),
	('Львів'),
	('Рівне'),
	('Дніпропетровськ'),
	('Харків'),
	('Луцьк'),
	('Одеса'),
	('Ужгород'),
	('Кіровоград'),
	('Суми'),
	('Полтава'),
	('Житомир'),
	('Тернопіль'),
	('Івано-Франківськ'),
	('Вінниця'),
	('Чернівці'),
	('Чернігів'),
	('Запоріжжя'),
	('Миколаїв'),
	('Херсон')
GO

INSERT INTO [dbo].[Universities]([Name])
VALUES
	('КПІ'),
	('ЛПІ'),
	('КНУ')
GO

INSERT INTO [dbo].[Faculties]([UniversityId], [Name])
VALUES
	(1, 'Зварювальний факультет - ЗФ'),
	(1, 'Інженерно-фiзичний факультет - ІФФ'),
	(1, 'Інженерно-хiмiчний факультет - ІХФ'),
	(1, 'Факультет біомедичної інженерії - ФБМІ'),
	(1, 'Приладобудiвний факультет - ПБФ'),
	(1, 'Радiотехнiчний факультет - РТФ'),
	(1, 'Теплоенергетичний факультет - ТЕФ'),
	(1, 'Факультет авiацiйних i космiчних систем - ФАКС'),
	(1, 'Факультет бiотехнологiї i бiотехнiки - ФБТ'),
	(1, 'Факультет електроенерготехнiки та автоматики - ФЕА'),
	(1, 'Факультет електронiки - ФЕЛ'),
	(1, 'Факультет iнформатики та обчислювальної технiки - ФІОТ'),
	(1, 'Факультет лiнгвiстики - ФЛ'),
	(1, 'Факультет менеджменту та маркетингу - ФММ'),
	(1, 'Факультет соціології і права - ФСП'),
	(1, 'Факультет прикладної математики - ФПМ'),
	(1, 'Фiзико-математичний факультет - ФМФ'),
	(1, 'Хiмiко-технологiчний факультет - ХТФ'),
	(2, 'Архітектури'),
	(2, 'Будівництва та інженерії довкілля'),
	(2, 'Геодезії'),
	(2, 'Гуманітарних та соціальних наук'),
	(2, 'Дистанційного навчання'),
	(2, 'Екології, природоохоронної діяльності та туризму'),
	(2, 'Економіки і менеджменту'),
	(2, 'Енергетики та систем керування'),
	(2, 'Інженерної механіки та транспорту'),
	(2, 'Комп''ютерних наук та інформаційних технологій'),
	(2, 'Комп''ютерних технологій, автоматики та метрології'),
	(2, 'Міжнародний інститут освіти, культури та зв’язків з діаспорою'),
	(2, 'Підприємництва та перспективних технологій'),
	(2, 'Післядипломної освіти'),
	(2, 'Права та психології'),
	(2, 'Прикладної математики та фундаментальних наук'),
	(2, 'Телекомунікацій, радіоелектроніки та електронної техніки'),
	(2, 'Хімії та хімічних технологій'),
	(3, 'Географічний факультет'),
	(3, 'Економічний факультет'),
	(3, 'Історичний факультет'),
	(3, 'Механіко-математичний факультет'),
	(3, 'Факультет інформаційних технологій'),
	(3, 'Факультет кібернетики'),
	(3, 'Факультет навчання іноземних громадян'),
	(3, 'Факультет психології'),
	(3, 'Факультет радіофізики, електроніки та комп''ютерних систем'),
	(3, 'Факультет соціології'),
	(3, 'Фізичний факультет'),
	(3, 'Філософський факультет'),
	(3, 'Хімічний факультет'),
	(3, 'Юридичний факультет')
GO

INSERT INTO [dbo].[Roles]([Name])
VALUES ('Admin'),
    ('User')