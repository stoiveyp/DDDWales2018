using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
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

                case IntentRequest intent:
                    if (intent.Intent.Name == BuiltInIntent.Cancel|| intent.Intent.Name == BuiltInIntent.Stop)
                    {
                        return ResponseBuilder.Tell("Okay");
                    }

                    var intentResponse = await HandleIntent(input, intent.Intent);
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
                case IntentNames.StartGame:
                    return StartGame.Handle(request,intent);
                case IntentNames.CheckChallenges:
                    return CheckChallenge.Handle(request, intent);
            }

            return null;
        }

    }
}
