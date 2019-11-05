using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kattis_PegGameForTwo
{
    enum Directions { E, SE, SW, W, NW, NE }

    public class Player
    {
        string name;
        internal int score;

        public Player(string n)
        {
            name = n;
            score = 0;
        }
    }

    public class Move
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

        public static int GetScoreDifference(Move currentMove, Move nextMove)
        {
            return currentMove.Score - nextMove.Score;
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

    public class Position
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
    public class Program
    {
        static int[,] pegs;
        static int[,] nextMovePegs;
        static int bestTurnScoreResult;
        static Player jacquez = new Player("Jaquez"), alia = new Player("Alia");
        static Move bestMove;
        static bool gameOver = false, checkingNextMove = false;
        static List<Position> holes = new List<Position>();
        static List<Position> nextMoveHoles = new List<Position>();
        private static Move nextBestMove, candidateNextMove;

        static void Main(string[] args)
        {
            pegs = new int[5, 5];

            for (int y = 0; y < 5; y++)
            {
                var input = Console.ReadLine();
                var stringNumbers = input.Split();
                ProcessLineOfPointValues(y, stringNumbers);
            }


            while(!gameOver)
            {
                TakeTurn(jacquez);
                PrintPegsState();
                if (!gameOver)
                {
                    TakeTurn(alia);
                    PrintPegsState();
                }
            }
            Console.WriteLine(jacquez.score - alia.score);
        }

        public static void TakeTurn(Player player)
        {
            bestMove = null;
            candidateNextMove = null;
            foreach (var hole in holes)
            {
                CheckForBestMove(hole);
                PrepareToCheckNextMove(bestMove);
                checkingNextMove = true;
                foreach (var nextMoveHole in nextMoveHoles)
                {
                    CheckForBestMove(nextMoveHole);
                    
                }
            }
            if (bestMove != null)
            {
                DoMove(bestMove, player);
            }
            else
            {
                gameOver = true;
            }
        }

        public static void ProcessLineOfPointValues(int y, string[] stringNumbers)
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
        public static void PrintPegsState()
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
        public static void CheckForBestMove(Position hole)
        {
            foreach (var direction in Enum.GetValues(typeof(Directions)))
            {
                if (direction is Directions dir)
                {
                    Position origin;
                    switch (dir)
                    {
                        case Directions.E:
                            origin = new Position(hole.y, hole.x + 2);
                            EvaluateMove(hole, origin);
                            break;
                        case Directions.SE:
                            origin = new Position(hole.y + 2, hole.x + 2);
                            EvaluateMove(hole, origin);
                            break;
                        case Directions.SW:
                            origin = new Position(hole.y + 2, hole.x);
                            EvaluateMove(hole, origin);
                            break;
                        case Directions.W:
                            origin = new Position(hole.y, hole.x - 2);
                            EvaluateMove(hole, origin);
                            break;
                        case Directions.NW:
                            origin = new Position(hole.y - 2, hole.x - 2);
                            EvaluateMove(hole, origin);
                            break;
                        case Directions.NE:
                            origin = new Position(hole.y - 2, hole.x);
                            EvaluateMove(hole, origin);
                            break;
                        // Repeat for the rest of the options
                        default:
                            throw new Exception("The switch statement in CheckForBestMove did not find a match in any of its' statements.");
                    }
                }
            }

        }

        public static void EvaluateMove(Position hole, Position origin)
        {
            if (ValidMove(hole, origin))
            {
                Position midpoint = FindMidpoint(hole, origin);
                EvaluateScore(hole, midpoint, origin);
            }
        }

        public static void EvaluateScore(Position hole, Position midpoint, Position origin)
        {
            Move candidate = new Move(hole, midpoint, origin);
            if (candidate.Score > 0 && (candidateNextMove == null || candidate.Score > candidateNextMove.Score))
            {
                if (!checkingNextMove) { candidateNextMove = candidate; } // How do I keep track of the difference between the best move the next player can possibly take? Am I overcomplicating this?
                else { nextBestMove = candidate; }
            }

        } // Check next turn's best moves as well and subtract that move's score from this one to find the score to evaluate

        public static Position FindMidpoint(Position hole, Position origin)
        {
            int midY = hole.y + (origin.y - hole.y) / 2;
            int midX = hole.x + (origin.x - hole.x) / 2;
            return new Position(midY, midX);
        }

        public static bool ValidMove(Position hole, Position origin)
        {
            bool validoriginY = origin.y < 5 && origin.y > -1;
            bool validoriginX = origin.x < 5 && origin.x > -1;
            bool validorigin = validoriginY && validoriginX;

            if(!validorigin || pegs[origin.y, origin.x] <= 0) { return false; }
            else { return true; }
        }

        public static void DoMove(Move bestmove, Player player)
        {
            if (bestmove.Score != 0)
            {
                pegs[bestmove.hole.y, bestmove.hole.x] = pegs[bestmove.origin.y, bestmove.origin.x];
                pegs[bestmove.origin.y, bestmove.origin.x] = pegs[bestmove.midpoint.y, bestmove.midpoint.x] = 0;
                player.score = bestmove.Score;
                UpdateHoleList(holes); 
            }
        }

        public static void PrepareToCheckNextMove(Move bestmove)
        {
            bestTurnScoreResult = 0;
            Array.Copy(pegs, nextMovePegs, pegs.Length);
            nextMoveHoles = holes.ToList();
            if (bestmove.Score != 0)
            {
                nextMovePegs[bestmove.hole.y, bestmove.hole.x] = pegs[bestmove.origin.y, bestmove.origin.x];
                nextMovePegs[bestmove.origin.y, bestmove.origin.x] = pegs[bestmove.midpoint.y, bestmove.midpoint.x] = 0;
                UpdateHoleList(nextMoveHoles);
            }
        }

        private static void UpdateHoleList(List<Position> list)
        {
            holes.Remove(bestMove.hole);
            holes.Add(bestMove.midpoint);
            holes.Add(bestMove.origin);
        }

        public static int GetScore(Position midpoint, Position origin)
        {
            return pegs[midpoint.y, midpoint.x] * pegs[origin.y, origin.x];
        }
    }
}
