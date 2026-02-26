# aisummary/0004_scenes.md

## Scenes module — summary (Composition Root)

### Purpose
`Scenes` contains a single entry point component that wires Core services for the scene:
- Builds the grid (`GridService`) from `GridConfigSO`.
- Builds placement system (`PlacementService`) on top of `IGrid`.
- Exposes both via properties for other modules (Gameplay) to use.

### Main class
**Scenes.CompositionRoot**
- Serialized dependency: `GridConfigSO gridConfig`
- Outputs:
  - `IGrid Grid { get; private set; }`
  - `IPlacementService Placement { get; private set; }`
- Awake flow:
  - Validate `gridConfig`
  - Create `GridRect` bounds from config
  - Create `GridSettings` (cellSize + originWorld + bounds)
  - `Grid = new GridService(settings)`
  - `Placement = new PlacementService(Grid)`
  - Debug log init result

### Assembly notes
`Scenes.asmdef`:
- name/rootNamespace: `Scenes`
- `autoReferenced: false`
- references are GUID-based (Core/Grid, Core/Placement, GameData, etc.)

### Scene requirements
- Scene must contain a `CompositionRoot` GameObject.
- `gridConfig` must be assigned; otherwise component disables itself.

### Notes
- One comment shows encoding artifacts; consider re-saving as UTF-8.