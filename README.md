# PokerEngine

PokerEngine is a deterministic poker rules engine written in C#.

## Features

- No-Limit Texas Hold'em
- Manual and automatic dealing
- Blinds, antes, straddles, and extra blinds
- Betting validation
- All-ins and side pots
- Uncalled bet returns
- Multiple boards and runouts
- Hand evaluation
- Event-based hand history
- Real-hand replay tests

## Status

The project is under active development. The public API may change before version `1.0.0`.

Planned support:

- Pot-Limit Omaha
- Fixed-Limit games
- NuGet distribution

## Requirements

- .NET 10 SDK

## Build

```bash
dotnet restore
dotnet build
dotnet test
```

## Manual mode

In manual mode, the caller posts blinds and deals cards.

```csharp
IPokerGame game = new NoLimitTexasHoldem(
    automation: Automation.None,
    smallBlind: 100,
    bigBlind: 200);

IPokerState state = game.CreateState();

state.Initialize([10_000, 10_000, 10_000]);

state.PlayerPost(0, PostType.SmallBlind, 100);
state.PlayerPost(1, PostType.BigBlind, 200);
state.Start();

state.DealHole(0, ["As", "Ad"]);
state.DealHole(1, ["Kh", "Qh"]);
state.DealHole(2, ["7c", "7d"]);

state.PlayerAction(2, ActionType.RaiseTo, 600);
state.PlayerAction(0, ActionType.Fold);
state.PlayerAction(1, ActionType.Fold);

//History
foreach(var e in state.Events)
{
    Console.WriteLine($"{e}");
}
```

## Automatic mode

In automatic mode, the engine posts blinds, shuffles the deck, and deals cards.

Player decisions are still passed through `PlayerAction`.

```csharp
IPokerGame game = new NoLimitTexasHoldem(
    automation: Automation.All,
    smallBlind: 100,
    bigBlind: 200);

IPokerState state = game.CreateState();

state.Initialize([10_000, 10_000, 10_000]);
state.Start();

state.PlayerAction(2, ActionType.RaiseTo, 600);
state.PlayerAction(0, ActionType.Fold);
state.PlayerAction(1, ActionType.Fold);

//History
foreach(var e in state.Events)
{
    Console.WriteLine($"{e}");
}
```

## Events

All state changes and hand results are stored in `state.Events`.

```csharp
foreach (PokerHandEvent handEvent in state.Events)
{
    Console.WriteLine(handEvent);
}
```

For example, pot winners can be read from `PotAwardedEvent`:

```csharp
foreach (PotAwardedEvent award in state.Events
             .OfType<PotAwardedEvent>())
{
    Console.WriteLine(
        $"Seat {award.SeatId} won {award.Amount} chips.");
}
```

A completed hand emits `EndHandEvent`.

## Card format

Cards use two-character notation:

```text
As = ace of spades
Kd = king of diamonds
Th = ten of hearts
2c = two of clubs
xx = unknown card
```

## Hand evaluation

PokerEngine includes a Texas Hold'em hand evaluator that selects the strongest five-card combination from the player's hole cards and the board.

```csharp
using PokerEngine.Evaluation;

IHandEvaluator evaluator = new TexasHoldemEvaluator();

HandRank result = evaluator.Evaluate(
    holeCards: ["As", "Ks"],
    boardCards: ["Qs", "Js", "Ts", "2d", "3c"]);

Console.WriteLine(result.Category);
Console.WriteLine(result.Strength);
Console.WriteLine(string.Join(", ", result.Cards));
```

## License

This project is licensed under the [MIT License](LICENSE).