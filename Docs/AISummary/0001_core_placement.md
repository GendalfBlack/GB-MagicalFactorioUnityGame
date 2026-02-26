# AI Summary — Core.Placement

## Purpose
`Core.Placement` stores placement state on top of the grid and exposes event-driven hooks for scene systems.

## Main types
- `ElementType` (`Water`, `Fire`).
- `NodeId` (typed wrapper over `int`).
- `PlacedNode` (`Id`, `Type`, `Coord`).
- `IPlacementService` (contract).
- `PlacementService` (dictionary-backed implementation).

## Public contract
- Events: `NodePlaced`, `NodeRemoved`.
- Methods: `CanPlace`, `TryPlace`, `TryRemove`, `TryGet`.

## Behavior
- Placement is valid only when coord is inside `IGrid` bounds and currently empty.
- Successful placement increments node ID and emits `NodePlaced`.
- Successful removal emits `NodeRemoved`.
- `Try*` APIs return `false` for expected invalid operations.

## Cross-module links
- Depends on `Core.Grid` (`GridCoord`, `IGrid`).
- Instantiated in `Scenes.CompositionRoot`.
- Consumed by `Gameplay.Placement.BuildModeController` (commands) and `Gameplay.Placement.NodeSpawner` (events).
