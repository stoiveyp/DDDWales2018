using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using BoredomInc;

namespace AlexaSkill
{
    internal class CheckChallenge
    {
        public static async Task<SkillResponse> Handle(SkillRequest request, Intent intent)
        {
            var userId = request.Session.User.UserId;

            var challenges = await GetChallenges(userId);
            var nextChallenge = challenges.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nextChallenge))
            {
                return ResponseBuilder.Tell(Responses.NoChallenges);
            }

            if (RejectedChallenges(intent))
            {
                return ResponseBuilder.Tell("Okay, hope you play soon");
            }

            if (!ConfirmedChallenge(intent))
            {
                var nextOpponent = GetOpponentName(userId, nextChallenge.Split("_").Last());
                return AskAboutNextOpponent(request, intent, nextOpponent);
            }

            if (MovesRemaining(intent))
            {
                return ResponseBuilder.DialogDelegate(request.Session, intent);
            }

            return await CompleteGame(nextChallenge,userId,intent);
        }

        private static async Task<SkillResponse> CompleteGame(string nextChallenge,string userId, Intent intent)
        {
            var bucket = Environment.GetEnvironmentVariable("bucket");
            var s3 = new AmazonS3Client();
            var result = await s3.GetObjectAsync(bucket, nextChallenge);
            var game = new JsonSerializer().Deserialize<Game>(result.ResponseStream);
            game.Player2.UserId = userId;
            game.Player2.MoveInformation = new List<Move>
            {
                ParseMove(intent.Slots[SlotNames.MoveOne].Value),
                ParseMove(intent.Slots[SlotNames.MoveTwo].Value),
                ParseMove(intent.Slots[SlotNames.MoveThree].Value)
            };

            await s3.DeleteObjectAsync(bucket, nextChallenge);
            return GameResults(game);
        }

        private static SkillResponse GameResults(Game game)
        {
            throw new NotImplementedException();
        }

        private static bool ConfirmedChallenge(Intent intent)
        {
            return intent.Slots[SlotNames.Opponent].ConfirmationStatus == ConfirmationStatus.Confirmed;
        }

        private static bool MovesRemaining(Intent intent)
        {
            return intent.Slots.Any(s => string.IsNullOrWhiteSpace(s.Value.Value));
        }

        private static SkillResponse AskAboutNextOpponent(SkillRequest request, Intent intent, string nextOpponent)
        {
            intent.Slots[SlotNames.Opponent].Value = nextOpponent;
            return ResponseBuilder.DialogConfirmSlot(Responses.NextChallenger(nextOpponent), SlotNames.Opponent,
                request.Session, intent);
        }

        private static bool RejectedChallenges(Intent intent)
        {
            return intent.Slots[SlotNames.Opponent].ConfirmationStatus == ConfirmationStatus.Denied;
        }

        private static string GetOpponentName(string userId, string opponentId)
        {
            return "yourself";
        }

        private static async Task<List<string>> GetChallenges(string userId)
        {
            var s3 = new AmazonS3Client();
            var objects = await s3.ListObjectsAsync(
                System.Environment.GetEnvironmentVariable("bucket"),
                $"challenge_{userId}"
            );
            return objects.S3Objects.Select(o => o.Key).ToList();
        }

        private static Move ParseMove(string value)
        {
            if (Enum.TryParse(value, true, out Move move))
            {
                return move;
            }

            throw new InvalidOperationException("Unknown move type " + value);
        }
    }
}