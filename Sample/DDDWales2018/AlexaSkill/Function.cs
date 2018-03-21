using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Notifications;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.SkillMessaging;
using Amazon.Lambda.Core;
using Amazon.Runtime;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexaSkill
{

    public class Function
    {
        public Function()
        {
            RequestConverter.RequestConverters.Add(new MessageReceivedRequestTypeConverter());
        }


        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            Console.WriteLine("Received Request Type: "+input.Request.Type);
            switch (input.Request)
            {
                case LaunchRequest _:
                    return ResponseBuilder.Ask(Responses.Welcome, null);
                case IntentRequest intentRequest:
                    var intentResponse = await HandleIntent(input, intentRequest.Intent);
                    if (intentResponse != null)
                    {
                        return intentResponse;
                    }
                    break;
                case MessageReceivedRequest message:
                    await SendNotification(input,message);
                    return ResponseBuilder.Empty();
            }

            return ResponseBuilder.Ask("Sorry, didn't understand a word. Please try that again", null);
        }

        private Task SendNotification(SkillRequest request, MessageReceivedRequest message)
        {

            var token = request.Context.System.ApiAccessToken;
            var challengerName = message.Message["from"];
            var notification = new NotificationClient(NotificationClient.EuropeEndpoint,token);

            var display = new DisplayInfo{Content = new List<DisplayContent>
                {
                    DisplayContent(challengerName,"en-US"),
                    DisplayContent(challengerName,"en-GB")
                }
            };
            var spoken = new SpokenInfo
            {
                Content = new List<SpokenText>
                {
                    new SpokenText("en-US","You've a new game of rock paper scissors lizard spock"),
                    new SpokenText("en-GB","You've a new game of rock paper scissors lizard spock")
                }
            };
            var reference = Guid.NewGuid().ToString("N");
            var expiry = DateTime.Now.AddSeconds(30);

            return notification.Create(display, spoken, reference, expiry);
        }

        private DisplayContent DisplayContent(string challengerName, string locale)
        {
            return new DisplayContent
            {
                Title = $"A new challenge has been issued",
                Body = $"{challengerName} has challenged you to a game at DDD Wales",
                Toast = new ContentItem("A new challenge has been issued"),
                Locale = locale
            };
        }


        private Task<SkillResponse> HandleIntent(SkillRequest request, Intent intent)
        {
            switch (intent.Name)
            {
                case BuiltInIntent.Stop:case BuiltInIntent.Cancel:
                    return Task.FromResult(ResponseBuilder.Tell(Responses.Close));
                case IntentNames.StartGame:
                    return StartGame.Handle(request,intent);
                case IntentNames.CheckChallenges:
                    return CheckChallenge.Handle(request, intent);
                case IntentNames.Moves:
                    return Task.FromResult(ResponseBuilder.Tell(Responses.Moves));
            }

            return null;
        }

    }
}
