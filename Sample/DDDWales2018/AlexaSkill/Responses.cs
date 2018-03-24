using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using BoredomInc;

namespace AlexaSkill
{
    public static class Responses
    {
        public const string Draw = "It's a draw, maybe you'll get the upper hand next time";
        public const string Win = "You won! Well done. Hope you have another game soon";
        public const string Loss = "You didn't win this time, maybe next time";
        public const string GameCreated = "The challenge has been sent, check your challenges to find out the result";
        public const string NoChallenges = "No challenges so far, please try again soon";
        public const string ChallengeLength = "You have {0} challenges available right now.";
        public const string Close = "Okay. Hope you play another game soon.";

        public const string Help = "A game is three rounds of rock paper scissors lizard spock. You can find out what moves you can play, start a new game or check your challenges from other players";
        public const string Moves = @"Scissors cuts Paper,
Paper covers Rock,
Rock crushes Lizard,
Lizard poisons Spock,
Spock smashes Scissors,
Scissors decapitates Lizard,
Lizard eats Paper,
Paper disproves Spock,
Spock vaporizes Rock,
and, as it always has,
Rock crushes Scissors";

        public static string PendingChallenges(int length) => string.Format(ChallengeLength, length);

        public static IOutputSpeech NextChallenger(string nextOpponent)
        {
            return new PlainTextOutputSpeech { Text = $"Your next challenger is {nextOpponent}, do you accept the challenge?" };
        }

        public static Speech Welcome => new Speech(
            new Paragraph(
                new PlainText("Welcome to"),
                new Prosody(new Sentence("rock paper scissors lizard spock")) { Rate = ProsodyRate.Fast },
                new Sentence("Do you want to start a game or check your challenges?")
            ));

        public static string Results(Results results)
        {
            var sb = new StringBuilder("Here are the results.  ");
            foreach (var result in results.ResultInformation)
            {
                sb.Append($"Your {result.You} {result.Description} their {result.Them}.  ");
            }

            if (!results.OverallWin.HasValue)
            {
                sb.Append(Responses.Draw);
            }
            else
            {
                sb.Append(results.OverallWin.Value ? Responses.Win : Responses.Loss);
            }

            return sb.ToString();
        }
    }
}