using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Notifications;
using Alexa.NET.Request;
using Alexa.NET.SkillMessaging;

namespace AlexaSkill
{
    public class GameNotification
    {
        public static Task Send(SkillRequest request, MessageReceivedRequest message)
        {

            var token = request.Context.System.ApiAccessToken;
            var challengerName = message.Message["from"];
            var notification = new NotificationClient(NotificationClient.EuropeEndpoint, token);

            var display = new DisplayInfo { Content = new List<DisplayContent>() };
            var spoken = new SpokenInfo { Content = new List<SpokenText>() };

            AddNotificationTo(display, spoken, challengerName, "en-US");
            AddNotificationTo(display, spoken, challengerName, "en-GB");

            var reference = Guid.NewGuid().ToString("N");
            var expiry = DateTime.Now.AddSeconds(30);

            return notification.Create(display, spoken, reference, expiry);
        }

        private static void AddNotificationTo(DisplayInfo display, SpokenInfo spoken, string challengerName,
            string locale)
        {
            display.Content.Add(DisplayContent(challengerName, locale));
            spoken.Content.Add(new SpokenText("en-US", "You've a new game of rock paper scissors lizard spock"));
        }

        private static DisplayContent DisplayContent(string challengerName, string locale)
        {
            return new DisplayContent
            {
                Title = $"A new challenge has been issued",
                Body = $"{challengerName} has challenged you to a game at DDD Wales",
                Toast = new ContentItem("A new challenge has been issued"),
                Locale = locale
            };
        }
    }
}