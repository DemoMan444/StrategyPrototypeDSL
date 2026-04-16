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

var p1Resources = new Dictionary<string, StrategyGameTextbasedPrototype.Resources>
{
    ["Gold"] = new StrategyGameTextbasedPrototype.Resources("Gold", 100, 1000, 0),
    ["Food"] = new StrategyGameTextbasedPrototype.Resources("Food", 50, 500, 0)
};

var p2Resources = new Dictionary<string, StrategyGameTextbasedPrototype.Resources>
{
    ["Gold"] = new StrategyGameTextbasedPrototype.Resources("Gold", 80, 1000, 0),
    ["Food"] = new StrategyGameTextbasedPrototype.Resources("Food", 40, 500, 0)
};

var p1MilResources = new Dictionary<string, StrategyGameTextbasedPrototype.Resources>
{
    ["Swords"] = new StrategyGameTextbasedPrototype.Resources("Swords", 10, 10, 0)
};

var p2MilResources = new Dictionary<string, StrategyGameTextbasedPrototype.Resources>
{
    ["Swords"] = new StrategyGameTextbasedPrototype.Resources("Swords", 8, 10, 0)
};

var game = new StrategyGameTextbasedPrototype.Game()
    .createGame("Test Battle", 20, p1Resources, p2Resources, 100, 100)
    .AssignEachPlayerObjects(p1MilResources, p2MilResources)
    ._policy.SetWinningLosing(
        (me, other) => other.Health <= 0,
        (me, other) => me.Health <= 0)
    .SetDecisions("Attack", (me, other) =>
    {
        Console.WriteLine(">> Attack for 30 damage!");
        other.Health -= 30;
    })
    .SetDecisions("Heal", (me, other) =>
    {
        Console.WriteLine(">> Heal for 20 health!");
        me.Health += 20;
    });

/*
game.TakeDecision(1, "Attack")
    .TakeDecision(2, "Heal")
    .TakeDecision(1, "Attack")
    .TakeDecision(2, "Attack")
    .TakeDecision(1, "Attack");
*/
Console.WriteLine("=== Game simulation finished ===\n");


app.Run();