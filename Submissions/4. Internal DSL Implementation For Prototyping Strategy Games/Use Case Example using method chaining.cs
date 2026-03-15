```
var p1Resources = new Dictionary<string, Resources>
            {
                ["Gold"] = new Resources("Gold", 100, 1000, 0),
                ["Food"] = new Resources("Food", 50, 500, 0)
            };

            var p2Resources = new Dictionary<string, Resources>
            {
                ["Gold"] = new Resources("Gold", 80, 1000, 0),
                ["Food"] = new Resources("Food", 40, 500, 0)
            };

            var p1MilResources = new Dictionary<string, Resources>
            {
                ["Swords"] = new Resources("Swords", 10, 10, 0)
            };

            var p2MilResources = new Dictionary<string, Resources>
            {
                ["Swords"] = new Resources("Swords", 8, 10, 0)
            };

            // --- Fully method‑chained configuration + play sequence ---

            new Game()
                // basic game + health + economy resources
                .createGame(
                    name: "Test Battle",
                    timesetup: 20,
                    ply1Rsr: p1Resources,
                    ply2Rsr: p2Resources,
                    p1Health: 100,
                    p2Health: 100
                )
                // assign military resources
                .AssignEachPlayerObjects(p1MilResources, p2MilResources)
                // set win/lose: win if opponent health <= 0, lose if my health <= 0
                .SetWinningLosing(
                    (me, other) => other.Health <= 0,
                    (me, other) => me.Health <= 0
                )
                // define decisions (actions)
                .SetDecisions("Attack", (me, other) =>
                {
                    Console.WriteLine(">> Attack for 30 damage");
                    other.Health -= 30;
                })
                .SetDecisions("Heal", (me, other) =>
                {
                    Console.WriteLine(">> Heal for 20");
                    me.Health += 20;
                })
                // play a few turns, all chained
                .TakeDecision(1, "Attack")   // Turn 0: Player 1 attacks
                .TakeDecision(2, "Heal")     // Turn 1: Player 2 heals
                .TakeDecision(1, "Attack")   // Turn 2: Player 1 attacks again
                .TakeDecision(2, "Attack")   // Turn 3: Player 2 attacks
                .TakeDecision(1, "Attack");  // Turn 4: Player 1 attacks (might kill P2)
```