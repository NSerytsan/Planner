namespace Planner.Dtos.Plan
{
    public abstract class BasePlanDto
    {
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public string color { get; set; } = null!;
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}