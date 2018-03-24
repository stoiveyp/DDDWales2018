using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Alexa.NET.Response.Ssml.SoundLibrary;
using NoTone;

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


        public const String Welcome =
            "Welcome to rock paper scissors lizard spock. Do you want to start a game or check your challenges?";

        public static string Results(Results results)
        {
            var sb = new StringBuilder("Here are the results.  ");
            foreach (var result in results.ResultInformation)
            {
                sb.Append($"Your {result.You} {result.Description} their {result.Them}.  ");
            }

            if (!results.OverallWin.HasValue)
            {
                sb.Append(Draw);
            }
            else
            {
                sb.Append(results.OverallWin.Value ? Win : Loss);
            }

            return sb.ToString();
        }

        public static Speech EmotiveResults(Results results)
        {
            var speech = new Speech();
            speech.Elements.Add(new Paragraph(new Sentence("Here are the results")));

            foreach (var result in results.ResultInformation)
            {
                var moveResult = new Paragraph(
                    new Sentence($"Your {result.You}"),
                    new Break { Strength = BreakStrength.Strong},
                    new Emphasis(result.Description),
                    new Sentence($"their { result.Them }")
                    
                    );
                speech.Elements.Add(moveResult);

                if (result.Win.HasValue)
                {
                    speech.Elements.Add(result.Win.Value ? WinEmotion() : LoseEmotion());
                }

            }

            if (!results.OverallWin.HasValue)
            {
                speech.Elements.Add(new Sentence(Draw));
            }
            else if(results.OverallWin.Value)
            {
                speech.Elements.Add(new Sentence(Win));
                speech.Elements.Add(WinEmotion());
            }
            else
            {
                speech.Elements.Add(new Sentence(Loss));
                speech.Elements.Add(LoseEmotion());
            }

            return speech;
        }

        private static readonly Audio[] WinEmotions = {
            Human.CrowdApplause01,
            Human.CrowdApplause02,
            Human.CrowdApplause03,
            Human.CrowdApplause04,
            Human.CrowdApplause05
        };

        private static readonly Audio[] LoseEmotions = {
            Human.CrowdBoo01,
            Human.CrowdBoo02,
            Human.CrowdBoo03
        };
        private static Audio WinEmotion()
        {
            return WinEmotions[new Random().Next(0, WinEmotions.Length - 1)];
        }

        private static Audio LoseEmotion()
        {
            return LoseEmotions[new Random().Next(0, WinEmotions.Length - 1)];
        }
    }
}