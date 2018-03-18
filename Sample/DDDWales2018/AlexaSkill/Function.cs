using System.Linq;
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

                    if (HandleIntent(input, intent.Intent, out SkillResponse response))
                    {
                        return response;
                    }
                    break;
            }

            return ResponseBuilder.Ask("Sorry, didn't understand a word. Please try that again", null);
        }










        private bool HandleIntent(SkillRequest request, Intent intent, out SkillResponse skillResponse)
        {
            skillResponse = null;
            switch (intent.Name)
            {
                case "NewGame":
                    skillResponse = ResponseBuilder.Tell("Old News");
                    return true;
                case "StartGame":
                    if (intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value)))
                    {
                        skillResponse = ResponseBuilder.DialogDelegate(request.Session);
                        return true;
                    }

                    skillResponse = ResponseBuilder.Tell("Game stuff coming soon!");
                    return true;

            }
            return false;
        }
    }
}
