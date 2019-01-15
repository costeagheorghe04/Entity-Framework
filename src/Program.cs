using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EFtutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            // AddUpdateDelete();
            //  InsertdeleteupdateDisconected();
            // LinqToEntities();
            // FindEntiti();
            // LazyLoad();
            // ExplicitLoad();
            //SqlRawQuery();
            // SqlComand();
            // DynamicProx();
            DisplayTracker();
            Console.ReadLine();
        }

        public static void AddUpdateDelete()
        {
            Console.WriteLine("addupdatedelete");

            using (var context = new SchoolDbEntities())
            {

                context.Database.Log = Console.WriteLine;
                context.SaveChanges();

                var newStudent = context.Students.Add(new Student() { StudentName = "Jonathan", StudentAddress = new StudentAddress() { Address1 = "1, Harbourside", City = "Jersey City", State = "NJ" } });
                context.SaveChanges();


                var student2 = context.Students.Add(new Student() { StudentName = "exer", StudentAddress = new StudentAddress() { Address1 = "dorobanti", City = "oradaea", State = "dm" } });
                context.SaveChanges();

                context.Students.First<Student>();
                context.Students.Remove(newStudent);
                context.SaveChanges();
                student2.StudentName = "blabla";
                context.SaveChanges();


            }

        }
        public static void InsertdeleteupdateDisconected()
        {
            var newStudent = new Student() { StudentName = "ion" };
            var existingStudent = new Student { StudentID = 1, StudentName = "dorel" };
            using (var context = new SchoolDbEntities())
            {
                context.Database.Log = Console.WriteLine;
                context.Entry(newStudent).State = newStudent.StudentID == 0 ? EntityState.Added : EntityState.Modified;
                context.Entry(existingStudent).State = existingStudent.StudentID == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
            }
        }


        public static void LinqToEntities()
        {

            using (var context = new SchoolDbEntities())
            {
                context.Database.Log = Console.WriteLine;

                var students = (from s in context.Students
                                where s.StudentName == "ion"
                                select s).ToList();

                var newStudent = (from s in context.Students
                                  where s.StudentID == 1
                                  select s).ToList();

                var studentSameName = context.Students
                                      .GroupBy(s => s.StudentName)
                                      .Where(g => g.Count() > 1)
                                      .Select(g => g.Key);
                Console.WriteLine("studen same name");
                foreach (var stud in studentSameName)
                {
                    Console.WriteLine(stud);
                }

            }
            
        }
        public static void FindEntiti()
        {
            Console.WriteLine("find");
            using (var context = new SchoolDbEntities())
            {
                context.Database.Log = Console.WriteLine;

                var stud = context.Students.Find(1);
                Console.WriteLine(stud.StudentName + " found");

                var test = context.GetCoursesByStudentId(1);

            }
        }
        public static void LazyLoad() {

            Console.WriteLine("lazi load");
            using (var context = new SchoolDbEntities())
            {
                context.Database.Log = Console.WriteLine;
                Student student = context.Students.Where(s => s.StudentID == 1).FirstOrDefault<Student>();
                //retrieve data
                Standard std = student.Standard;
            }
            Console.WriteLine("lazi load ending");
        }

        public static void ExplicitLoad()
        {
            Console.WriteLine("explicit load");
            using (var context = new SchoolDbEntities())
            {
                Student std = context.Students
                              .Where(s => s.StudentID == 1)
                               .FirstOrDefault<Student>();
                context.Entry(std).Reference(s => s.Standard).Load();
                context.Entry(std).Collection(s => s.Courses).Load();
            }
            Console.WriteLine("explicit load end");
        }

        public static void SqlRawQuery()
        {
            Console.WriteLine("slq raw");
            using (var context = new SchoolDbEntities())
            {
                var studentList = context.Students.SqlQuery("Select * from Student").ToList<Student>();
                var student = context.Students.SqlQuery("Select StudentId, StudentName, StandardId, RowVersion from Student where StudentId=1").ToList();
                foreach (var s in student)
                {
                    Console.WriteLine(s.StudentID);
                }
            }
            
        }
        public static void SqlComand()
        {
            Console.WriteLine("slq comm");
            using (var context = new SchoolDbEntities())
            {
                context.Database.Log = Console.WriteLine;

                int noOfRowInsert = context.Database.ExecuteSqlCommand("insert into student(studentname) values('Robert')");
                int noOfRowUpdate = context.Database.ExecuteSqlCommand("Update student set studentname='Mark' where studentname='Robert' ");
                int noOfRowDelete = context.Database.ExecuteSqlCommand("delete from student where studentname ='Mark'");
            }
        }
        public static void DynamicProx()
        {
            Console.WriteLine("dinamic prox");
            using (var context = new SchoolDbEntities())
            {
                var stud = context.Students.Where(s => s.StudentName == "Bill")
                                    .FirstOrDefault<Student>();

                Console.WriteLine("Proxy Type: {0}", stud.GetType().Name);
                Console.WriteLine("Underlying Entity Type : {0}", stud.GetType().BaseType);

                context.Configuration.ProxyCreationEnabled = false;
                Console.WriteLine("prox disable");

                var student = context.Students.Where(s => s.StudentName == "Steve ")
                                            .FirstOrDefault<Student>();
                Console.WriteLine("Entity Type : {0}", student.GetType().Name);
            }
        }
        public static void DisplayTracker()
        {
            Console.WriteLine("traker");
            using (var context = new SchoolDbEntities())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var student = context.Students.Add(new Student() { StudentName = "Andrei" });
                DisplayTrackedEntities(context);

                Console.WriteLine("retrieve student ");
                var existingstudent = context.Students.Find(1);
                DisplayTrackedEntities(context);

            }
        }
        private static void DisplayTrackedEntities(SchoolDbEntities context)
        {
            
            Console.WriteLine("Context is tracking {0} entities.", context.ChangeTracker.Entries().Count());
            DbChangeTracker changeTracker = context.ChangeTracker;
            var entries = changeTracker.Entries();
            foreach (var entry in entries)
            {
                Console.WriteLine("Entity Name: {0}", entry.Entity.GetType().FullName);
                Console.WriteLine("Status: {0}", entry.State);
            }
        }
    }
}
