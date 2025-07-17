using System.Collections.Generic;
using System.Linq;
using SchoolManagementSystem.Models;

public class StudentDAL
{
    private SchoolDBDataContext db;

    public StudentDAL(string connectionString)
    {
        db = new SchoolDBDataContext(connectionString);
    }

    // Validate Student Login
    public Student IsValidStudent(string userId, string password)
    {
        return db.Students.FirstOrDefault(s => s.UserId == userId && s.Password == password);
    }

    // Get Marks (Only Published)
    public List<Mark> GetMarksForStudent(int studentId)
    {
        return db.Marks.Where(m => m.StudentId == studentId && (m.IsPublished ?? false)).ToList();
    }

    // Get Student Profile
    public Student GetStudentDetails(int studentId)
    {
        return db.Students.FirstOrDefault(s => s.StudentId == studentId);
    }

    // Get Total Marks
    public int GetTotalMarks(int studentId)
    {
            return db.Marks
                 .Where(m => m.StudentId == studentId && (m.IsPublished ?? false))
                 .Sum(m => m.Marks ?? 0);
    }

    // Get Percentage (based on total subjects)
    public double GetPercentage(int studentId)
    {
        int totalMarks = GetTotalMarks(studentId);

        int subjectCount = db.Marks
                             .Where(m => m.StudentId == studentId && (m.IsPublished ?? false))
                             .Select(m => m.SubjectId)
                             .Distinct()
                             .Count();

        if (subjectCount == 0)
            return 0;

        return (double)totalMarks / (subjectCount * 100) * 100;
    }

}
