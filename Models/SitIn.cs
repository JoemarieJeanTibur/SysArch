using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace Tibur_LabAct1.Models
{
    public class SitIn
    {
        public int Id { get; set; }
        public string IdNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Lab { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public string Status { get; set; } = "Active";
        public int RemainingSession { get; set; }
    }
}
