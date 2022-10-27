using System.ComponentModel.DataAnnotations;

namespace Planner.Data
{
    public class Plan
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public byte IsFullDay { get; set; }
        public string Color { get; set; } = null!;
    }
}