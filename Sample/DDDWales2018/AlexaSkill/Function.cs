using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using BoredomInc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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







        private async Task<SkillResponse> HandleIntent(SkillRequest request, Intent intent)
        {
            var userId = request.Session.User.UserId;
            switch (intent.Name)
            {
                case IntentNames.StartGame:
                    var response = ValidateNewGame(request, intent);
                    if (response != null)
                    {
                        return response;
                    }

                    await CreateGame(userId,intent);
                    return ResponseBuilder.Tell(Responses.GameCreated);
                case IntentNames.CheckChallenges:
                    var challenges = await GetChallenges(userId);
                    if (challenges.Count > 0)
                    {
                        return ResponseBuilder.Ask(Responses.PendingChallenges(challenges.Count),null);
                    }

                    return ResponseBuilder.Tell(Responses.NoChallenges);
            }

            return null;
        }

        private async Task<List<string>> GetChallenges(string userId)
        {
            var s3 = new AmazonS3Client();
            var objects = await s3.ListObjectsAsync(
                System.Environment.GetEnvironmentVariable("bucket"),
                $"challenge_{userId}"
                );
            return objects.S3Objects.Select(o => o.Key).ToList();
        }


        private SkillResponse ValidateNewGame(SkillRequest request, Intent intent)
        {
            var opponent = intent.Slots.First(s => s.Key == SlotNames.Opponent);
            if (!string.IsNullOrWhiteSpace(opponent.Value.Value) || !ValidateName(opponent.Value.Value))
            {
                return ResponseBuilder.DialogElicitSlot(new PlainTextOutputSpeech
                {
                    Text = "I didn't recognise your opponent's name, could you repeat that?"
                }, SlotNames.Opponent, request.Session, intent);
            }

            if (intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value)))
            {
                return ResponseBuilder.DialogDelegate(request.Session);
            }

            return null;
        }


        private async Task<string> CreateGame(string challenger, Intent intent)
        {
            var game = new Game
            {
                Player1 = new Moves
                {
                    UserId = challenger,
                    MoveInformation = new List<Move>
                    {
                        ParseMove(intent.Slots[SlotNames.MoveOne].Value),
                        ParseMove(intent.Slots[SlotNames.MoveTwo].Value),
                        ParseMove(intent.Slots[SlotNames.MoveThree].Value)
                    }
                }
            };

            var opponent = GetOpponentUserId(challenger, intent);
            var s3 = new AmazonS3Client();

            var putRequest = new PutObjectRequest
            {
                BucketName = Environment.GetEnvironmentVariable("bucket"),
                Key = $"challenge_{opponent}_{challenger}",
                ContentBody = JObject.FromObject(game).ToString(Formatting.Indented)
            };
            await s3.PutObjectAsync(putRequest);
            return opponent;
        }

        private string GetOpponentUserId(string challenger, Intent intent)
        {
            var opponentName = intent.Slots[SlotNames.Opponent].Value;
            if (opponentName == "myself")
            {
                return challenger;
            }
            throw new InvalidOperationException("Unable to find opponent " + opponentName);
        }

        private Move ParseMove(string value)
        {
            if (Enum.TryParse(value, out Move move))
            {
                return move;
            }

            throw new InvalidOperationException("Unknown move type " + value);
        }

        private bool ValidateName(string opponentName)
        {
            return opponentName.ToLower() == "myself";
        }
    }
}
