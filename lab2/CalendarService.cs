
namespace lab2
{
    public class CalendarService : ICalendarService
    {
        private List<CalendarEvent> _events;

        public CalendarService()
        {
            _events = new List<CalendarEvent>();
        }
        public void AddEvent(CalendarEvent calendarEvent)
        {
            _events.Add(calendarEvent);
        }

        public void DeleteEvent(Guid eventId)
        {
            var calendarEvent = GetCalendarEvent(eventId);
            if (calendarEvent != null)
            {
                _events.Remove(calendarEvent);
            }
        }

        public CalendarEvent GetCalendarEvent(Guid eventId)
        {
            return _events.FirstOrDefault(e => e.Id == eventId);
        }

        public List<CalendarEvent> GetEvents()
        {
            return _events;
        }

        public void UpdateEvent(Guid eventId, CalendarEvent calendarEvent)
        {
            var existingEvent = GetCalendarEvent(eventId);
            if (existingEvent != null)
            {
                existingEvent.Title = calendarEvent.Title;
                existingEvent.Description = calendarEvent.Description;
                existingEvent.StartDate = calendarEvent.StartDate;
                existingEvent.EndDate = calendarEvent.EndDate;
            }
        }

        public int GetCount()
        {
            return _events.Count;
        }
    }
}
