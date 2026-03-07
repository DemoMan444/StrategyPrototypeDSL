using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace StrategyGameTextbasedPrototype
{
    public class ConditionException : Exception
    {
        public ConditionException(string message) : base(message)
        {

        }
    }


    class Game
    {
        public string name;
        Player player1 = new Player();
        Player player2 = new Player();

        List<Rule> winconditions = new List<Rule>();
        List<Rule> loseconditions = new List<Rule>();
        public Game createGame(string name, Dictionary<string, Resources> ply1Rsr, Dictionary<string, Resources> ply2Rsr)
        {
            this.name = name;
            this.player1.resources = ply1Rsr;
            this.player2.resources = ply2Rsr;
            return this;
        }

        public Game playerInitialization(int p1Health, int p2Health)
        {
            this.player1.Health = p1Health;
            this.player2.Health = p2Health;
            return this;
        }

        public Game setWinCondition(List<Rule> winCond)
        {
            this.winconditions = winCond;
            return this;
        }

        public Game setloseCondition(List<Rule> loseCond)
        {
            this.winconditions = loseCond;
            return this;
        }

        // public Player 

        public Game setAction(string name, Player player, List<Rule> preCond)
        {
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
        public int Health = 100;
        public Dictionary<string, Resources> resources = new Dictionary<string, Resources>();

        public Dictionary<string, object> Stats = new Dictionary<string, object>();

        public List<Action> actions = new List<Action>();

        public static int PlayerCount
        {
            get { return playerCount; }
        }

        public Player()
        {
            playerCount++;
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
    }
    class Rule
    {
        public string name;
        public Player PlayerRef1;
        public Player PlayerRef2;
        private List<object> rules = new List<object>();

        public Rule(string name, List<object> obj, Player ply1, Player ply2)
        {
            this.PlayerRef1 = ply1;
            this.PlayerRef1 = ply1;
            this.name = name;
            this.rules = obj;
        }

        public static bool ParseRules(List<Rule> rules)
        {
            bool retval = true;
            foreach (var rule in rules)
            {
                retval &= rule.ParseRule();
            }
            return retval;
        }
        // Example: evaluate all rules with logical AND
        public bool ParseRule()
        {
            bool result = true;
            foreach (var r in rules)
            {
                if (r is Conditions c)
                    result &= c.ParseCondition();
                else if (r is bool b)
                    result &= b;
                else
                    throw new ConditionException("Unsupported rule type");
            }
            return result;
        }
    }

    class Conditions
    {
        public string Type;        // unused here but you can keep it
        public object LeftOperand;
        public object RightOperand;
        public string Operand;     // "and" / "or"

        public bool ParseCondition()
        {
            bool left = EvaluateOperand(LeftOperand);
            bool right = EvaluateOperand(RightOperand);

            switch (Operand.Trim().ToLowerInvariant())
            {
                case "or":
                    return left || right;
                case "and":
                    return left && right;
                default:
                    throw new ConditionException("Invalid operator in condition");
            }
        }

        private bool EvaluateOperand(object op)
        {
            if (op is Conditions nested)
                return nested.ParseCondition();       // recursive for nested condition
            if (op is bool b)
                return b;

            throw new ConditionException("Operand must be bool or Conditions");
        }
    }

    class Action
    {
        List<Rule> preconditions = new List<Rule>();
    }

}
