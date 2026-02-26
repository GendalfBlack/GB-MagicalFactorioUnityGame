# aisummary/0003_gameplay.md

## Gameplay module — summary (Grid + Build Mode + Node visuals)

### What it contains
Gameplay assembly provides scene-level glue on top of Core modules:
- Simple player movement (`PlayerController2D`) using transform translation.
- Player → Grid coordinate probe (`PlayerGridProbe`) via `root.Grid.WorldToGrid`.
- Build mode controller (`BuildModeController`) that:
  - reads mouse world position,
  - converts it to `TargetCoord`,
  - checks build range with Manhattan distance to player coord,
  - uses `root.Placement.CanPlace / TryPlace / TryRemove`,
  - supports type switching (1=Water, 2=Fire),
  - draws placement gizmo.
- Node visual spawner (`NodeSpawner`) that listens to Placement events:
  - `root.Placement.NodePlaced` → instantiate prefab at `GridToWorldCenter`,
  - `NodeRemoved` → destroy instance,
  - tracks instances by `node.Id.Value`.
- Minimal `NodeView` for storing `NodeId` and `ElementType` on prefab instances.
- Grid gizmo renderer (`GridGizmosRenderer`, ExecuteAlways) drawing grid lines from `GridConfigSO` without relying on runtime grid init.
- Utility `MouseWorld.GetWorld2D(Camera)`.

### Key dependencies
- `Scenes.CompositionRoot` is the access point for `root.Grid` and `root.Placement`.
- `Core.Grid`: `GridCoord`, conversions, `GridSettings/GridRect`.
- `Core.Placement`: placement rules + events (`NodePlaced/NodeRemoved`), `PlacedNode`, `ElementType`.
- `GameData.GridConfigSO`: gizmo grid config.

### Scene wiring (minimal)
- CompositionRoot present and initialized (Grid + Placement).
- Player GameObject: `PlayerController2D` + `PlayerGridProbe`.
- BuildModeController: references root/probe/camera (can auto-find but explicit is safer).
- NodeSpawner: assign prefabs for Water/Fire; optional parent transform.
- GridGizmosRenderer: assign GridConfigSO.

### Controls
- Move: WASD/Arrows (Input axes).
- Build: LMB place, RMB remove, 1=Water, 2=Fire.

### Notes
- `BuildModeController` has unused `using UnityEngine.tvOS;` (likely removable).
- Some comments show encoding artifacts; consider re-saving files as UTF-8.