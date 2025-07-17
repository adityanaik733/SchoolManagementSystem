// 🔷 TeacherController.cs
using SchoolManagementSystem.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using SchoolManagementSystem.Filters;

namespace SchoolManagementSystem.Controllers
{
    //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*", Location = OutputCacheLocation.None)]
    public class TeacherController : Controller
    {
        private TeacherDAL teacherDAL;
        private string connectionString = ConfigurationManager.ConnectionStrings["SchoolManagementDB1ConnectionString"].ConnectionString;

        public TeacherController()
        {
            teacherDAL = new TeacherDAL(connectionString);
        }

        // Teacher Login (GET)
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string userId, string password)
        {
            var teacher = teacherDAL.IsValidTeacher(userId, password);
            if (teacher != null)
            {
                Session["TeacherId"] = teacher.TeacherId;
                Session["TeacherName"] = teacher.FullName;
                Session["AssignedClass"] = teacher.AssignedClass;
                return RedirectToAction("Dashboard");
            }

            ViewBag.Message = "Invalid credentials!";
            return View();
        }

        // Dashboard
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult Dashboard()
        {
            ViewBag.TeacherName = Session["TeacherName"];
            return View();
        }

        // View Students of Assigned Class
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult StudentList()
        {
            int classNo = Convert.ToInt32(Session["AssignedClass"]);
            var students = teacherDAL.GetStudentsByClass(classNo);
            return View(students);
        }

        // Enter Marks
        [HttpGet]
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult EnterMarks()
        {

            int assignedClass = Convert.ToInt32(Session["AssignedClass"]);

            var students = teacherDAL.GetStudentsByClass(assignedClass);
            var subjects = teacherDAL.GetAllSubjects(); // Already distinct from DAL

            ViewBag.Students = new SelectList(students, "StudentId", "FullName");
            ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName");

            return View();
        }
        [HttpPost]
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult EnterMarks(Mark mark)
        {

            mark.TeacherId = Convert.ToInt32(Session["TeacherId"]);
            mark.DateGiven = DateTime.Now;
            mark.IsPublished = false;

            teacherDAL.EnterMarks(mark);
            TempData["Success"] = "Marks entered successfully!";
            return RedirectToAction("EnterMarks");
        }


        // View Marks Given By This Teacher
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult ViewMarks()
        {
            int teacherId = Convert.ToInt32(Session["TeacherId"]);
            var marks = teacherDAL.GetMarksByTeacher(teacherId);
            return View(marks);
        }

        // Edit Marks (GET)
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult EditMarks(int id)
        {

            int teacherId = Convert.ToInt32(Session["TeacherId"]);
            var marks = teacherDAL.GetMarksByTeacher(teacherId);
            var mark = marks.FirstOrDefault(m => m.MarkId == id);

            var subjects = teacherDAL.GetAllSubjects();
            ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", mark.SubjectId);

            return View(mark);
        }

        // Edit Marks (POST)
        [HttpPost]
        [TeacherAuthFilter] // Apply filter to the whole controller
        public ActionResult EditMarks(Mark updatedMark)
        {
            teacherDAL.UpdateMarks(updatedMark);
            TempData["Success"] = "Marks updated successfully!";
            return RedirectToAction("ViewMarks");
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
