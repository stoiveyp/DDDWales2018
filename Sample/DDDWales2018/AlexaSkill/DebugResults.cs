using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;

namespace AlexaSkill
{
    internal class DebugResults
    {
        public static Task<SkillResponse> Handle(SkillRequest request, Intent intent)
        {
            if (intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value)))
            {
                return Task.FromResult(ResponseBuilder.DialogDelegate(request.Session, intent));
            }
        }
    }
}