# AI Summary — Gameplay

## Purpose
`Gameplay` is the scene-facing layer that turns Core services into player interactions and visuals.

## Main components
- `PlayerController2D`: movement.
- `PlayerGridProbe`: player world -> `GridCoord`.
- `BuildModeController`: targeting, range checks, placement/removal input.
- `NodeSpawner`: reacts to placement events and spawns/destroys prefabs.
- `NodeView`: stores node runtime metadata on spawned objects.
- `GridGizmosRenderer`: editor grid drawing from config.
- `MouseWorld`: mouse screen -> world helper.

## Cross-module links
- Reads `Grid` and `Placement` from `Scenes.CompositionRoot`.
- Uses `Core.Grid` for coordinate math and conversions.
- Uses `Core.Placement` for state changes and events.
- Uses `GameData.GridConfigSO` for gizmo rendering.

## Runtime flow
1. Probe computes player coord.
2. Build controller computes target coord and range validity.
3. LMB/RMB trigger placement service updates.
4. Spawner syncs scene objects with placement events.
