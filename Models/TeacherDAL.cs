// 🔷 TeacherDAL.cs
using System.Collections.Generic;
using System.Linq;
using SchoolManagementSystem.Models;

public class TeacherDAL
{
    SchoolDBDataContext db;

    public TeacherDAL(string connectionString)
    {
        db = new SchoolDBDataContext(connectionString);
    }

    // Teacher login using LINQ query syntax
    public Teacher IsValidTeacher(string userId, string password)
    {
        var teacher = (from t in db.Teachers
                       where t.UserId == userId && t.Password == password
                       select t).FirstOrDefault();
        return teacher;
    }

    // Get students in assigned class using LINQ query syntax
    public List<Student> GetStudentsByClass(int classNo)
    {
        var students = from s in db.Students
                       where s.Class == classNo
                       select s;
        return students.ToList();
    }

    // Get all subjects using LINQ query syntax
    // TeacherDAL.cs
    public List<Subject> GetAllSubjects()
    {
        var distinctSubjects = db.Subjects
            .GroupBy(s => s.SubjectName) // Group by subject name instead of SubjectId
            .Select(g => g.First())      // Take one from each group
            .ToList();

        return distinctSubjects;
    }

    // Add marks
    public void EnterMarks(Mark mark)
    {
        db.Marks.InsertOnSubmit(mark);
        db.SubmitChanges();
    }

    // Update marks using LINQ query syntax
    public void UpdateMarks(Mark updatedMark)
    {
        var existing = (from m in db.Marks
                        where m.MarkId == updatedMark.MarkId
                        select m).FirstOrDefault();
        if (existing != null)
        {
            existing.Marks = updatedMark.Marks;
            existing.SubjectId = updatedMark.SubjectId;
            db.SubmitChanges();
        }
    }

    // View marks given by this teacher using LINQ query syntax
    public List<Mark> GetMarksByTeacher(int teacherId)
    {
        var marks = from m in db.Marks
                    where m.TeacherId == teacherId
                    select m;
        return marks.ToList();
    }
}