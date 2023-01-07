using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace TravelAgencyHRD
{
    public static class ValuePadding
    {
        public static void ValuesForEducation(ApplicationDbContext appDbContext)
        {
            appDbContext
            .Database
            .ExecuteSqlRaw
                 (@"
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('History', 'English, Ukrainian, German', 'Bachelor', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Culture', 'English, Ukrainian', 'Bachelor', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Philosophy',  'Ukrainian', 'Bachelor', 'FALSE'); 
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Hotel business',  'Ukrainian, German', 'Bachelor', 'FALSE');  
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Economic',  'English, Ukrainian', 'Master', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Economics and Mathematics',  'English, Ukrainian', 'Bachelor', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Strategic Managment',  'English, Ukrainian, Polish, German', 'Master', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Financial management and human resource management',  'English, Ukrainian, German', 'Master', 'FALSE');
                    insert into PersonsEducation (Profession, LanguageSkills, ACDegree, IsDeleted) values ('Hotel business',  'Ukrainian, German, English, Polish', 'Master', 'FALSE');  
                 ");
        }
        public static void ValuesForPerson(ApplicationDbContext appDbContext)
        {
           appDbContext
           .Database
           .ExecuteSqlRaw
                (@"
                    insert into Person (Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values ('Lysenko Inna Oleksandrivnya', 'Ukrainian', 1, 'F', '2004-08-04', 'unmarried', '+123456789011', 'grant.christop@mexcool.com', 0, 0, 'FALSE');     
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Kramarenko Anton Ivanovich', 'Polish', 5, 'M', '1977-08-26', 'unmarried', '+123456783212', 'royal.wilkinson@vs-neustift.ua', 1, 0, 'FALSE');                 
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Kravchuk Natasha Mykolayivna', 'Ukrainian', 1, 'F', '1988-04-14', 'unmarried', '+107900444264', 'royal.wilkinson@vs-neustift.ua', 1, 2, 'FALSE');                 
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Sereda Andriy Yanovich', 'German', 2, 'M', '1963-11-07', 'married', '+104149755937', 'alec48@speeddataanalytics.com', 0, 1, 'FALSE');                 
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Lisenko Yaroslav Petrovich', 'Ukrainian', 3, 'M', '1987-07-06', 'married', '+380939764306', 'inna82@mtcxmail.com', 1, 3, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Ponomarchuk Volodymyr Evgenovich', 'Ukrainian', 2, 'M', '1976-04-29', 'unmarried', '+380677046794', 'valentina86@danirafsanjani.com', 0, 0, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Bondarenko Raisa Yanivna', 'Ukrainian', 3, 'F', '1990-10-30', 'married', '+380630707573', 'lsereda@shiro.pw', 1, 0, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Lev Andriyovich Dmitrenko', 'Ukrainian', 4, 'M', '1991-02-20', 'unmarried', '+380994876521', 'ponomarcuk.sofi@chiguires.com', 0, 0, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Gnatiuk Raisa Tarasivna', 'Ukrainian', 4, 'F', '1991-02-20', 'unmarried', '+380677371248', 'illa90@weinzed.com', 1, 0, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Pavlyuk Raisa Sergiivna', 'Italian', 2, 'F', '1983-03-18', 'married', '+380966180261', 'brovarcuk.arosl@ianz.pro', 1, 3, 'FALSE');   
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Shevchuk Bogdan Oleksandrovych', 'Ukrainian', 6, 'M', '1990-04-22', 'unmarried', '+380937230252', 'tamara.ponomare@greendike.com', 1, 0, 'FALSE'); 
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Kravchuk Vadim Sergeyevich', 'Ukrainian', 7, 'M', '1977-06-26', 'married', '+380674374407', 'olena76@boranora.com', 1, 2, 'FALSE');
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Ponomarchuk Georgy Anatoliyovych', 'Ukrainian', 8, 'M', '1978-03-13', 'unmarried', '+380936446190', 'polina71@kitnastar.com', 1, 0, 'FALSE');                
                    insert into Person(Initials, Nationality, EducationId, Sex, DateOfBirth, FamilyStatus, PhoneNumber, Email, IsPhotoAvaliable, ChildrensCount, IsDeleted) values('Ignatova Varvara Antonivna', 'Ukrainian', 9, 'F', '1985-11-01', 'married', '+380765860907', 'vv12@antw.com', 1, 3, 'FALSE');   
");
        }
        public static void ValuesForPost(ApplicationDbContext appDbContext)
        {
           appDbContext
           .Database
           .ExecuteSqlRaw
                (@"
                    insert into Post(Name, IsDeleted) values('Chief Accountant', 'FALSE');
                    insert into Post(Name, IsDeleted) values('Accountant', 'FALSE');
                    insert into Post(Name, IsDeleted) values('Deputy Director', 'FALSE');
                    insert into Post(Name, IsDeleted) values('Director', 'FALSE');
                    insert into Post(Name, IsDeleted) values('Manager', 'FALSE');
                    insert into Post(Name, IsDeleted) values('Manager''s assistant', 'FALSE');
                ");
        }
        public static void ValuesForCabinet(ApplicationDbContext appDbContext)
        {
         /* appDbContext
          .Database
          .ExecuteSqlRaw
               (@"
                    insert into Cabinet(Name, CabinetNumber, Floor, IsDeleted) values('Accounting', 19, 2, 'FALSE');
                    insert into Cabinet(Name, CabinetNumber, Floor, IsDeleted) values('Chief Accounting', 21, 2, 'FALSE');
                    insert into Cabinet(Name, CabinetNumber, Floor, IsDeleted) values('Guest room', 5, 1, 'FALSE');
                    insert into Cabinet(Name, CabinetNumber, Floor, IsDeleted) values('Main office', 12, 1, 'FALSE');
                    insert into Cabinet(Name, CabinetNumber, Floor, IsDeleted) values('Director''s office', 1, 3, 'FALSE');
               ");*/
            appDbContext.Cabinet.AddRange(
               new Cabinet() { Name = "Accounting", CabinetNumber = 19, Floor = 2 },
               new Cabinet() { Name = "Chief Accounting", CabinetNumber = 21, Floor = 2 },
               new Cabinet() {  Name = "Guest room", CabinetNumber = 5, Floor = 1 },
               new Cabinet() { Name = "Main office", CabinetNumber = 12, Floor = 1 },
               new Cabinet() {  Name = "Director''s office", CabinetNumber = 1, Floor = 3 }
               );
            appDbContext.SaveChanges();
        }
        public static void ValuesForSeats(ApplicationDbContext appDbContext)
        {
            /* appDbContext
             .Database
             .ExecuteSqlRaw
                  (@"
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(2, 9, 1, 80000, 'FALSE');
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(1, 1, 2, 15000, 'FALSE');
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(3, 1, 4, 18000, 'FALSE');               
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(4, 1, 5, 25000, 'FALSE'); 
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(5, 1, 3, 16000, 'FALSE'); 
                     insert into Seats(PostId, PostCount, CabinetId, Salary, IsDeleted) values(6, 1, 3, 13000, 'FALSE'); 
                  ");*/
            appDbContext.Seats.AddRange(
                new Seats() { PostId = 2, PostCount = 9, CabinetId = 1, Salary = 80000m },
                new Seats() { PostId = 1, PostCount = 1, CabinetId = 2, Salary = 15000m },
                new Seats() { PostId = 3, PostCount = 1, CabinetId = 4, Salary = 18000m },
                new Seats() { PostId = 4, PostCount = 1, CabinetId = 5, Salary = 25000m },
                new Seats() { PostId = 5, PostCount = 1, CabinetId = 3, Salary = 16000m },
                new Seats() { PostId = 6, PostCount = 1, CabinetId = 3, Salary = 13000m }
                ) ;
            appDbContext.SaveChanges();
        }
        public static void ValuesForAppointments(ApplicationDbContext appDbContext)
        {
            /*appDbContext
           .Database
           .ExecuteSqlRaw
                (@"
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(1, 10000, '2020-01-02', null, null, 1, 'FALSE');
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(2, 25000, '2012-10-11', null, null, 4, 'FALSE');
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(11, 18000, '2012-10-11', null, null, 3, 'FALSE');
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(12, 13000, '2021-10-11', null, null, 6, 'FALSE');
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(13, 16000, '2021-10-11', null, null, 5, 'FALSE');
                    insert into Appointments(PersonId, Stake, InvokationDate, DeletionDate, DismissalReason, SeatsId, IsDeleted) values(14, 15000, '2020-01-21', null, null, 2, 'FALSE');
                ");*/
            appDbContext.Appointments.AddRange(
                new Appointments() { PersonId = 1, Stake  = 10000m, InvokationDate = DateTime.Parse("2020-01-02"), SeatsID = 1 },
                new Appointments() { PersonId = 2, Stake = 25000m, InvokationDate = DateTime.Parse("2012-10-11"), SeatsID = 4 },
                new Appointments() { PersonId = 11, Stake = 18000m, InvokationDate = DateTime.Parse("2012-10-11"), SeatsID = 3 },
                new Appointments() { PersonId = 12, Stake = 13000m, InvokationDate = DateTime.Parse("2012-10-11"), SeatsID = 6 },
                new Appointments() { PersonId = 13, Stake = 16000m, InvokationDate = DateTime.Parse("2012-10-11"), SeatsID = 5 },
                new Appointments() { PersonId = 14, Stake = 15000m, InvokationDate = DateTime.Parse("2020-01-21"), SeatsID = 2 }
                );
            appDbContext.SaveChanges();
        }
    }
}
