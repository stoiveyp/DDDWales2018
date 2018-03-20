using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Amazon.S3;
using Amazon.S3.Model;
using BoredomInc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AlexaSkill
{
    internal class StartGame
    {
        public static async Task<SkillResponse> Handle(SkillRequest request, Intent intent)
        {
            var userId = request.Session.User.UserId;

            var response = ValidateNewGame(request, intent);
            if (response != null)
            {
                return response;
            }

            await CreateGame(userId, intent);
            return ResponseBuilder.Tell(Responses.GameCreated);
        }

        private static SkillResponse ValidateNewGame(SkillRequest request, Intent intent)
        {
            var opponent = intent.Slots.First(s => s.Key == SlotNames.Opponent);

            if (!string.IsNullOrWhiteSpace(opponent.Value.Value) && !ValidateName(opponent.Value.Value))
            {
                intent.Slots[SlotNames.Opponent].Value = null;
                return ResponseBuilder.DialogElicitSlot(new PlainTextOutputSpeech
                {
                    Text = "I didn't recognise your opponent's name, could you repeat that?"
                }, SlotNames.Opponent, request.Session, intent);
            }


            if (intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value)))
            {
                return ResponseBuilder.DialogDelegate(request.Session,intent);
            }

            return null;
        }


        private static async Task<string> CreateGame(string challenger, Intent intent)
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
            var bucket = Environment.GetEnvironmentVariable("bucket");
            var key = $"challenge_{opponent}_{challenger}";
            var s3 = new AmazonS3Client();

            
            Console.WriteLine($"{bucket}");
            Console.WriteLine($"{key}");
            var putRequest = new PutObjectRequest
            {
                BucketName = bucket,
                Key = key,
                ContentBody = JObject.FromObject(game).ToString(Formatting.Indented)
            };
            await s3.PutObjectAsync(putRequest);
            return opponent;
        }

        private static string GetOpponentUserId(string challenger, Intent intent)
        {
            var opponentName = intent.Slots[SlotNames.Opponent].Value;
            if (opponentName == "myself")
            {
                return challenger;
            }
            throw new InvalidOperationException("Unable to find opponent " + opponentName);
        }

        private static Move ParseMove(string value)
        {
            if (Enum.TryParse(value, true, out Move move))
            {
                return move;
            }

            throw new InvalidOperationException("Unknown move type " + value);
        }

        private static bool ValidateName(string opponentName)
        {
            return (opponentName?.ToLower() ?? string.Empty) == "myself";
        }
    }
}