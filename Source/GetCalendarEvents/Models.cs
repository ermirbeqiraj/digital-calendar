using System.Collections.Generic;

namespace GetCalendarEvents
{
    internal class EventModel
    {
        public string What { get; set; }
        public string When { get; set; }
    }

    internal class StorageEvents
    {
        public string CalendarId { get; set; }
        public List<EventModel> Events { get; set; }
    }
}
