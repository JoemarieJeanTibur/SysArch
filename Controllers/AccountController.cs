using Microsoft.AspNetCore.Mvc;
using Tibur_LabAct1.Models;
using Tibur_LabAct1.Data;

namespace Tibur_LabAct1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string IdNumber, string Password)
        {
            // ✅ ADMIN LOGIN
            if (IdNumber == "1000" && Password == "admin123")
            {
                HttpContext.Session.SetString("User", "admin");
                HttpContext.Session.SetString("Role", "Admin");

                return RedirectToAction("Dashboard", "Admin");
            }

            var user = _context.Students
                .FirstOrDefault(u => u.IdNumber == IdNumber && u.Password == Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            // ✅ STORE SESSION
            HttpContext.Session.SetString("User", user.IdNumber);
            HttpContext.Session.SetString("Role", "Student");

            return RedirectToAction("StudentProfile");
        }

        [HttpGet]
        public IActionResult StudentProfile()
        {
            var studentId = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(studentId))
                return RedirectToAction("Login");

            var user = _context.Students.FirstOrDefault(u => u.IdNumber == studentId);

            if (user == null)
                return RedirectToAction("Login");

            ViewBag.Student = user;
            ViewBag.SessionsLeft = user.RemainingSession;

            // ✅ TOTALS
            ViewBag.TotalSitIns = _context.SitIn.Count(s => s.IdNumber == studentId);
            ViewBag.TotalReservations = _context.Reservations.Count(r => r.IdNumber == studentId);

            // ✅ THIS FIXES YOUR SIT-IN HISTORY (IMPORTANT)
            ViewBag.MySitIns = _context.SitIn
                .Where(s => s.IdNumber == studentId)
                .OrderByDescending(s => s.TimeIn)
                .ToList();

            // OTHER DATA
            ViewBag.Announcements = _context.Announcements
                .OrderByDescending(a => a.DatePosted)
                .ToList();

            ViewBag.MyReservations = _context.Reservations
                .Where(r => r.IdNumber == studentId)
                .ToList();

            ViewBag.MyFeedbacks = _context.Feedbacks
                .Where(f => f.IdNumber.ToString() == studentId)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitFeedback(string message)
        {
            var studentId = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(studentId))
                return RedirectToAction("Login");

            var user = _context.Students.FirstOrDefault(u => u.IdNumber == studentId);

            if (!string.IsNullOrWhiteSpace(message))
            {
                _context.Feedbacks.Add(new Feedback
                {
                    IdNumber = int.Parse(studentId),
                    Name = $"{user.FirstName} {user.LastName}",
                    Message = message,
                    DateSubmitted = DateTime.Now
                });

                _context.SaveChanges();
            }

            return RedirectToAction("StudentProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitReservation(string Lab, string Purpose, DateTime ReservationDate, string ReservationTime)
        {
            var studentId = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(studentId))
                return RedirectToAction("Login");

            var user = _context.Students.FirstOrDefault(u => u.IdNumber == studentId);

            _context.Reservations.Add(new Reservation
            {
                IdNumber = studentId,
                Name = $"{user.FirstName} {user.LastName}",
                Lab = Lab,
                Purpose = Purpose,
                ReservationDate = ReservationDate,
                ReservationTime = ReservationTime,
                Status = "Pending"
            });

            _context.SaveChanges();

            return RedirectToAction("StudentProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string IdNumber, string FirstName, string LastName,
            string MiddleName, string Course, int CourseLvl, string Email,
            string Address, string Password, string RepeatPassword)
        {
            if (Password != RepeatPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var existing = _context.Students.FirstOrDefault(u => u.IdNumber == IdNumber);
            if (existing != null)
            {
                ViewBag.Error = "ID Number is already registered.";
                return View();
            }

            _context.Students.Add(new User
            {
                IdNumber = IdNumber,
                FirstName = FirstName,
                LastName = LastName,
                MiddleName = MiddleName,
                Course = Course,
                CourseLvl = CourseLvl,
                Email = Email,
                Address = Address,
                Password = Password,
                RepeatPassword = RepeatPassword,
                RemainingSession = 30
            });

            _context.SaveChanges();

            TempData["RegisterSuccess"] = "Account created!";
            return RedirectToAction("Login");
        }
    }
}