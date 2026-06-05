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

