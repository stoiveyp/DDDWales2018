using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Response;

namespace AlexaSkill
{
    internal class MoveInfo
    {
        public static Task<SkillResponse> Handle()
        {
            return Task.FromResult(ResponseBuilder.Tell(Responses.Moves));
        }
    }
}