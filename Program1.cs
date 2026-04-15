using System;
using System.Collections.Generic;

namespace StrategyGameTextbasedPrototype
{
    public class TurnException : Exception
    {
        public TurnException(string message) : base(message)
        {

        }
    }


    class Game
    {
        public string name;
        public int timesetup;
        Player player1 = new Player();
        Player player2 = new Player();
        public int turns = 0;
        Func<Player, Player, bool> winningCond;
        Func<Player, Player, bool> losingCond;

        public List<Decision> thisGameDecisions = new List<Decision>();
        private void LogStats()
        {
            Console.WriteLine();
            Console.WriteLine($"===== STATS AFTER TURN {turns} =====");
            this.player1.PrintStats("Player 1");
            this.player2.PrintStats("Player 2");
            Console.WriteLine("====================================");
            Console.WriteLine();
        }
        private void CheckEndOfGame()
        {
            if (this.winningCond == null || this.losingCond == null)
                return; // nothing configured yet

            // Convention: winningCond(candidate, opponent)
            bool p1Won = winningCond(this.player1, this.player2);
            bool p2Won = winningCond(this.player2, this.player1);
            bool p1Lost = losingCond(this.player1, this.player2);
            bool p2Lost = losingCond(this.player2, this.player1);

            if (p1Won || p2Lost)
            {
                Console.WriteLine(">>> Player 1 wins!");
                LogStats();
                Environment.Exit(0);
            }

            if (p2Won || p1Lost)
            {
                Console.WriteLine(">>> Player 2 wins!");
                LogStats();
                Environment.Exit(0);
            }
        }

        public Game createGame(string name, int timesetup, Dictionary<string, Resources> ply1Rsr, Dictionary<string, Resources> ply2Rsr, int p1Health, int p2Health)
        {
            this.name = name;
            this.player1.resources = ply1Rsr;
            this.player2.resources = ply2Rsr;
            this.timesetup = timesetup;
            this.player1.Health = p1Health;
            this.player2.Health = p2Health;
            return this;
        }

        public Game AssignEachPlayerObjects(Dictionary<string, Resources> ply1MilRsr, Dictionary<string, Resources> ply2MilRsr)
        {
            this.player1.milResources = ply1MilRsr;
            this.player2.milResources = ply2MilRsr;
            return this;
        }

        public Game SetWinningLosing(Func<Player, Player, bool> WinningConditions, Func<Player, Player, bool> LosingConditions)
        {
            this.winningCond = WinningConditions;
            this.losingCond = LosingConditions;
            return this;
        }
        public Game SetDecisions(string DecisionName, Action<Player, Player> DecisionAction)
        {
            Decision assignment = new Decision();
            assignment.DecisionName = DecisionName;
            assignment.DecisionAction = DecisionAction;
            this.thisGameDecisions.Add(assignment);
            return this;
        }

        public Game TakeDecision(int playerCount, string DecisionName)
        {
            if (this.turns % 2 == 0)
            {
                if (playerCount != 1)
                {
                    throw new TurnException("Wrong Turn");
                }
            }
            else
            {
                if (playerCount != 2)
                {
                    throw new TurnException("Wrong Turn");
                }
            }

            var decision = thisGameDecisions.Find(d => d.DecisionName == DecisionName);
            if (decision == null)
                throw new InvalidOperationException($"Unknown decision '{DecisionName}'");

            decision.DecisionAction(this.player1, this.player2);
            turns++;

            LogStats();
            CheckEndOfGame();

            return this;
        }
        public static void Main(string[] args)
        {
        }
    }

    class Player
    {

        private static int playerCount = 0;

        public string playerName;
        public int Health;
        public Dictionary<string, Resources> resources = new Dictionary<string, Resources>();

        public Dictionary<string, Resources> milResources = new Dictionary<string, Resources>();

        public static int PlayerCount
        {
            get { return playerCount; }
        }

        public Player()
        {
            playerCount++;
        }

        public void PrintStats(string PlayerName)
        {
            Console.WriteLine($"Player: {PlayerName}\n");
            Console.WriteLine("Resources : \n");
            foreach (var kvp in this.resources)
            {
                Console.WriteLine($"{kvp.Key} {kvp.Value.PrintResource()} \n");
            }
            foreach (var bvp in this.milResources)
            {
                Console.WriteLine($"{bvp.Key} {bvp.Value.PrintResource()}\n");
            }
            Console.WriteLine($"Health of this player {this.Health} \n");
        }
    }

    class Resources
    {
        public string resourceName;
        public int currVal;
        public int maxVal;
        public int minVal;

        public Resources(string name, int currVal, int maxVal, int minVal)
        {
            this.resourceName = name;
            this.currVal = currVal;
            this.maxVal = maxVal;
            this.minVal = minVal;
        }

        public string Name
        {
            get { return resourceName; }
            set { resourceName = value; }
        }
        public int Curr
        {
            get { return currVal; }      // getter
            set { currVal = value; }     // setter
        }

        public int Max
        {
            get { return maxVal; }      // getter
            set { maxVal = value; }     // setter
        }

        public int Min
        {
            get { return minVal; }      // getter
            set { minVal = value; }     // setter
        }

        public bool aboveMax()
        {
            return currVal > maxVal;
        }

        public bool belowMin()
        {
            return currVal < minVal;
        }

        public string PrintResource()
        {
            string str = $"Resource_Name = {this.resourceName} Current_Value = {this.currVal} Max_value = {this.maxVal} Min_value = {this.minVal}";
            return str;
        }
    }

    class Decision
    {
        public string DecisionName;
        public Action<Player, Player> DecisionAction;
    }
}
