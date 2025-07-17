// 🔷 AdminDAL.cs
using System.Collections.Generic;
using System.Linq;
using SchoolManagementSystem.Models;

public class AdminDAL
{
    SchoolDBDataContext db;

    public AdminDAL(string connectionString)
    {
        db = new SchoolDBDataContext(connectionString);
    }

    // Check admin login using LINQ query syntax
    public bool IsValidAdmin(string username, string password)
    {
        var query = from a in db.Admins
                    where a.Username == username && a.Password == password
                    select a;
        return query.Any();
    }

    // Add student to DB
    public void AddStudent(Student student)
    {
        db.Students.InsertOnSubmit(student);
        db.SubmitChanges();
    }

    public Student GetStudentById(int StudentId)
    {
        var student = (from a in db.Students where a.StudentId == StudentId select a).Single();
        return student;
    }

    public void UpdateStudent(Student NewValue)
    {
        Student OldValue = db.Students.Single(S => S.StudentId == NewValue.StudentId);
        OldValue.StudentId = NewValue.StudentId;
        OldValue.FullName = NewValue.FullName;
        OldValue.UserId = NewValue.UserId;
        OldValue.Password = NewValue.Password;
        OldValue.Class = NewValue.Class;
        db.SubmitChanges();
    }

    public void DeleteStudent(int studentId)
    {
        // 1) Delete related marks first
        var marksToDelete = db.Marks.Where(m => m.StudentId == studentId);
        db.Marks.DeleteAllOnSubmit(marksToDelete);

        // 2) Delete the student record
        var student = db.Students.FirstOrDefault(s => s.StudentId == studentId);
        if (student != null)
        {
            db.Students.DeleteOnSubmit(student);
        }

        // 3) Submit all deletions together
        db.SubmitChanges();

        // 4) Reseed the identity
        int newSeed = db.Students.Any()
            ? db.Students.Max(s => s.StudentId)
            : 0;

        db.ExecuteCommand($"DBCC CHECKIDENT ('Students', RESEED, {newSeed})");
    }


    // Add teacher to DB
    public void AddTeacher(Teacher teacher)
    {
        db.Teachers.InsertOnSubmit(teacher);
        db.SubmitChanges();
    }

    public Teacher GetTeacherById(int TeacherId)
    {
        var teacher = (from t in db.Teachers where t.TeacherId == TeacherId select t).Single();
        return teacher;
    }

    public void UpdateTeacher(Teacher newteacher)
    {
        Teacher oldteacher = db.Teachers.Single(T => T.TeacherId == newteacher.TeacherId);
        oldteacher.TeacherId = newteacher.TeacherId;
        oldteacher.FullName = newteacher.FullName;
        oldteacher.UserId = newteacher.UserId;
        oldteacher.Password = newteacher.Password;
        oldteacher.AssignedClass = newteacher.AssignedClass;
        db.SubmitChanges();

    }
    public void DeleteTeacher(int teacherId)
    {
        // 1) Delete the teacher record
        var teacher = db.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);
        if (teacher != null)
        {
            db.Teachers.DeleteOnSubmit(teacher);
            db.SubmitChanges();
        }

        // 2) Reseed the identity so the next insert fills the gap
        //    Compute the current max TeacherId, or 0 if the table is now empty
        int newSeed = db.Teachers.Any()
            ? db.Teachers.Max(t => t.TeacherId)
            : 0;

        //    Execute the DBCC CHECKIDENT command 
        //    Note: Table name must match your database table
        db.ExecuteCommand($"DBCC CHECKIDENT ('Teachers', RESEED, {newSeed})");
    }


    // View all students using LINQ query syntax
    public List<Student> GetAllStudents()
    {
        var students = from s in db.Students
                       select s;
        return students.ToList();
    }

    // View all teachers using LINQ query syntax
    public List<Teacher> GetAllTeachers()
    {
        var teachers = from t in db.Teachers
                       select t;
        return teachers.ToList();
    }

    // Assign a class to a teacher using LINQ query syntax
    public void AssignTeacherToClass(int teacherId, int classNo)
    {
        var teacher = (from t in db.Teachers
                       where t.TeacherId == teacherId
                       select t).FirstOrDefault();
        if (teacher != null)
        {
            teacher.AssignedClass = classNo;
            db.SubmitChanges();
        }
    }
}