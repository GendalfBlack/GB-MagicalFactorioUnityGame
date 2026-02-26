# AI Summary — Scenes (CompositionRoot)

## Purpose
`Scenes` provides the composition entry point that wires Core services for a scene.

## Main component
- `Scenes.CompositionRoot`
  - input dependency: `GridConfigSO`
  - outputs: `IGrid Grid`, `IPlacementService Placement`

## Initialization sequence
1. Validate config reference.
2. Build `GridRect` and `GridSettings` from config values.
3. Create `GridService`.
4. Create `PlacementService` using `Grid`.

## Cross-module links
- Reads from `Data` (`GameData.GridConfigSO`).
- Instantiates `Core.Grid` and `Core.Placement` services.
- Exposes services for all gameplay systems (`PlayerGridProbe`, `BuildModeController`, `NodeSpawner`).
