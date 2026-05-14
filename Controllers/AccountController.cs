using Microsoft.AspNetCore.Mvc;
using Tibur_LabAct1.Models;
using Tibur_LabAct1.Data;

namespace Tibur_LabAct1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public async Task<IActionResult> UpdateProfilePicture(IFormFile ProfilePicture)
        {
            var studentId = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(studentId))
                return RedirectToAction("Login");

            if (ProfilePicture == null || ProfilePicture.Length == 0)
            {
                TempData["ProfileError"] = "Please select an image to upload.";
                return RedirectToAction("StudentProfile");
            }

            if (!ProfilePicture.ContentType.StartsWith("image/"))
            {
                TempData["ProfileError"] = "Profile picture must be an image file.";
                return RedirectToAction("StudentProfile");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".jfif" };
            var extension = Path.GetExtension(ProfilePicture.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                TempData["ProfileError"] = "Please upload a JPG, PNG, GIF, WEBP, or JFIF image.";
                return RedirectToAction("StudentProfile");
            }

            var user = _context.Students.FirstOrDefault(u => u.IdNumber == studentId);

            if (user == null)
                return RedirectToAction("Login");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfilePicture.CopyToAsync(stream);
            }

            user.ProfilePicture = $"/uploads/{fileName}";
            _context.SaveChanges();

            TempData["ProfileSuccess"] = "Profile picture updated.";
            return RedirectToAction("StudentProfile");
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
