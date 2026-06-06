using System.Collections.Generic;
using System.Xml.Linq;
using Xunit;

public class ExamFixture
{
    public List<Group> Groups { get; }
    public List<Subject> Subjects { get; }
    public List<Student> Students { get; }
    public List<Exam> Exams { get; }

    public ExamFixture()
    {
        // Групи (назви спеціально не за алфавітом, щоб перевірити сортування)
        Groups = new List<Group>
        {
            new Group(2, "PMI-32"),
            new Group(1, "PMI-31")
        };

        // Предмети
        Subjects = new List<Subject>
        {
            new Subject(1, "C#", 15),
            new Subject(2, "SQL", 5)
        };

        // Студенти (спеціально не за алфавітом)
        Students = new List<Student>
        {
            new Student(1, "Zeta", 1),   // Група 1 (PMI-31)
            new Student(2, "Alpha", 1),  // Група 1 (PMI-31)
            new Student(3, "Beta", 2)    // Група 2 (PMI-32)
        };

        // Іспити (перевіряємо крайові випадки!)
        Exams = new List<Exam>
        {
            // Студент 1 (Zeta): Успішно склав C#, але провалив SQL
            new Exam(1, 1, 90), // +15 кредитів
            new Exam(1, 2, 40), // 0 кредитів (менше 50 балів, фільтр має його відкинути)

            // Студент 2 (Alpha): Здав C# двічі (наприклад, перездача)
            new Exam(2, 1, 60), // +15 кредитів
            new Exam(2, 1, 75), // ДУБЛІКАТ! Кредити не мають додаватися двічі (сума = 15)

            // Студент 3 (Beta): Успішно здав SQL
            new Exam(3, 2, 85)  // +5 кредитів
        };
    }
}

public class ExamProcessorTests : IClassFixture<ExamFixture>
{
    private readonly ExamFixture _fixture;

    // xUnit автоматично передасть сюди дані з ExamFixture
    public ExamProcessorTests(ExamFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TaskA_ShouldReturnCorrectXml_WithFilteringAndDuplicates()
    {
        // 1. Arrange (Готуємо ОЧІКУВАНИЙ результат вручну)
        // Враховуємо сортування: спочатку PMI-31, потім PMI-32.
        // У PMI-31: спочатку Alpha, потім Zeta.
        var expectedXml = new XElement("TaskA",
            new XElement("Group", new XAttribute("Name", "PMI-31"),
                new XElement("Student", 
                    new XAttribute("Surname", "Alpha"), 
                    new XAttribute("Credits", 15) // Перевірка дублікатів: має бути 15, а не 30
                ),
                new XElement("Student", 
                    new XAttribute("Surname", "Zeta"), 
                    new XAttribute("Credits", 15) // Перевірка фільтру: SQL (40 балів) відкинуто
                )
            ),
            new XElement("Group", new XAttribute("Name", "PMI-32"),
                new XElement("Student", 
                    new XAttribute("Surname", "Beta"), 
                    new XAttribute("Credits", 5)
                )
            )
        );

        // 2. Act (Викликаємо наш метод)
        var actualXml = ExamProcessor.TaskA(
            _fixture.Students, 
            _fixture.Subjects, 
            _fixture.Groups, 
            _fixture.Exams
        );

        // 3. Assert (Перевіряємо чи дерева XML абсолютно ідентичні)
        Assert.True(XNode.DeepEquals(expectedXml, actualXml));
    }
}