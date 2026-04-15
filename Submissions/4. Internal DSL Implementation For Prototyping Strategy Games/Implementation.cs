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

    public class Randomness
    {
        //public int StoneThreshold { get; set; } = 100;
        public float MinDisadvantage { get; set; } = -0.05f;
        public float MaxDisadvantage { get; set; } = -0.25f;

        // More randomness rules
    }

    public class Game
    {
        public string name;
        public int timesetup;
        public Player player1 = new Player();
        public Player player2 = new Player();
        public int turns = 0;

        private Func<Player, Player, bool> winningCond;
        private Func<Player, Player, bool> losingCond;
        public List<Decision> thisGameDecisions = new List<Decision>();
        
        private Randomness _randomness = new Randomness();

        public List<Decision> AvailableDecisions { get; } = new List<Decision>();

        private void LogStats()
        {
            Console.WriteLine($"\n-- Stats after each turn {turns} --");
            player1.PrintStats("Player 1");
            player2.PrintStats("Player 2");
            Console.WriteLine("----\n");
        }

        private void CheckEndOfGame()
        {
            if (winningCond == null || losingCond == null)
            {
                // nothing configured yet
                return; 
            }

            bool p1Won = winningCond(player1, player2);
            bool p2Won = winningCond(player2, player1);
            bool p1Lost = losingCond(player1, player2);
            bool p2Lost = losingCond(player2, player1);

            if (p1Won || p2Lost)
            {
                Console.WriteLine("Player 1 wins!");
                LogStats();
                return;
            }
            if (p2Won || p1Lost)
            {
                Console.WriteLine("Player 2 wins!");
                LogStats();
                return;
            }
        }

        public Game createGame(string name, int timesetup, Dictionary<string, Resources> ply1Rsr,
            Dictionary<string, Resources> ply2Rsr, int p1Health, int p2Health)
        {
            this.name = name;
            this.timesetup = timesetup;
            this.player1.resources = ply1Rsr;
            this.player2.resources = ply2Rsr;
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

        public Game AssignEachPlayerObjectsSame(
            Dictionary<string, Resources> units,
            Dictionary<string, Resources> buildings)
        {
            return AssignEachPlayerObjects(units, units); // same for both players
        }

        //public Game SetRandomness(int stoneThreshold, float minDisadvantage, float maxDisadvantage)
        public Game SetRandomness(float minDisadvantage, float maxDisadvantage)
        {
            //_randomness.StoneThreshold = stoneThreshold;
            _randomness.MinDisadvantage = minDisadvantage;
            _randomness.MaxDisadvantage = maxDisadvantage;
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
            thisGameDecisions.Add(new Decision { DecisionName = DecisionName, DecisionAction = DecisionAction });
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
            {
                throw new InvalidOperationException($"Unknown decision '{DecisionName}'");
            }
            
            Console.WriteLine($"\n-- Player {playerCount} chooses: {DecisionName} --");
            decision.DecisionAction(player1, player2);

            turns++;
            LogStats();
            CheckEndOfGame();
            return this;
        }
    }

    public class Player
    {
        // Not used
        private static int playerCount = 0;
        // Not used
        public string playerName;

        public int Health;
        public Dictionary<string, Resources> resources = new();
        public Dictionary<string, Resources> milResources = new();

        // Not used
        public static int PlayerCount
        {
            get { return playerCount; }
        }

        public Player()
        {
            playerCount++;
        }

        public void PrintStats(string label)
        {
            Console.WriteLine($"-- {label} --");
            Console.WriteLine($"Health: {Health}");
            Console.WriteLine("Economy:");
            foreach (var r in resources.Values) Console.WriteLine($" {r.PrintResource()}");
            Console.WriteLine("Military:");
            foreach (var r in milResources.Values) Console.WriteLine($" {r.PrintResource()}");
            Console.WriteLine();
        }
    }

    public class Resources
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

        // Unused part of the code atm
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

        public bool AboveMax()
        {
            return currVal > maxVal;
        }

        public bool BelowMin()
        {
            return currVal < minVal;
        }
        // Unused part of the code end atm

        public string PrintResource() => $"{resourceName}: {currVal} (max:{maxVal}, min:{minVal})";
    }

    public class Decision
    {
        public string DecisionName;
        public Action<Player, Player> DecisionAction;
    }
}