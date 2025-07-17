using System;
using System.Configuration;
using System.Web.Mvc;
using Rotativa;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Filters;

public class StudentController : Controller
{
    private StudentDAL studentDAL;
    string connectionString = ConfigurationManager.ConnectionStrings["SchoolManagementDB1ConnectionString"].ConnectionString;

    public StudentController()
    {
        studentDAL = new StudentDAL(connectionString);
    }

    // Login Page (GET)
    public ActionResult Login()
    {
        return View();
    }
    [HttpPost]
    
    public ActionResult Login(string userId, string password)
    {
        var student = studentDAL.IsValidStudent(userId, password);
        if (student != null)
        {
            Session["StudentId"] = student.StudentId;
            Session["StudentName"] = student.FullName;
            return RedirectToAction("Dashboard");
        }

        ViewBag.Message = "Invalid credentials.";
        return View();
    }

    // Dashboard (Protected)
    [StudentAuthFilter] // Apply filter to the whole controller
    public ActionResult Dashboard()
    {
        int studentId = Convert.ToInt32(Session["StudentId"]);
        var marks = studentDAL.GetMarksForStudent(studentId);
        var student = studentDAL.GetStudentDetails(studentId);

        ViewBag.Student = student;
        ViewBag.Total = studentDAL.GetTotalMarks(studentId);
        ViewBag.Percentage = studentDAL.GetPercentage(studentId);

        return View(marks);
    }

    // Marksheet View (Protected)
    [StudentAuthFilter] // Apply filter to the whole controller
    public ActionResult Marksheet()
    {
        int studentId = Convert.ToInt32(Session["StudentId"]);
        var marks = studentDAL.GetMarksForStudent(studentId);
        var student = studentDAL.GetStudentDetails(studentId);

        ViewBag.Name = student.FullName;
        ViewBag.Total = studentDAL.GetTotalMarks(studentId);
        ViewBag.Percentage = studentDAL.GetPercentage(studentId);

        return View(marks);
    }

    // Download PDF (Protected)
    [StudentAuthFilter] // Apply filter to the whole controller
    public ActionResult DownloadPdf()
    {
        int studentId = Convert.ToInt32(Session["StudentId"]);
        var marks = studentDAL.GetMarksForStudent(studentId);
        var student = studentDAL.GetStudentDetails(studentId);

        ViewBag.Name = student.FullName;
        ViewBag.Total = studentDAL.GetTotalMarks(studentId);
        ViewBag.Percentage = studentDAL.GetPercentage(studentId);

        return new ViewAsPdf("MarksheetPDF", marks)
        {
            FileName = "Marksheet.pdf"
        };
    }

    // Logout  
    public ActionResult Logout()
    {
        Session.Abandon();
        return RedirectToAction("Login");
    }
}
