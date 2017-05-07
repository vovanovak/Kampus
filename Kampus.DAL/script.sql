insert into Cities(Name)
values ('Kyiv')
go

insert into Cities(Name)
values ('Lviv')
go


insert into Cities(Name)
values ('Rivne')
go

insert into Cities(Name)
values ('Kharkiv')
go

insert into Cities(Name)
values ('Dnipropetrovsk')
go

insert into UserRoles(Name)
values ('Admin')
go

insert into UserRoles(Name)
values ('User')
go

insert into TaskCategories(Name)
values ('Maths')
go

insert into TaskCategories(Name)
values ('English')
go

insert into TaskCategories(Name)
values ('Physics')
go

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('English1', 1)

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('English2', 1)

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('Maths1', 2)

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('Maths2', 2)

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('Physics1', 3)

insert into TaskSubcats(Name, TaskTaskCategoryId)
values ('Physics2', 3)

insert into Universities(Name)
values('KPI')

insert into Universities(Name)
values('LPI')

insert into Universities(Name)
values('KNY')

insert into UniversityFaculties(Name, UniversityId)
values('English1', 1)

insert into UniversityFaculties(Name, UniversityId)
values('Maths1', 1)

insert into UniversityFaculties(Name, UniversityId)
values('Physics1', 1)

insert into UniversityFaculties(Name, UniversityId)
values('English2', 2)

insert into UniversityFaculties(Name, UniversityId)
values('Maths2', 2)

insert into UniversityFaculties(Name, UniversityId)
values('Physics2', 2)

insert into UniversityFaculties(Name, UniversityId)
values('English3', 3)

insert into UniversityFaculties(Name, UniversityId)
values('Maths3', 3)

insert into UniversityFaculties(Name, UniversityId)
values('Physics3', 3)