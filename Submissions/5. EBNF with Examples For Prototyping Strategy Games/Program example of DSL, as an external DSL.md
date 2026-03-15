Example 1:

StrategyGameTextbasedPrototype
CreateGame(
StartingResources: Gold=5000, Stone=500, Food=100,
MaxResources: Gold=10000, Stone=1000, Food=100,
TimeSetup: TurnBased 15
)
AssignEachPlayerObjects(Units: Bowman=100, Swordsman=100)
SetRandomness(Randomness: Attacking vs DefendingWithEvery100StoneGivesDisadvantageNr -> [-0.05, -0.25])
SetDecisions(Decisions: ThereIsSiegeWar, NoFoodProduction, Attack, Defend, Build, Recruit)
SetWinningLosing(
    Victory: TwentyThousandMoreGoldThanEnemy|ThousandUnitsMoreThanEnemy,
    Defeat: TwentyThousandLessGoldThanEnemy|ThousandUnitsLessThanEnemy
)