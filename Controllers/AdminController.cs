using Microsoft.AspNetCore.Mvc;
using Tibur_LabAct1.Data;
using Tibur_LabAct1.Models;

namespace Tibur_LabAct1.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
                return RedirectToAction("Login", "Account");

            ViewBag.Students = _context.Students.ToList();
            ViewBag.CurrentSitins = _context.SitIn.Where(s => s.Status == "Active").ToList();
            ViewBag.AllRecords = _context.SitIn.ToList();
            ViewBag.Announcements = _context.Announcements.OrderByDescending(a => a.DatePosted).ToList();
            ViewBag.Feedbacks = _context.Feedbacks.OrderByDescending(f => f.DateSubmitted).ToList();
            ViewBag.Reservations = _context.Reservations.OrderByDescending(r => r.ReservationDate).ToList();

            return View();
        }

        [HttpGet]
        public IActionResult SearchStudent(string query)
        {
            var results = _context.Students
                .Where(s => s.IdNumber.Contains(query) ||
                            s.FirstName.Contains(query) ||
                            s.LastName.Contains(query))
                .Select(s => new {
                    s.IdNumber,
                    s.FirstName,
                    s.LastName,
                    s.Course,
                    s.CourseLvl,
                    s.Email,
                    s.Address,
                    s.RemainingSession
                })
                .ToList();

            return Json(results);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterSitin(string IdNumber, string Name, string Purpose, string Lab)
        {
            var allowedLabs = new[] { "Lab 524", "Lab 526", "Lab 528", "Lab 530", "Lab 542", "Lab 544" };

            if (!allowedLabs.Contains(Lab))
            {
                TempData["SitinError"] = "Please select a valid lab.";
                return RedirectToAction("Dashboard");
            }

            var student = _context.Students.FirstOrDefault(s => s.IdNumber == IdNumber);

            if (student == null)
            {
                TempData["SitinError"] = "Student not found.";
                return RedirectToAction("Dashboard");
            }

            if (student.RemainingSession <= 0)
            {
                TempData["SitinError"] = "No remaining sessions!";
                return RedirectToAction("Dashboard");
            }

            student.RemainingSession -= 1;

            var sitin = new SitIn
            {
                IdNumber = IdNumber,
                Name = Name,
                Lab = Lab,
                Purpose = Purpose,
                TimeIn = DateTime.Now,
                Status = "Active",
                RemainingSession = student.RemainingSession
            };

            _context.SitIn.Add(sitin);
            _context.SaveChanges();

            TempData["SitinSuccess"] = "Sit-in registered successfully!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EndSitin(int id)
        {
            var sitin = _context.SitIn.FirstOrDefault(s => s.Id == id);
            if (sitin != null)
            {
                sitin.Status = "Done";
                sitin.TimeOut = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveReservation(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation != null)
            {
                reservation.Status = "Approved";
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectReservation(int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation != null)
            {
                reservation.Status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult EditStudent(string id)
        {
            var student = _context.Students.FirstOrDefault(s => s.IdNumber == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStudent(User student)
        {
            var existing = _context.Students.FirstOrDefault(s => s.IdNumber == student.IdNumber);
            if (existing == null) return NotFound();

            existing.FirstName = student.FirstName;
            existing.LastName = student.LastName;
            existing.Course = student.Course;
            existing.CourseLvl = student.CourseLvl;
            existing.Email = student.Email;
            existing.Address = student.Address;
            existing.RemainingSession = student.RemainingSession;

            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult DeleteStudent(string id)
        {
            var student = _context.Students.FirstOrDefault(s => s.IdNumber == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAnnouncement(string Title, string Content)
        {
            if (!string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content))
            {
                _context.Announcements.Add(new Announcement
                {
                    Title = Title,
                    Content = Content,
                    DatePosted = DateTime.Now
                });
                _context.SaveChanges();
                TempData["AnnouncementSuccess"] = "Announcement posted!";
            }
            else
            {
                TempData["AnnouncementError"] = "Title and content are required.";
            }
            return RedirectToAction("Dashboard");
        }

        public IActionResult DeleteAnnouncement(int id)
        {
            var a = _context.Announcements.Find(id);
            if (a != null)
            {
                _context.Announcements.Remove(a);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
