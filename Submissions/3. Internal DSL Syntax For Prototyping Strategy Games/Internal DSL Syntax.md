Chosen Language: C# due to personal preference (experience) and Unity

 

Internal DSL Syntax present

Method chaining, fluent API
Function sequence - it’s kinda there but not strictly enforced at the moment
 

Internal DSL Syntax not present

Nested function - could make things and reading more complicated
Closures - could make things and reading more complicated
 

Internal DSL Patterns used

Metamodel (Semantic Model) - yes, classes will be created for implementation
Expression builder (fluent API) - I use method chaining
Construction Builder – game is built 1 time and it is immutable(at least resources are)
 

Internal DSL Patterns not used at the moment (may change)

Context Variable - good potential to be used as could be similar to code in Unity for example when assigning player gold
Symbol table - good potential and probably needed for implementation and its safety
Class symbol table - good potential maybe to be used
 

Main example:

 

StrategyGameTextbasedPrototype

.CreateGame(StartingResourcesList, MaxResourcesList, TimeSetup)

.AssignEachPlayerObjects(StartingAndOnlyAvailableUnits, StartingAndOnlyAvailableBuildings)

.SetRandomness(UnitsAttackingAgainstThisNrOfStoneGetThisNrOfRandomDisadvantage)

.SetDecisions(StartingDecisionsAlreadyMade, DecisionsAvailable)

.SetWinningLosing(WinningConditions, LosingConditions)

 

Example:

For Stronghold Crusader:

StrategyGameTextbasedPrototype

.CreateGame(

    [Player1: [gold: 5000, stone: 500, food: 100],

     Player2: [gold: 10000, stone: 1000, food: 50]],

    [maxGold: 10000, maxStone: 1000, maxFood: 100],

    RealTime

)

.AssignEachPlayerObjects(

    Same(

        [bowman: 100, swordsman: 100],

        [stoneProducingBuilding: 2, marketPlace: 1]

    )

)

.SetRandomness(ForEveryNrOfStone: 100, RandomnessDisadvantageRange: [-0.05, -0.25])

.SetDecisions(ThereIsSiegeWar, NoFoodProduction, Attack, Defend, Build, Recruit)

.SetWinningLosing(

    TwentyThousandMoreGoldThanEnemy/ThousandUnitsMoreThanEnemy,

    TwentyThousandLessGoldThanEnemy/ThousandUnitsLessThanEnemy

)

