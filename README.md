# StrategyPrototypeDSL

## Initial Idea: DSL for Prototyping Text-Based Strategy Games

This project proposes a platform that allows game developers to prototype and play through their strategy game ideas. It enables them to take notes, evaluate outcomes, and analyze results during the early design phase.

 

## Benefits for Users (Developers)

### Game Planning

Makes it easier to think through a strategy game concept before building it, supporting early prototyping.
Enables fast planning and testing of different strategy games or simulations, such as economic simulations.

### Supporting Development of the Real Game

Maintaining a clear vision:
Helps developers understand what they are building and how it should function before serious prototyping or coding begins.
By defining a structured world with strict constraints, the platform saves time and helps teams — especially small teams with limited project-management resources — stay focused.
Reducing coding effort:
Some code generated through the DSL may be reusable in the final game, either directly or as interfaces/abstract classes. Examples include:
Core systems like health and damage logic could already be defined
AI behavior (optional)
Event systems (e.g., hunger, disease)
Other elements may still require additional implementation (e.g., unit movement)

 

## Technical Overview

Prototype games are created in C# using the DSL.
The system is delivered as a web application using .NET to:

Send content (images, text, etc.) to the frontend
Handle AI connections (preferably LLMs)
Manage database interaction for replayability and statistics collection (e.g., determining which games perform best)

## DSL Scope

### DSL may be used for:

POST/GET operations (if meaningful)
AI integration (if meaningful)
Collecting user input, feedback, and preferences (such as liked AI outputs) and analyzing/modifying this data for statistical purposes
Generating randomness within the game (if meaningful)

### DSL not intended for:

Database connections
Saving JSON files locally may be more practical than building full database integration
 

## Example User Journey

Once the DSL is ready, a typical user interaction would be:

Through the interface, the user defines:
Game rules (what actions are possible)
Rewards and punishments
Win and loss conditions
Resources (e.g., wood, stone, food, population)
The user plays the game.
The user provides feedback during or after gameplay.
The user repeats gameplay and refinement.
Eventually, the user views statistics summarizing preferences or outcomes.
 

## Project Output

The final deliverable is a DSL and interface that allow users to define:

Rules for a (limited-scope) text-based strategy game
Reward and punishment systems
Win/lose conditions
Resource types (wood, stone, food, people, etc.)
Scene images or model visuals
Basic game AI setup and training defined by the user (optional, non-functional requirement)