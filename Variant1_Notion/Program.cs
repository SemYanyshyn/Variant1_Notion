using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

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

public class Group
{
    public int Id {get;}
    public string  GroupName{get;}

    public Group(int id, string groupName)
    {
        Id = id;
        GroupName = groupName;
    }
}

public class Exam
{
   public int StudentId {get;}
    public int SubjectId {get;}
    public int Score {get;}

    public Exam(int studentId, int subjectId, int score)
    {
        StudentId = studentId;
        SubjectId = subjectId;
        Score = score;
    }
}

public static class ExamProcessor
{
    public static XElement TaskA(
        IEnumerable<Student> students,
        IEnumerable<Subject> subjects,
        IEnumerable<Group> groups,
        IEnumerable<Exam> exams
    )
    {
    var flatData = from exam in exams
        where exam.Score > 50
        select new
        {
            exam.StudentId,
            exam.SubjectId
        };

    var query = from groupItem in groups
        orderby groupItem.GroupName
        select new XElement ("Group",
            new XAttribute ("Name", groupItem.GroupName),
        from student in students
        where student.GroupId == groupItem.Id
        orderby student.Surname
        select new XElement ("Student",
            new XAttribute("Surname", student.Surname),
            new XAttribute("Credits", 
            (from item in flatData
            where item.StudentId == student.Id
            group item by item.SubjectId into SubjectData
            join subject in subjects on SubjectData.Key equals subject.Id
            select subject.Credits).Sum()
            )
        )
        
    );
        return new XElement ("TaskA", query);
}}

public static class DataLoader
{
    public static IEnumerable<Student> StudentLoader(string path)
    {
        XDocument document = XDocument.Load(path);

        var students =
            from student in document.Root.Elements("Student")
            select new Student(
                (int)student.Element("Id"),
                (string)student.Element("Surname"),
                (int)student.Element("GroupId")
            );

        return students;

    }

    public static IEnumerable<Group> GroupLoader(string path)
    {
        XDocument document = XDocument.Load(path);

        var groups = 
        from groupItem in document.Root.Elements("Group")
        select new Group(
            (int)groupItem.Element("Id"),
            (string)groupItem.Element("GroupName")
        );
        return groups;
    }    


    public static IEnumerable<Subject> SubjectLoader(string path)
    {
        XDocument document = XDocument.Load(path);
        
        var subjects = 
        from subject in document.Root.Elements("Subject")
        select new Subject(
            (int)subject.Element("Id"),
            (string)subject.Element("Name"),
            (int)subject.Element("Credits")
        );
        return subjects;
    }

    public static IEnumerable<Exam> ExamLoader(IEnumerable<string> paths)
    {
        var exams = 
            from path in paths
            let document = XDocument.Load(path)
            from exam in document.Root.Elements("Exam")
            select new Exam(
                (int)exam.Element("StudentId"),
                (int)exam.Element("SubjectId"),
                (int)exam.Element("Score")
            );
        return exams;
    }
}

public class Program
{
    public static void Main()
    {
        // AppContext.BaseDirectory — це папка, звідки запускається програма [bin/Debug/net10.0/]
        string dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Variant1_Notion", "Data");
        string studentsPath = Path.Combine(dataDirectory, "students.xml"); // Це створює повний шлях до файлу students.xml.
        string groupsPath = Path.Combine(dataDirectory, "groups.xml");
        string subjectsPath = Path.Combine(dataDirectory, "subjects.xml");

// Програма запускається.
// AppContext.BaseDirectory визначає папку запуску.
// Path.Combine створює повні шляхи до XML-файлів.
// Ці шляхи передаються в DataLoader.
// DataLoader відкриває XML-файли.
// XML-дані перетворюються в об’єкти Student, Group, Subject.

        IEnumerable<string> examPaths = new string[] // examPaths - Це колекція рядків. new string[] - створює список  "шлях до exam1.xml","шлях до exam2.xml".
        {
            Path.Combine(dataDirectory, "exam1.xml"), // Склеює папку запуску програми з назвою файлу
            Path.Combine(dataDirectory, "exam2.xml")
        };

        // Ці рядки викликають методи з DataLoader і зчитують дані з XML-файлів і записують значення у C# об’єкти.
        IEnumerable<Student> students = DataLoader.StudentLoader(studentsPath);
        IEnumerable<Group> groups = DataLoader.GroupLoader(groupsPath);
        IEnumerable<Subject> subjects = DataLoader.SubjectLoader(subjectsPath);
        IEnumerable<Exam> exams = DataLoader.ExamLoader(examPaths);

        XElement result = ExamProcessor.TaskA( // передаю дані методу TaskA і він викликається 
            students,
            subjects,
            groups,
            exams
        );

        string resultPath = Path.Combine(dataDirectory, "taskAResult.xml"); // зберігає результат у XML-файл і показує його в консолі

        new XDocument(result).Save(resultPath);

        Console.WriteLine(result);
    }
}











 


























