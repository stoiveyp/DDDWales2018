using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

namespace NoTone
{
    public class Game
    {
        public Moves Player1 { get; set; }
        public Moves Player2 { get; set; }

        public Results ResultsFor(string userid)
        {
            var currentUser = Player1.UserId == userid ? Player1 : Player2;
            var opposition = currentUser == Player1 ? Player2 : Player1;

            return GetResults(currentUser, opposition);
        }

        private Results GetResults(Moves currentUser, Moves opposition)
        {
            var results = new List<Result>();
            return new Results(currentUser.MoveInformation.Select((m, i) => CalculateResult(m, opposition.MoveInformation[i])));
        }

        private Result CalculateResult(Move you, Move them)
        {
            var result = new Result
            {
                You = you.ToString(),
                Them = them.ToString(),
                Win = null,
                Description = " is matched by "
            };
            if (you == them)
            {
                return result;
            }

            var win = false;
            switch (you)
            {
                case Move.Rock:
                    win = them == Move.Scissors || them == Move.Lizard;
                    break;
                case Move.Paper:
                    win = them == Move.Rock || them == Move.Spock;
                    break;
                case Move.Scissors:
                    win = them == Move.Paper || them == Move.Lizard;
                    break;
                case Move.Lizard:
                    win = them == Move.Paper || them == Move.Spock;
                    break;
                case Move.Spock:
                    win = them == Move.Scissors || them == Move.Rock;
                    break;
            }

            result.Description = win ? WinDescription(you, them) : LoseDescription(them,you);

            result.Win = win;
            return result;
        }

        private string WinDescription(Move you, Move them)
        {
            switch (you)
            {
                case Move.Rock:
                    return "crushes";
                case Move.Paper:
                    return them == Move.Spock ? "disproves" : "wraps";
                case Move.Scissors:
                    return them == Move.Paper ? "cuts" : "decapitates";
                case Move.Lizard:
                    return them == Move.Paper ? "eats" : "poisons";
                case Move.Spock:
                    return them == Move.Scissors ? "smashes" : "vaporises";
            }

            throw new InvalidOperationException("invalid to win with this");
        }

        private string LoseDescription(Move them, Move you)
        {
            switch (you)
            {
                case Move.Rock:
                    return "is crushed by";
                case Move.Paper:
                    return them == Move.Spock ? "is totally disproved" : "is wrapped up in";
                case Move.Scissors:
                    return them == Move.Paper ? "is sliced by" : "has its head brutally removed by";
                case Move.Lizard:
                    return them == Move.Paper ? "is chewed up by" : "scurries in and poisons";
                case Move.Spock:
                    return them == Move.Scissors ? "are smashed by" : "is vaporised by";
            }

            throw new InvalidOperationException("invalid to lose with this");
        }
    }
}
