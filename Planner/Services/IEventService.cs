using Planner.Dtos.Event;

namespace Planner.Services
{
    public interface IEventService
    {
        IEnumerable<EventViewModel> GetEventViewModels();
        void RemoveEvent(string id);
        void AddEvent(EventViewModel eventViewModel);
        void EditEvent(EventViewModel eventViewModel);
        bool IsEventExists(string eventName);
        bool IsEventExists(string eventName, string id);
        EventViewModel GetEventViewModel(string value);
        EventViewModel GetEventViewModelById(int id);
        IEnumerable<EventViewModel> GetEventViewModels(EventFilter eventFilter);
    }
}