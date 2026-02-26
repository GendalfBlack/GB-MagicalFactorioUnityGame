# AI Summary — Core.Grid

## Purpose
`Core.Grid` is the pure math layer for grid coordinates, bounds, and world/grid conversion.

## Main types
- `GridCoord`: immutable integer cell coordinate.
- `GridRect`: grid bounds with `Contains` and `Clamp`.
- `GridSettings`: `CellSize`, `OriginWorld`, `Bounds`.
- `IGrid`: grid service contract.
- `GridService`: concrete implementation.
- `GridDirections`: cardinal direction constants.

## Behavior highlights
- `WorldToGrid` uses `floor` on local coordinates.
- `GridToWorldCenter` maps cell index to center point.
- `SnapWorldToCellCenter` can optionally clamp into bounds.
- `GridRect` and `GridSettings` validate constructor input.

## Cross-module links
- Built by `Scenes.CompositionRoot` from `GameData.GridConfigSO`.
- Queried by `Core.Placement` for placement validation.
- Used directly in gameplay probes/controllers and gizmo rendering.
