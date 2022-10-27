using Planner.Extensions;

namespace Planner.Dtos.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public EventStatus ActualStatus
        {
            get
            {
                if (End.CompareTo(DateTime.Now) < 0) return EventStatus.Expired;
                else if (CheckEventStatus(Start)) return EventStatus.Current;
                else return EventStatus.Upcoming;
            }
        }
        public string Title { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        private bool CheckEventStatus(DateTime start) => DateTime.Now.Month == start.Month && DateTime.Now.Year == start.Year;
        public string Location { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Status
        {
            get
            {
                if (End.CompareTo(DateTime.Now) < 0) return EventStatus.Expired.GetBadge();
                else if (CheckEventStatus(Start)) return EventStatus.Current.GetBadge();
                else return EventStatus.Upcoming.GetBadge();
            }
        }
        public string FormattedDate { get { return $"{Start.GetFormattedDate()} to {End.GetFormattedDate()}"; } }
        public string SlugTitle { get { return Title.ApplySlug(); } }
    }
}