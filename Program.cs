// пиши коментар без води де треба 
using System; // це необхідно для використання базових типів даних та функцій, таких як Console, AppContext, etc.
using System.Collections.Generic; // це необхідно для використання колекцій, таких як IEnumerable<T>, List<T>, etc.
using System.IO; // це необхідно для роботи з файловою системою, наприклад, для побудови шляхів до файлів та збереження результатів.
using System.Linq; // це необхідно для використання LINQ-запитів, які дозволяють ефективно обробляти колекції даних.
using System.Text.RegularExpressions;
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
        
        var query = from item in flatData
        where item.StudentId == 1
        group item by item.SubjectId into SubjectData // отримали список всіх предметів одного студента SubjectData.Key = 2 
        //всередині групи всі записи з SubjectId = 2
        //SubjectData.Key = 3
        //всередині групи всі записи з SubjectId = 3
        join subject in subjects on SubjectData.Key equals subject.Id
    }
    
} 


    // var queryForOneStudent = 
    //     from item in flatData                 // беремо наші відфільтровані екзамени
    //     where item.StudentId == 1             // 1. Беремо тільки студента №1 (Шербана)
    //     group item by item.SubjectId into g   // 2. Збираємо його екзамени по предметах
    //     join subject in subjects on g.Key equals subject.Id // 3. Шукаємо ці предмети в довіднику
    //     select subject.Credits;