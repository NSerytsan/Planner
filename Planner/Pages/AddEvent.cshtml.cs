using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planner.Dtos.Event;
using Planner.Services;

namespace Planner.Pages
{
    public class AddEventModel : PageModel
    {
        private readonly IEventService _eventService;
        private readonly ILogger<AddEventModel> _logger;

        public AddEventModel(IEventService eventService, ILogger<AddEventModel> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }
        [BindProperty]
        public EventPageModel EventModel { get; set; } = new EventPageModel();
        public void OnGet()
        {
        }
        
        public IActionResult OnPost()
        {
            ApplyCustomValidation();

            if (ModelState.IsValid)
            {
                var eventModel = GetViewModel();
                _eventService.AddEvent(eventModel);
                _logger.LogInformation("Added new event to records: {@eventModel}", eventModel);
                return RedirectToPage("Events");
            }

            return Page();
        }

        private EventViewModel GetViewModel() => new EventViewModel()
        {
            Title = EventModel.Title,
            Start = EventModel.Start.Value,
            End = EventModel.End.Value,
            Body = EventModel.Description,
            Location = EventModel.Location         
        };
        
        private void ApplyCustomValidation()
        {
            if (ModelState.IsValid && EventModel.Start.HasValue && EventModel.End.HasValue)
            {
                var isErrorExists = false;
                if (EventModel.Start.Value.CompareTo(EventModel.End.Value) > 0)
                {
                    ModelState.AddModelError("", "Start date should not be later than End date.");
                    isErrorExists = true;
                }

                if (!isErrorExists && _eventService.IsEventExists(EventModel.Title))
                {
                    ModelState.AddModelError("", "Event title is already exists.");
                }
            }
        }
    }

    public class EventPageModel
    {
        public string Color { get; set; } = "#9e5fff";

        [StringLength(100, ErrorMessage = "Maximum length is 50 characters."),
            Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Start date is required.")/*,
            ModelBinder(BinderType = typeof(CustomModelBinder))*/]
        public DateTime? Start { get; set; }
        [Required(ErrorMessage = "End date is required.")/*,
            ModelBinder(BinderType = typeof(CustomModelBinder))*/]
        public DateTime? End { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }
    }
}
