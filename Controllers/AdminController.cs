// 🔷 AdminController.cs
using SchoolManagementSystem.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using SchoolManagementSystem.Filters;
using System.Collections.Generic;


namespace SchoolManagementSystem.Controllers
{
    
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*", Location = OutputCacheLocation.None)]
    public class AdminController : Controller
    {
        private AdminDAL adminDAL;
        private MarkDAL markDAL;
        string connectionString = ConfigurationManager.ConnectionStrings["SchoolManagementDB1ConnectionString"].ConnectionString;

        public AdminController()
        {
            adminDAL = new AdminDAL(connectionString);
            markDAL = new MarkDAL(connectionString);
        }

        // Admin Login (GET)
        public ActionResult Login()
        {
            return View();
        }

        // Admin Login (POST)
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (adminDAL.IsValidAdmin(username, password))
            {
                Session["Admin"] = username;
                return RedirectToAction("Dashboard");
            }
            ViewBag.Message = "Invalid username or password";
            return View();
        }

        // Admin Dashboard
        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult Dashboard()
        {
            return View();
        }

        // Add Student (GET)
        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult AddStudent()
        {
            return View();
        }
        [HttpPost]
        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult AddStudent(Student student)
        {
            adminDAL.AddStudent(student);
            ViewBag.Message = "Student added successfully";
            return View();
        }

        // Add Teacher (GET)
        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult AddTeacher()
        {
            return View();
        }
        [HttpPost]
        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult AddTeacher(Teacher teacher)
        {
            adminDAL.AddTeacher(teacher);
            ViewBag.Message = "Teacher added successfully";
            return View();
        }

        //  View All Students
        // 👨‍🎓 Step 1: Show all students on GET
        [AdminAuthFilter] // Apply filter to the whole controller
        public ViewResult StudentList()
        {
            List<Student> students = adminDAL.GetAllStudents();
            return View(students);
        }

        // 🔍 Step 2: Search on POST
        [HttpPost]
        public ViewResult SearchStudent(string SearchTerm)
        {
            List<Student> students;

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                students = adminDAL.GetAllStudents();
            }
            else
            {
                students = adminDAL.GetAllStudents()
                    .Where(s => s.FullName.ToLower().Contains(SearchTerm.ToLower()))
                    .ToList();
            }

            return View("StudentList", students);
        }

        // 🔁 Step 3: Autocomplete using jQuery UI
        [AdminAuthFilter] // Apply filter to the whole controller
        public JsonResult GetStudent(string term)
        {
            List<string> names = adminDAL.GetAllStudents()
                .Where(s => s.FullName.ToLower().StartsWith(term.ToLower()))
                .Select(s => s.FullName)
                .ToList();

            return Json(names, JsonRequestBehavior.AllowGet);
        }


        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult EditStudent(int StudentId)
        {
            var student = adminDAL.GetStudentById(StudentId);
            return View(student);
        }
        [HttpPost]
        [AdminAuthFilter]
        public RedirectToRouteResult EditStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                adminDAL.UpdateStudent(student);
                ViewBag.Message = "Student updated successfully.";
            }
            return RedirectToAction("StudentList");
        }
        public RedirectToRouteResult DeleteStudent(int StudentId)
        {
             adminDAL.DeleteStudent(StudentId);
            return RedirectToAction("StudentList");
        }

        // View All Teachers
        [AdminAuthFilter] //Apply filter to the whole controller
        public ActionResult TeacherList(string searchName = "", int? classNo = null)
        {
            
            var teachers = adminDAL.GetAllTeachers();
            ViewBag.SelectedClass = classNo;
            ViewBag.SearchName = searchName;

            return View(teachers);
        }
        [HttpGet]
        [AdminAuthFilter] //Apply filter to the whole controller
        public ActionResult EditTeacher(int TeacherID)
        {
            Teacher teacher = adminDAL.GetTeacherById(TeacherID);
            return View(teacher);
        }
        [HttpPost]
        [AdminAuthFilter]//Apply filter to the whole controller
        public RedirectToRouteResult EditTeacher(Teacher teacher)
        {
            adminDAL.UpdateTeacher(teacher);
            return RedirectToAction("TeacherList");
        }

        [AdminAuthFilter] // Apply filter to the whole controller
        public ActionResult DeleteTeacher(int TeacherId)
        {
            adminDAL.DeleteTeacher(TeacherId);
            return RedirectToAction("TeacherList");
        }

        // Assign Teacher To Class (GET)
        [AdminAuthFilter]  //Apply filter to the whole controller
        public ActionResult AssignTeacher()
        {
            ViewBag.Teachers = new SelectList(adminDAL.GetAllTeachers(), "TeacherId", "FullName");
            return View();
        }
        [HttpPost]
        [AdminAuthFilter]  //Apply filter to the whole controller
        public ActionResult AssignTeacher(int teacherId, int classNo)
        {
            adminDAL.AssignTeacherToClass(teacherId, classNo);
            ViewBag.Message = "Teacher assigned successfully";
            ViewBag.Teachers = new SelectList(adminDAL.GetAllTeachers(), "TeacherId", "FullName");
            return View();
        }

        // Generate Marksheet
        [AdminAuthFilter]//Apply filter to the whole controller
        public ActionResult GenerateResult()
        {
            var marks = markDAL.GetUnpublishedMarks();
            return View(marks);
        }

        //  Publish Result
        [HttpPost]
        [AdminAuthFilter] //Apply filter to the whole controller
        public ActionResult PublishResult()
        {
            markDAL.PublishMarks();
            TempData["Success"] = "Marksheet published successfully";
            return RedirectToAction("Dashboard");
        }

        //  Logout
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Back()
        {
            return RedirectToAction("Index", "Home");
        }   
    }
}
