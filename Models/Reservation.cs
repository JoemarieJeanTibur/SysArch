namespace Tibur_LabAct1.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string IdNumber { get; set; }  // FIX: make STRING (IMPORTANT)
        public string Name { get; set; }
        public string Lab { get; set; }
        public DateTime ReservationDate { get; set; }
        public string ReservationTime { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; } = "Pending";
    }
}