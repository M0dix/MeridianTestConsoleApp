using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static Program;

public class Program
{   
    public static async Task Main()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string teacherFilePath = Path.Combine(basePath, "Data", "Учителя.txt");
        string studentsFilePath = Path.Combine(basePath, "Data", "Ученики.txt");

        //Получаем учителей из файла
        var teachers = await GetTeachersFromTxtAsync(teacherFilePath); // Заполнить из файла Учитиля.txt
        //Получаем студентов из файла
        var students = await GetStudentsFromTxtAsync(studentsFilePath); // Заполнить из файла Ученики.txt

        var exams = new List<Exams>(); // Допустим что записи есть уже, можно не заполнять
        
        //1. Исправить ошибки если есть.
        //  1) Отсутствие директивы для List<>, в зависимости от версии .NET, класс List<> может быть не найден
        //  2) Значения перечисления для предметов, оба равны 1 
        
        //2. Найти учителя у которого в классе меньше всего учеников 
        //  У нас нет классов, значит будем считать через экзамены
        var teacherWithMaxExams = exams
            .GroupBy(e => e.TeacherId) // Создаем группы по учителям
            .OrderBy(g => g.Count()) // Сортируем по возрастанию количества экзаменов у учителей
            .Select(g => g.First().TeacherId) // Выбираем id учителей
            .FirstOrDefault(); // Берём первый id в нужной нам группе 
        
        //3. Найти средний бал экзамена по Физике за 2023 год.		
        var averagePhysicsScoreIn2023 = exams
            .Where(e => e.Lesson == LessonType.Physics && e.ExamDate.Year == 2023) // Находим экзамены по физике за 2023 год
            .Average(e => e.Score); // Находим среднее значение
        
        //4. Получить количество учеников которые по экзамену Математики получили больше 90 баллов, где учитель Alex 
        var studentsWithMathScoreMoreThan90Count = exams
            .Where(e => e.Lesson == LessonType.Mathematics && e.Teacher.Name == "Alex" && e.Score > 90) // Находим экзамены по математике учителя Alex с оценкой больше 90 баллов
            .Count(); // Считаем количество элементов группы
        
        //5. Найти учителя у который второй по количеству учеников
        var teacherWith = exams
            .GroupBy(e => e.TeacherId) // Создаем группы по учителям
            .Select(g => new { TeacherId = g.Key, ExamsCount = g.Count() }) // Выбираем ID учителя и считаем количество его экзаменов
            .OrderByDescending(t => t.ExamsCount) // Сортируем полученний список учителей по убыванию количества экзаменов
            .Skip(1) // Пропускаем первого учителя
            .FirstOrDefault(); // Выбираем второго учителя
    }

    public static async Task<List<Teacher>> GetTeachersFromTxtAsync(string filePath)
    {
        var teachers = new List<Teacher>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            //Не учитываем строку с заголовком
            await reader.ReadLineAsync();

            //Идём до отсутствия строк, обрабатываем построчно
            while (!reader.EndOfStream)
            {
                // "сплитим" строку по одинарной и двойной табуляции, используя регулярное выражение 
                string[] parts = Regex.Split(await reader.ReadLineAsync(), "\t+");

                //Создаем класс учителя
                Teacher teacher = new Teacher
                {
                    Name = parts[0],
                    LastName = parts[1],
                    Age = int.Parse(parts[2]),
                    Lesson = GetLessonTypeByString(parts[3])
                };

                teachers.Add(teacher);
            }
        }

        return teachers;
    }

    public static LessonType GetLessonTypeByString(string lessonString)
    {
        var lessonDictionary = new Dictionary<string, LessonType>()
        {
            {"Математика", LessonType.Mathematics },
            {"Физика", LessonType.Physics }
        };

        return lessonDictionary[lessonString];
    }

    public static async Task<List<Student>> GetStudentsFromTxtAsync(string filePath)
    {
        var students = new List<Student>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            //Не учитываем строку с заголовком
            await reader.ReadLineAsync();

            //Идём до отсутствия строк, обрабатываем построчно
            while (!reader.EndOfStream)
            {
                // "сплитим" строку по одинарной и двойной табуляции, используя регулярное выражение 
                string[] parts = Regex.Split(await reader.ReadLineAsync(), "\t+");

                //Создаем класс студента
                Student student = new Student
                {
                    Name = parts[0],
                    LastName = parts[1],
                    Age = int.Parse(parts[2]),
                };

                students.Add(student);
            }
        }

        return students;
    }

    public class Person
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class Teacher : Person
    {
        public LessonType Lesson { get; set; }
    }

    public class Student : Person
    {

    }

    public class Exams
    {
        public LessonType Lesson { get; set; }

        public long StudentId { get; set; }
        public long TeacherId { get; set; }

        public decimal Score { get; set; }
        public DateTime ExamDate { get; set; }

        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
    }

    public enum LessonType
    {
        Mathematics = 1,
        Physics = 2
    }
}