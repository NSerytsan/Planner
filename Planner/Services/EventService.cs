using Planner.Data;
using Planner.Dtos.Event;
using Planner.Extensions;

namespace Planner.Services
{
    public class EventService : IEventService
    {
        private readonly PlannerDbContext _context;

        public EventService(PlannerDbContext context)
        {
            _context = context;
        }

        public void AddEvent(EventViewModel eventViewModel)
        {
            var ev = eventViewModel.ToEvent();
            _context.Events.Add(ev);
            _context.SaveChanges();
        }

        public void EditEvent(EventViewModel eventViewModel)
        {
            throw new NotImplementedException();
        }

        public EventViewModel GetEventViewModel(string value)
        {
            var events = GetEventViewModels();
            return events.FirstOrDefault(q => q.SlugTitle.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public EventViewModel GetEventViewModelById(int id)
        {
            var events = GetEventViewModels();
            return events.FirstOrDefault(q => q.Id == id);
        }

        public IEnumerable<EventViewModel> GetEventViewModels()
        {
            var events = _context.Events.ToList();
            return events.ToEventViewModels();
        }

        public IEnumerable<EventViewModel> GetEventViewModels(EventFilter eventFilter)
        {
            throw new NotImplementedException();
        }

        public bool IsEventExists(string eventName)
        {
            var events = GetEventViewModels();
            return events.Any(q => q.Title.Equals(eventName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsEventExists(string eventName, string id)
        {
            throw new NotImplementedException();
        }

        public void RemoveEvent(string id)
        {
            throw new NotImplementedException();
        }
    }
}