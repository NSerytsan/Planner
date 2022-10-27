using System.Globalization;
using System.Text.RegularExpressions;
using Planner.Dtos.Event;

namespace Planner.Extensions
{
    public static class EventExtensions
    {
        public static string GetBadge(this EventStatus eventStatus)
        {
            string htmlclass = eventStatus switch
            {
                EventStatus.Current => "badge rounded-pill bg-success",
                EventStatus.Upcoming => "badge rounded-pill bg-dark",
                EventStatus.Expired => "badge rounded-pill bg-danger",
                _ => string.Empty,
            };

            return @$"<span class=""{htmlclass}"">{eventStatus}</span>";
        }

        public static string GetFormattedDate(this DateTime dateTime)
        {
            var cultureInfo = new CultureInfo("uk-UA");
            return $"{cultureInfo.DateTimeFormat.GetMonthName(dateTime.Month)} {dateTime.Day}, {dateTime.Year}";
        }

        public static string ApplySlug(this string value) => 
        Regex.Replace(value.Replace(" ", "-").ToString()!, @"[^\w-]+", "", 
        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)).ToLowerInvariant();

        public static EventViewModel ToEventViewModel(this Planner.Data.Event ev)
        {
            return new EventViewModel()
            {
                Id = ev.Id,
                Title = ev.Name,
                Body = ev.Description,
                Start = ev.StartDate,
                End = ev.EndDate,
                Location = ev.Address
            };
        }

        public static IEnumerable<EventViewModel> ToEventViewModels(this IEnumerable<Planner.Data.Event> events)
        {
            return from ev in events
                   select ev.ToEventViewModel();
        }

        public static Data.Event ToEvent(this EventViewModel eventViewModel)
        {
            return new Data.Event()
            {
                Id = eventViewModel.Id,
                Name = eventViewModel.Title,
                Description = eventViewModel.Body,
                StartDate = eventViewModel.Start,
                EndDate = eventViewModel.End,
                Address = eventViewModel.Location
            };
        }
    }
}