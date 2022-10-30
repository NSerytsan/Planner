#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planner.Dtos.Event;
using Planner.Services;

namespace Planner.Pages
{
    public class EventsModel : PageModel
    {
        private readonly IEventService _eventService;
        public EventsModel(IEventService eventService)
        {
            _eventService = eventService;
            SelectListItems = Enum.GetNames(typeof(EventStatus)).Select(s =>
            {
                var status = (short)(EventStatus)Enum.Parse(typeof(EventStatus), s);
                var selectListItem = new SelectListItem
                {
                    Text = status == 1 ? "All" : s,
                    Value = status.ToString(),
                    Selected = SearchModel.StatusId == status
                };
                return selectListItem;
            }).ToList();
        }

        public List<SelectListItem> SelectListItems { get; set; }
        public IEnumerable<EventViewModel> Events { get; set; }

        [BindProperty]
        public Search SearchModel { get; set; } = new Search();

        public void OnGet()
        {
            FillValues();
        }

        public IActionResult OnPost()
        {
            FillValues();

            return Page();
        }

        private void FillValues()
        {
            Events = _eventService.GetEventViewModels();

            ApplyFilter();
        }

        public class Search
        {
            public int StatusId { get; set; } = 1;
            public string SearchKey { get; set; } = string.Empty;
        }

        private void ApplyFilter()
        {
            var selectedStatus = (EventStatus)int.Parse(SelectListItems.FirstOrDefault(q => q.Selected).Value);

            if (selectedStatus != EventStatus.None)
                Events = Events.Where(q => q.ActualStatus == selectedStatus);

            ApplySearch();
        }

        private void ApplySearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchModel.SearchKey))
            {
                var comparison = StringComparison.InvariantCultureIgnoreCase;

                Events = Events.Where(q => q.Title.Contains(SearchModel.SearchKey, comparison) ||
                    q.Start.ToShortDateString().Contains(SearchModel.SearchKey, comparison) ||
                    q.End.ToShortDateString().Contains(SearchModel.SearchKey, comparison) ||
                    q.ActualStatus.ToString().Contains(SearchModel.SearchKey, comparison));
            }
        }
    }
}
