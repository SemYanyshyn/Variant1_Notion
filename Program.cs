// пиши коментар без води де треба 
using System; // це необхідно для використання базових типів даних та функцій, таких як Console, AppContext, etc.
using System.Collections.Generic; // це необхідно для використання колекцій, таких як IEnumerable<T>, List<T>, etc.
using System.IO; // це необхідно для роботи з файловою системою, наприклад, для побудови шляхів до файлів та збереження результатів.
using System.Linq; // це необхідно для використання LINQ-запитів, які дозволяють ефективно обробляти колекції даних.
using System.Xml.Linq; // це необхідно для роботи з XML-документами, що використовується для завантаження даних та створення результатів у форматі XML

public class Student
{
    public int Id {get;}
    public string Surname {get;}
    public int GroupId {get;}

    public Student(int id, string surname, int groupId)
    {
        Id = id;
        Surname = surname;
        GroupId = groupId;
    }

}

public class Group
{
    public int Id {get;}
    
    public string GroupName {get;}

    public Group(int id, string groupname)
    {
        Id = id;
        GroupName = groupname;
    }
}

public class Subject
{
    public int Id {get;}
    public string Name {get;}
    public int Credits {get;}

    public Subject(int id, string name, int credits)
    {
        Id = id;
        Name = name;
        Credits = credits;
    }
}


public class Exam
{
    public int StudentId{get;}
    public int SubjectId{get;}
    public int Score{get;}
    
    public Exam(int studentid, int subjectid, int score)
    {
        StudentId = studentid;
        SubjectId = subjectid;
        Score = score;
    }

}

public static class ExamProcessor
{
    public static XElement TaskA(
        IEnumerable<Student> students,
        IEnumerable<Group> groups,
        IEnumerable<Subject> subjects,
        IEnumerable<Exam> exams
    ){
        var flatData = from exam in exams
            where exam.Score > 50
            select new
            {
              exam.StudentId,
              exam.SubjectId
            };

        var query = from groupItem in groups
        orderby groupItem.GroupName 
        select new XElement("Group", 
            new  XAttribute("Name", groupItem.GroupName),
        from student in students
        where student.GroupId == groupItem.Id
        orderby student.Surname
        select new XElement("Student", 
            new XAttribute ("Surname", student.Surname),
            new XAttribute ("Credits", 
                  (from item in flatData
                  // where item.StudentId == 1 // Беремо тільки одного студента і р
                  where item.StudentId == student.Id
                  group item by item.SubjectId into SubjectData
                  //всередині групи всі записи з SubjectId = 2
                  //всередині групи всі записи з SubjectId = 3

                  // роблю цей join бо маючи студента знаю що він здав предмет з id 2 але не знаю що назву предмету і к-ть кредитів і Ця інформація є тільки в subjects
                  join subject in subjects on SubjectData.Key equals subject.Id
                  select subject.Credits).Sum()
                            )
                        )
                   );
        return new XElement ("TaskA", query);
    }}
        
        //    new XElement("Surname", patient.Surname), 
        
        // from item in flatData
        // // where item.StudentId == 1 // Беремо тільки одного студента
        // where item.StudentId == //student.Id
        // group item by item.SubjectId into SubjectData // отримали список всіх предметів одного студента SubjectData.Key = 2 
        // //всередині групи всі записи з SubjectId = 2
        // //SubjectData.Key = 3
        // //всередині групи всі записи з SubjectId = 3
        // join subject in subjects on SubjectData.Key equals subject.Id
        // select subject.Credits.Sum()
    
    


// from item in flatData
// where item.StudentId == student.Id // <-- Замість 1 підставляємо Id поточного студента
// group item by item.SubjectId into SubjectData 
// join subject in subjects on SubjectData.Key equals subject.Id
// select subject.Credits).Sum()

/////////////////////////////////////////////////

    // var queryForOneStudent = 
    //     from item in flatData                 // беремо наші відфільтровані екзамени
    //     where item.StudentId == 1             // 1. Беремо тільки студента №1 (Шербана)
    //     group item by item.SubjectId into g   // 2. Збираємо його екзамени по предметах
    //     join subject in subjects on g.Key equals subject.Id // 3. Шукаємо ці предмети в довіднику
    //     select subject.Credits;


    
// public static class DataLoader
// {
//     public static IEnumerable<Patient> LoadPatients(string path)
//     {
//         var query = from x in XDocument.Load(path).Descendants("Patient")
//                     select new Patient(
//                         (int)x.Element("Id")!,
//                         (string)x.Element("Surname")!,
//                         (int)x.Element("Department")!
//                     );

//         return query;
//     }

//     public static IEnumerable<Drug> LoadDrugs(string path)
//     {
//         var query = from x in XDocument.Load(path).Descendants("Medicine")
//                     select new Drug(
//                         (int)x.Element("Id")!,
//                         (string)x.Element("Name")!
//                     );

//         return query;
//     }

//     public static IEnumerable<Department> LoadDepartments(string path)
//     {
//         var query = from x in XDocument.Load(path).Descendants("Department")
//                     select new Department(
//                         (int)x.Element("Id")!,
//                         (string)x.Element("Name")!
//                     );

//         return query;
//     }

//     public static IEnumerable<Visit> LoadVisits(IEnumerable<string> paths)
//     {
//         var query = from path in paths
//                     from x in XDocument.Load(path).Descendants("Visit")
//                     select new Visit(
//                         (int)x.Element("PatientId")!,
//                         (int)x.Element("MedZasibId")!,
//                         (double)x.Element("DozePerOne")!,
//                         (int)x.Element("InOneDay")!,
//                         (int)x.Element("Days")!
//                     );

//         return query;
//     }
// }