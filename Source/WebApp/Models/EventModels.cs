using System.Collections.Generic;

namespace WebApp.Models
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
