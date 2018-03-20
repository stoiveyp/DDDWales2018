using Alexa.NET.Response;

namespace AlexaSkill
{
    public static class Responses
    {
        public const string Welcome = "Welcome to rock, paper, scissors, lizard, spock. Do you want to start a game or check your challenges?";
        public const string GameCreated = "The challenge has been sent, check your challenges to find out the result";
        public const string NoChallenges = "No challenges so far, please try again soon";
        public const string ChallengeLength = "You have {0} challenges available right now.";

        public static string PendingChallenges(int length) => string.Format(ChallengeLength, length);

        public static IOutputSpeech NextChallenger(string nextOpponent)
        {
            return new PlainTextOutputSpeech{Text=$"Your next challenger is {nextOpponent}, do you accept the challenge?"};
        }
    }
}