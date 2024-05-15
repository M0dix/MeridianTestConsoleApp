/* 
Таблица 1
Teachers (ID, Name, Age)

Таблица 2
Students (ID, Name, TeacherID)

Таблица 3
Lessons (ID, Name)

Таблица 4
Exams (StudentID, LessonID, Date, Score)
*/


-- 1.  Сколько учеников у каждого учителя. Сортировать по количеству учеников 
-- от меньшего

SELECT t.ID, t.Name, COUNT(s.ID) as StudentsCount
FROM Teachers t
JOIN Students s ON t.ID = s.TeacherID
GROUP BY t.ID, t.Name
ORDER BY StudentsCount ASC;

-- 2.  Найти ученика, у которого максимальный бал по Математике с 01.01.2021 
-- по 01.01.2022, не брать учителей, у которых возраст старше 40.

SELECT s.ID, s.Name, MAX(e.Score) as MaxScore
FROM Students s
JOIN Teachers t ON s.TeacherID = t.ID
JOIN Exams e ON s.ID = e.StudentID
JOIN Lessons l ON e.LessonID = l.ID
WHERE t.Age <= 40 AND l.Name = 'Математика' AND e.Date BETWEEN TO_DATE('01.01.2021', 'DD.MM.YYYY') AND TO_DATE('01.01.2022', 'DD.MM.YYYY')
GROUP BY s.ID, s.Name;

-- 3. Найти ученика, который третий по баллам по Математике с 01.01.2021 по 01.01.2022.

SELECT s.ID, s.Name, e.Score as MathScore
FROM Students s
JOIN Exams e ON s.ID = e.StudentID
JOIN Lessons l ON e.LessonID = l.ID
WHERE l.Name = 'Математика' AND e.Date BETWEEN TO_DATE('01.01.2021', 'DD.MM.YYYY') AND TO_DATE('01.01.2022', 'DD.MM.YYYY')
ORDER BY e.Score DESC
OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY;
