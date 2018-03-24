using System;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET.SkillMessaging;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AlexaSkill
{

    public class Function
    {
        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
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

            }

            return ResponseBuilder.Ask("Sorry, didn't understand a word. Please try that again", null);
        }

        private Task<SkillResponse> HandleIntent(SkillRequest request, Intent intent)
        {
            switch (intent.Name)
            {
                case BuiltInIntent.Stop:
                case BuiltInIntent.Cancel:
                    return CloseGame.Handle();
                case IntentNames.StartGame:
                    return StartGame.Handle(request, intent);
                case IntentNames.CheckChallenges:
                    return CheckChallenge.Handle(request, intent);
                case IntentNames.Moves:
                    return MoveInformation.Handle();
                case IntentNames.DebugResults:
                    return DebugResults.Handle(request, intent);
            }

            return null;
        }

    }
}
