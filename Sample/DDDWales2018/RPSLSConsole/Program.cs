using System;
using System.Collections.Generic;
using NoTone;

namespace RPSLSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Player One");
            var playerOneOne = Console.ReadLine();
            var playerOneTwo = Console.ReadLine();
            var playerOneThree = Console.ReadLine();

            Console.WriteLine("Player Two");
            var playerTwoOne = Console.ReadLine();
            var playerTwoTwo = Console.ReadLine();
            var playerTwoThree = Console.ReadLine();

            var game = new Game
            {
                Player1 = new Moves
                {
                    UserId = "fake1",
                    MoveInformation = new List<Move>
                    {
                        ParseMove(playerOneOne),
                        ParseMove(playerOneTwo),
                        ParseMove(playerOneThree),
                    }
                },
                Player2 = new Moves
                {
                    UserId = "fake2",
                    MoveInformation = new List<Move>
                    {
                        ParseMove(playerTwoOne),
                        ParseMove(playerTwoTwo),
                        ParseMove(playerTwoThree),
                    }
                }
            };

            var results = game.ResultsFor("fake1");

            foreach (var result in results.ResultInformation)
            {
                Console.WriteLine($"your {result.You} {result.Description} {result.Them} - {result.Win}");
            }

            Console.WriteLine($"Overall win? {results.OverallWin}");
            Console.Read();
        }

        public static Move ParseMove(string value)
        {
            if (Enum.TryParse(value, true, out Move move))
            {
                return move;
            }

            throw new InvalidOperationException("Unknown move type " + value);
        }
    }

}
