using System;
using System.Collections.Generic;
using System.Text;

namespace Kattis_PegGameForTwo
{
    enum Directions { E, SE, SW, W, NW, NE }

    class Player
    {
        string name;
        internal int score;

        public Player(string n)
        {
            name = n;
            score = 0;
        }
    }

    class Move
    {
        int score;
        internal Position hole, midpoint, origin;
        public Move(Position h, Position m, Position o)
        {
            hole = h; midpoint = m; origin = o;
        }
        public int Score
        {
            get
            {
                if (score == 0)
                {
                    score = Program.GetScore(midpoint, origin);
                }
                return score;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Move move)
            {
                    return move.hole.Equals(hole) && move.midpoint.Equals(midpoint) 
                        && move.origin.Equals(origin);
            }
            return false;
        }
    }

    class Position
    {
        internal int y, x;
        public Position(int yN, int xN)
        {
            y = yN;
            x = xN;
        }

        public override string ToString()
        {
            return $"{y}, {x}";
        }

        public static Position CopyPair(Position pair)
        {
            return new Position(pair.y, pair.x);
        }

        public override bool Equals(object obj)
        {
            if(obj is Position pos)
            {
                return pos.y == y && pos.x == x;
            }
            return false;
        }
    }
    class Program
    {
        static int[,] pegs;
        static Player jacquez = new Player("Jaquez"), alia = new Player("Alia");
        static Move bestmove;
        static bool gameOver = false;
        static List<Position> holes = new List<Position>();
        static void Main(string[] args)
        {
            pegs = new int[5, 5];

            for (int y = 0; y < 5; y++)
            {
                var input = Console.ReadLine();
                var stringNumbers = input.Split();
                ProcessLineOfPointValues(y, stringNumbers);
            }

            PrintPegsState();

            while(!gameOver)
            {
                TakeTurn(jacquez);
                if (!gameOver)
                {
                    TakeTurn(alia);
                }
            }
            Console.WriteLine(jacquez.score - alia.score);
        }

        private static void TakeTurn(Player player)
        {
            foreach (var hole in holes)
            {
                CheckForBestMove(hole, player);
            }
            if (bestmove != null)
            {
                DoMove(bestmove, player);
            }
            else
            {
                gameOver = true;
            }
        }

        private static void ProcessLineOfPointValues(int y, string[] stringNumbers)
        {
            for (int x = 0; x < 5; x++)
            {
                if (x < stringNumbers.Length)
                {
                    var number = int.Parse(stringNumbers[x]);
                    pegs[y, x] = number;
                    if (number == 0)
                    {
                        holes.Add(new Position(y, x));
                    }
                }
                else
                {
                    pegs[y, x] = -1;
                }
            }
        }

        //ToDo: Remove this debug output function
        private static void PrintPegsState()
        {
            for (int y = 0; y < 5; y++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int x = 0; x < 5; x++)
                {
                    var val = pegs[y, x];
                    if (val > -1)
                    {
                        stringBuilder.Append(val + "\t");
                    }
                }
                Console.WriteLine(stringBuilder);
            }
        }
        private static void CheckForBestMove(Position hole, Player player)
        {
            Position original = Position.CopyPair(hole);
            foreach (var direction in Enum.GetValues(typeof(Directions)))
            {
                if (direction is Directions dir)
                {
                    Position origin;
                    switch (dir)
                    {
                        case Directions.E:
                            origin = new Position(hole.y, hole.x + 2);
                            if (ValidMove(hole, origin))
                            {
                                Position midpoint = new Position(hole.y, hole.x + 1);
                                EvaluateMove(hole, midpoint, origin);
                            }
                            break;
                        case Directions.SE:
                            origin = new Position(hole.y + 2, hole.x + 2);
                            if (ValidMove(hole, origin))
                            {
                                Position midpoint = new Position(hole.y + 1, hole.x + 1);
                                EvaluateMove(hole, midpoint, origin);
                            }
                            break;
                        default:
                            throw new Exception("The switch statement in CheckForBestMove did not find a match in any of its' statements.");
                    }
                }
            }

        }

        private static void EvaluateMove(Position hole, Position midpoint, Position origin)
        {
            
        }

        private static bool ValidMove(Position hole, Position origin)
        {
            bool validoriginY = origin.y < 5 && origin.y > -1;
            bool validoriginX = origin.x < 5 && origin.x > -1;
            bool validorigin = validoriginY && validoriginX;

            if(!validorigin || pegs[origin.y, origin.x] <= 0) { return false; }
            else { return true; }
        }

        private static void DoMove(Move bestmove, Player player)
        {
            pegs[bestmove.hole.y, bestmove.hole.x] = pegs[bestmove.origin.y, bestmove.origin.x];
            pegs[bestmove.origin.y, bestmove.origin.x] = 0;
            player.score = bestmove.Score;
        }

        public static int GetScore(Position midpoint, Position origin)
        {
            return pegs[midpoint.y, midpoint.x] * pegs[origin.y, origin.x];
        }
    }
}