// public static class DataLoader // Тобто виходить не XML, а колекції об’єктів
// {
//     // string path - це шлях до файлу
//     public static IEnumerable<Student> LoadStudents(string path) // LoadStudents - Цей метод зчитує студентів з XML-файлу і перетворює їх у список об’єктів Student
//     {
//         XDocument document = XDocument.Load(path); //відкриває файл і перетворює звичайний текст на структуроване XML-дерево

//         var students =
//             from student in document.Root.Elements("Student") // document.Root - це головний тег; Elements("Student") — бере всі: <Student>...</Student>
//             select new Student(
//                 (int)student.Element("Id"), // Бере значення з ID і перетворює в int
//                 (string)student.Element("Surname"),
//                 (int)student.Element("GroupId")
//             );

//         return students; // Повертає всіх студентів як IEnumerable<Student>. [new Student(1, "Demus", 1)]
//     }

//     public static IEnumerable<Group> LoadGroups(string path)
//     {
//         XDocument document = XDocument.Load(path);

//         var groups =
//             from groupItem in document.Root.Elements("Group")
//             select new Group(
//                 (int)groupItem.Element("Id"),
//                 (string)groupItem.Element("GroupName")
//             );

//         return groups;
//     }

//     public static IEnumerable<Subject> LoadSubjects(string path)
//     {
//         XDocument document = XDocument.Load(path);

//         var subjects =
//             from subject in document.Root.Elements("Subject")
//             select new Subject(
//                 (int)subject.Element("Id"),
//                 (string)subject.Element("Name"),
//                 (int)subject.Element("Credits")
//             );

//         return subjects;
//     }

//     public static IEnumerable<Exam> LoadExams(IEnumerable<string> paths)
//     {
//         var exams =
//             from path in paths
//             from exam in XDocument.Load(path).Root.Elements("Exam")
//             select new Exam(
//                 (int)exam.Element("StudentId"),
//                 (int)exam.Element("SubjectId"),
//                 (int)exam.Element("Score")
//             );

//         return exams;
//     }
// }

// public class Program
// {
//     public static void Main()
//     {
//         string dataDirectory = AppContext.BaseDirectory;

//         string studentsPath = Path.Combine(dataDirectory, "students.xml");
//         string groupsPath = Path.Combine(dataDirectory, "groups.xml");
//         string subjectsPath = Path.Combine(dataDirectory, "subjects.xml");

//         IEnumerable<string> examPaths = new string[]
//         {
//             Path.Combine(dataDirectory, "exam1.xml"),
//             Path.Combine(dataDirectory, "exam2.xml")
//         };

//         IEnumerable<Student> students = DataLoader.LoadStudents(studentsPath);
//         IEnumerable<Group> groups = DataLoader.LoadGroups(groupsPath);
//         IEnumerable<Subject> subjects = DataLoader.LoadSubjects(subjectsPath);
//         IEnumerable<Exam> exams = DataLoader.LoadExams(examPaths);

//         XElement result = ExamProcessor.TaskA(
//             students,
//             subjects,
//             groups,
//             exams
//         );

//         string resultPath = Path.Combine(dataDirectory, "taskAResult.xml");

//         new XDocument(result).Save(resultPath);

//         Console.WriteLine(result);
//     }
// }
