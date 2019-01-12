using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GetCalendarEvents
{
    /*
     * Reference: 
     * https://developers.google.com/calendar/quickstart/dotnet
     */

    class Program
    {
        const string AUTHENTICATION_CLIENT_ID = "dd-client-id.json";
        const string APP_NAME = "DC - Read daily events";
        static readonly string[] CALENDARS = new string[] { "primary", "en.al#holiday@group.v.calendar.google.com", "#contacts@group.v.calendar.google.com" };

        static readonly string[] Scopes = { CalendarService.Scope.CalendarEventsReadonly };

        // this is where the events will be saved in file system to be used by the web app.
#if DEBUG
        const string STORAGE_FILE_PATH = "calendar-events.json";
#else
        const string STORAGE_FILE_PATH = "/var/netcore/web/wwwroot/calendar/calendar-events.json";
#endif

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream = new FileStream(AUTHENTICATION_CLIENT_ID, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = APP_NAME,
            });

            var storageModel = new List<StorageEvents>();
            foreach (var item in CALENDARS)
            {
                var evt = QueryEvents(item, ref service);
                storageModel.Add(new StorageEvents
                {
                    CalendarId = item,
                    Events = evt
                });
            }
            
            var eventsString = JsonConvert.SerializeObject(storageModel);
            File.WriteAllText(STORAGE_FILE_PATH, eventsString);
        }

        private static List<EventModel> QueryEvents(string calendarId, ref CalendarService service)
        {
            var eventsObj = new List<EventModel>();

            var now = DateTime.Now;
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List(calendarId);
            // for current month only
            request.TimeMin = new DateTime(now.Year, now.Month, 1);
            request.TimeMax = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            if (events.Items != null && events.Items.Count > 0)
            {
                eventsObj = events.Items.Select(x => new EventModel
                {
                    What = x.Summary,
                    When = x.Start.Date
                }).ToList();
            }

            return eventsObj;
        }
    }
}
