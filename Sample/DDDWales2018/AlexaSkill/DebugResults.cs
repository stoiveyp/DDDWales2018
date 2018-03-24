using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using NoTone;

namespace AlexaSkill
{
    static class DebugResults
    {
        private const string Player1Id = "fake1";
        private const string Player2Id = "fake2";

        public static Task<SkillResponse> Handle(SkillRequest request, Intent intent)
        {
            if (intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value)))
            {
                return Task.FromResult(ResponseBuilder.DialogDelegate(request.Session, intent));
            }

            var fakeGame = new Game();

            fakeGame.Player1 = new Moves
            {
                UserId=Player1Id,
                MoveInformation = new List<Move>
                {
                    intent.Slots.ParseMove("playerOneOne"),
                    intent.Slots.ParseMove("playerOneTwo"),
                    intent.Slots.ParseMove("playerOneThree")
                }
            };

            fakeGame.Player2 = new Moves
            {
                UserId=Player2Id,
                MoveInformation = new List<Move>
                {
                    intent.Slots.ParseMove("playerTwoOne"),
                    intent.Slots.ParseMove("playerTwoTwo"),
                    intent.Slots.ParseMove("playerTwoThree")
                }
            };

            var results = fakeGame.ResultsFor("fake1");
            return Task.FromResult(
                ResponseBuilder.Tell(Responses.EmotiveResults(results))
                );
        }
    }
}