namespace Tibur_LabAct1.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int IdNumber { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
}