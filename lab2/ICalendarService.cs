namespace lab2
{
    public interface ICalendarService
    {
        List<CalendarEvent> GetEvents();
        CalendarEvent GetCalendarEvent(Guid eventId);
        void AddEvent(CalendarEvent calendarEvent);
        void DeleteEvent(Guid eventId);
        void UpdateEvent(Guid eventId, CalendarEvent calendarEvent);
        int GetCount();
    }
}
