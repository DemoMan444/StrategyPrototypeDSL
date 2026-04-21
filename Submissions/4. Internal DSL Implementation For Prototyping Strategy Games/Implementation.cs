using System;
using System.Collections.Generic;
using System.Linq;

namespace StrategyGameTextbasedPrototype
{
    // Type system
    public record ResourceId(string Name);
    public record UnitId(string Name);

    public class Resource
    {
        public ResourceId Id;
    }

    // Reference object for linking phase, allows to refer to resources by name before they are defined 
    public class ResourceReference
    {
        public ResourceId Id;

        // After resolution (linking phase)
        public Resource Resolved;
    }

    public class Unit
    {
        public UnitId Id;

        // References instead of raw IDs
        public List<(ResourceReference resource, int amount)> Costs = new();
    }

    // Expression system for decision effects
    public interface IExpression
    {
        int Evaluate(GameState state, PlayerState me, PlayerState other);
    }

    public class Constant : IExpression
    {
        public int Value;
        public int Evaluate(GameState s, PlayerState me, PlayerState other) => Value;
    }

    public class RandomExpr : IExpression
    {
        // Optional override option instead of using RandomConfig values
        public int? Min, Max;

        private static Random rand = new();

        public int Evaluate(GameState s, PlayerState me, PlayerState other)
        {
            var config = s.Game.ResolveRandom();

            int min = Min ?? config.Min;
            int max = Max ?? config.Max;

            return rand.Next(min, max + 1);
        }
    }

    public class Add : IExpression
    {
        public IExpression Left, Right;

        public int Evaluate(GameState s, PlayerState me, PlayerState other)
            => Left.Evaluate(s, me, other) + Right.Evaluate(s, me, other);
    }

    public class Decision
    {
        public string Name;
        public IExpression Damage;
    }

    public class PlayerState
    {
        public int Health = 100;
        public Dictionary<ResourceId, int> Resources = new();
    }

    public class GameState
    {
        public GameDefinition Game;

        public PlayerState Player1 = new();
        public PlayerState Player2 = new();
    }

    public class RandomConfig
    {
        public int Min = 0;
        public int Max = 10;
    }

    public class GameDefinition
    {
        public string Name;
        public GameDefinition Parent;

        public Dictionary<ResourceId, Resource> Resources = new();
        public Dictionary<UnitId, Unit> Units = new();
        public Dictionary<string, Decision> Decisions = new();

        public RandomConfig Random = new();

        // Lexical scoping: look in current game first, then parent
        // Hierarchical scoping: units can reference resources defined in parent game
        public Resource ResolveResource(ResourceId id)
        {
            if (Resources.ContainsKey(id))
                return Resources[id];

            return Parent?.ResolveResource(id);
        }

        public Decision ResolveDecision(string name)
        {
            if (Decisions.ContainsKey(name))
                return Decisions[name];

            return Parent?.ResolveDecision(name);
        }

        public RandomConfig ResolveRandom()
        {
            return Random ?? Parent?.ResolveRandom();
        }
    }

    // Builders
    public static class DSL
    {
        public static GameBuilder Game(string name) => new GameBuilder(name);
    }

    public class GameBuilder
    {
        private GameDefinition _game = new();

        public GameBuilder(string name)
        {
            _game.Name = name;
        }

        public GameBuilder Extends(GameDefinition parent)
        {
            _game.Parent = parent;
            return this;
        }

        public GameBuilder Resource(string name)
        {
            var id = new ResourceId(name);

            // Shadowing check: if parent has a resource with the same name, warn about it
            if (_game.Parent?.ResolveResource(id) != null)
                Console.WriteLine($"Warning: '{name}' shadows parent resource");

            _game.Resources[id] = new Resource { Id = id };
            return this;
        }

        public UnitBuilder Unit(string name)
        {
            var unit = new Unit { Id = new UnitId(name) };
            _game.Units[unit.Id] = unit;
            return new UnitBuilder(this, unit);
        }

        public DecisionBuilder Decision(string name)
        {
            var d = new Decision { Name = name };
            _game.Decisions[name] = d;
            return new DecisionBuilder(this, d);
        }

        public GameBuilder Randomness(int min, int max)
        {
            _game.Random = new RandomConfig { Min = min, Max = max };
            return this;
        }

        public GameDefinition Build()
        {
            // Validating here after building the whole object graph, instead of validating each step in the builders, allows to do more complex checks that require the full context (like circular inheritance or linking references)
            GameValidator.Validate(_game);
            return _game;
        }
    }

    public class UnitBuilder
    {
        private GameBuilder _parent;
        private Unit _unit;

        public UnitBuilder(GameBuilder parent, Unit unit)
        {
            _parent = parent;
            _unit = unit;
        }

        public UnitBuilder Costs(string resourceName, int amount)
        {
            _unit.Costs.Add(
                (new ResourceReference { Id = new ResourceId(resourceName) }, amount)
            );
            return this;
        }

        public GameBuilder End() => _parent;
    }

    public class DecisionBuilder
    {
        private GameBuilder _parent;
        private Decision _decision;

        public DecisionBuilder(GameBuilder parent, Decision decision)
        {
            _parent = parent;
            _decision = decision;
        }

        public DecisionBuilder Damage(IExpression expr)
        {
            _decision.Damage = expr;
            return this;
        }

        public GameBuilder End() => _parent;
    }

    // Validation and linking phase (Bettini)
    public static class GameValidator
    {
        public static void Validate(GameDefinition game)
        {
            ValidateNoCycles(game);
            LinkReferences(game);
            ValidateUnits(game);
            ValidateDecisions(game);
        }

        // Circular inheritance
        private static void ValidateNoCycles(GameDefinition game)
        {
            var visited = new HashSet<GameDefinition>();
            var current = game;

            while (current != null)
            {
                if (!visited.Add(current))
                    throw new Exception($"Circular inheritance at '{current.Name}'");

                current = current.Parent;
            }
        }

        // Linking phase
        private static void LinkReferences(GameDefinition game)
        {
            foreach (var unit in game.Units.Values)
            {
                foreach (var (refRes, _) in unit.Costs)
                {
                    refRes.Resolved = game.ResolveResource(refRes.Id);

                    if (refRes.Resolved == null)
                        throw new Exception($"Unknown resource '{refRes.Id.Name}'");
                }
            }
        }

        private static void ValidateUnits(GameDefinition game)
        {
            foreach (var unit in game.Units.Values)
            {
                if (!unit.Costs.Any())
                    Console.WriteLine($"Warning: Unit '{unit.Id.Name}' has no cost");
            }
        }

        private static void ValidateDecisions(GameDefinition game)
        {
            foreach (var d in game.Decisions.Values)
            {
                if (d.Damage == null)
                    throw new Exception($"Decision '{d.Name}' missing damage");
            }
        }
    }
    
    // public class GameEngineOld
    // Had _policy
    // void LogStats
    // void CheckEndOfGame
    
    // Engine
    public class GameEngine
    {
        private GameDefinition _game;
        public GameState _state = new();

        public GameEngine(GameDefinition game)
        {
            _game = game;
            _state.Game = game;
        }

        public void Execute(string decisionName)
        {
            var decision = _game.ResolveDecision(decisionName);

            int dmg = decision.Damage.Evaluate(_state, _state.Player1, _state.Player2);

            Console.WriteLine($"Decision {decisionName} deals {dmg}");

            _state.Player2.Health -= dmg;
        }
    }
}