using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// Game simulation

Console.WriteLine("=== Starting Text-Based Strategy Game ===\n");

// Creating the game definition
var gameDef = StrategyGameTextbasedPrototype.DSL.Game("Test Battle")
    .Resource("Gold")
    .Resource("Food")
    // Swords can be used by units
    .Resource("Swords")

    .Unit("Swordsman")
        .Costs("Gold", 30)
        .Costs("Food", 15)
        .End()

    .Unit("Archer")
        .Costs("Gold", 25)
        .Costs("Food", 10)
        .End()

    .Decision("Attack")
        .Damage(new StrategyGameTextbasedPrototype.Constant { Value = 30 })
        .End()

    .Decision("HeavyAttack")
        .Damage(new StrategyGameTextbasedPrototype.Add
        {
            Left = new StrategyGameTextbasedPrototype.Constant { Value = 25 },
            Right = new StrategyGameTextbasedPrototype.RandomExpr { Min = 5, Max = 15 }
        })
        .End()

    .Decision("Heal")
        .Damage(new StrategyGameTextbasedPrototype.Constant { Value = -20 })
        .End()

    .Randomness(min: 0, max: 100)
    
    // Validation and linking happens in build
    .Build();

Console.WriteLine("? Game definition built and validated successfully!\n");

// Running the game
var engine = new StrategyGameTextbasedPrototype.GameEngine(gameDef);

Console.WriteLine("=== Game Simulation Starts ===\n");

engine.Execute("Attack");        // Player 1 Attacks
engine.Execute("Heal");          // Player 2 Heals
engine.Execute("HeavyAttack");   // Player 1 Heavy attacks
engine.Execute("Attack");        // Player 2 Attacks
engine.Execute("HeavyAttack");   // Player 1 Heavy attacks again

Console.WriteLine("=== Game simulation finished ===\n");
Console.WriteLine("Final Health Player 1: " + engine._state.Player1.Health);
Console.WriteLine("Final Health Player 2: " + engine._state.Player2.Health);

app.Run();