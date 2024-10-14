using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AE.Net.Mail;

namespace HomeMaintenanceCalendarMinder
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly, GmailService.Scope.GmailCompose };
        static string ApplicationName = "Home Maintenance Calendar Minder";

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            UserCredential credential;

            // May have to generate your own client_secret.son file? It's been a while since I first looked at this.
            // https://developers.google.com/calendar/api/guides/overview
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.ReadWrite)) 
            {
                // Points to credentials folder in My Documents
                string credPath = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Google Calendar API service.
            var calendarservice = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var gmailservice = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            EventsResource.ListRequest eventList = calendarservice.Events.List("9fjf6vkf1ou50j2umnju2lnrss@group.calendar.google.com");

            eventList.TimeMaxDateTimeOffset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 30);   
            eventList.TimeMinDateTimeOffset = new DateTime(eventList.TimeMaxDateTimeOffset.Value.Year, eventList.TimeMaxDateTimeOffset.Value.Month, 1);

            Events userEvents = eventList.Execute();
            var userEventList = userEvents.Items.ToList();
            
            string bodyText = "Home Maintenance Calendar Summary For " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + "\n\n";
            if (userEventList.Count > 0)
            {
                foreach (var e in userEventList)
                {
                    bodyText += "     " + e.Summary;
                    if (e.OriginalStartTime != null)
                    {
                        bodyText += " - " + e.OriginalStartTime.Date + "\n";
                    }
                    else
                    {
                        bodyText += "\n";
                    }
                }
            }
            else
            {
                bodyText += "No events for this month.";
            }

            MailMessage message = new MailMessage()
            {
                From = new System.Net.Mail.MailAddress("justin@jpassmore.com"),
                Subject = "Home Maintenance Monthly Checklist",
                Body = bodyText,
                Encoding = Encoding.UTF8
            };

            message.To.Add(new System.Net.Mail.MailAddress("justin@jpassmore.com"));
            var sw = new StringWriter();
            message.Save(sw);
            byte[] encodedBytes = Encoding.UTF8.GetBytes(sw.ToString());
            string base64EncodedText = System.Convert.ToBase64String(encodedBytes);

            var result = gmailservice.Users.Messages.Send(new Message
            {
                Raw = base64EncodedText,
            }, "me").Execute();
            Console.WriteLine("Message ID {0} sent.", result.Id);
        }
    }
}