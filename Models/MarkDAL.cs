// 🔷 MarkDAL.cs
using System.Collections.Generic;
using System.Linq;
using SchoolManagementSystem.Models;

public class MarkDAL
{
    SchoolDBDataContext db;

    public MarkDAL(string connectionString)
    {
        db = new SchoolDBDataContext(connectionString);
    }

    // Get all published marks using LINQ query syntax  
    public List<Mark> GetAllMarksForStudent(int studentId)
    {
        var marks = from m in db.Marks
                    where m.StudentId == studentId && m.IsPublished == true
                    select m;
        return marks.ToList();
    }

    // Get unpublished marks using LINQ query syntax  
    public List<Mark> GetUnpublishedMarks()
    {
        var marks = from m in db.Marks
                    where m.IsPublished == false
                    select m;
        return marks.ToList();
    }

    // Publish all marks using LINQ query syntax  
    public void PublishMarks()
    {
        var unPublished = from m in db.Marks
                          where m.IsPublished == false
                          select m;
        foreach (var mark in unPublished)
        {
            mark.IsPublished = true;
        }
        db.SubmitChanges();
    }

    // Calculate total marks using LINQ query syntax  
    public int GetTotalMarks(int studentId)
    {
        var total = (from m in db.Marks
                     where m.StudentId == studentId && m.IsPublished == true
                     select m.Marks ?? 0).Sum();
        return total;
    }


    // Count subjects using LINQ query syntax  
    public int GetSubjectCountForStudent(int studentId)
    {
        var count = (from m in db.Marks
                     where m.StudentId == studentId && m.IsPublished == true
                     select m.SubjectId).Distinct().Count();
        return count;
    }
}