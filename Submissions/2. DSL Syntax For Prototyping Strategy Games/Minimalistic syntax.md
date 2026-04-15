Something minimalistic that works.

 

I asked myself, “Which parts are necessary to keep for a very minimal strategy game playthrough in text?”

 

I assume having:

a resource and time setup
units and buildings (as numbers with representative strengths and weaknesses)
randomness
decisions to make
winning and losing conditions.
 

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