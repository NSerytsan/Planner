using System.ComponentModel.DataAnnotations;

namespace Planner.Data
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Address { get; set; } = null!;
        public string FilePath { get; set; } = string.Empty;
        public byte IsPublic { get; set; }
    }
}