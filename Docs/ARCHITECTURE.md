# ARCHITECTURE.md

# 1. Architectural Overview

Проєкт побудований за принципом **Layered + Composition Root** з чітким розділенням відповідальностей між модулями:

```
Data (ScriptableObjects)
↓
Scenes (Composition Root / Wiring)
↓
Core (Grid + Placement)
↑
Gameplay (MonoBehaviour Adapters)
```

- **Core** — чиста логіка без `UnityEngine`.
- **Scenes** — ініціалізація сервісів і wiring.
- **Gameplay** — MonoBehaviour-адаптери поверх Core.
- **Data** — конфігурація через `ScriptableObject`.

**Ключова ідея:** Data дає конфіг, Scenes збирає Core-сервіси, Gameplay взаємодіє з Core через `CompositionRoot`.

---

# 2. Assembly Layering

## Assemblies
- `Core.Grid`
- `Core.Placement`
- `Data`
- `Scenes`
- `Gameplay`

Усі мають `autoReferenced=false`, що забезпечує контрольовані залежності через `.asmdef`.

## Dependency Graph

```
Core.Grid
↑
Core.Placement
↑
Scenes
↑
Gameplay

Data → Scenes
Data → Gameplay (gizmos only)
```

### Dependency Rules
- Core не залежить ні від кого.
- Placement залежить лише від Grid.
- Scenes залежить від Core + Data.
- Gameplay залежить від Scenes + Core + Data.
- Data не містить runtime-логіки.

---

# 3. Core Layer

## 3.1 Core.Grid

### Responsibility
Pure-math модель дискретної 2D-сітки.

### Key Types
- `GridCoord` — immutable координата.
- `GridRect` — bounds (Min + Size, MaxExclusive).
- `GridSettings` — конфіг (CellSize, OriginWorld, Bounds).
- `IGrid` — контракт.
- `GridService` — реалізація.

### Guarantees
- `CellSize > 0`
- `Width/Height > 0`
- MaxExclusive semantics
- floor-based world→grid conversion
- Engine-agnostic (`Unity.Mathematics` only)

### Design Properties
- Stateless (окрім Settings)
- Deterministic
- Unit-test friendly
- No `UnityEngine` dependency

---

## 3.2 Core.Placement

### Responsibility
In-memory state management розміщених нод.

### Data Model
```
Dictionary<GridCoord, PlacedNode>
```

### Key Types
- `ElementType`
- `NodeId` (incremental, non-reusable)
- `PlacedNode` (immutable snapshot)
- `IPlacementService`
- `PlacementService`

### Placement Rules
Placement possible if:
1. `_grid.IsInside(coord)`
2. `_byCoord` does not contain key

### Reactive Integration
Events:
- `NodePlaced(PlacedNode)`
- `NodeRemoved(PlacedNode)`

Core не знає про `GameObject`, UI або Prefab.

### Architectural Properties
- Single responsibility (state + rules)
- Event-driven integration
- O(1) average lookup (Dictionary)
- No serialization baked in
- No stackable support (1 cell = 1 node)

---

# 4. Data Layer

## Purpose
Configuration-only assembly (ScriptableObjects).

## Current Asset
### GridConfigSO
Stores:
- cellSize
- originWorld
- minX/minY
- width/height
- gizmo flags

## Role in Architecture
- Scene-level configuration input
- Editor visualization source
- No runtime logic

Scenes module consumes Data → builds Core settings.

---

# 5. Scenes Layer (Composition Root)

## Main Component
### Scenes.CompositionRoot

### Responsibilities
- Validate `GridConfigSO`
- Construct `GridRect`
- Construct `GridSettings`
- Instantiate:
  - `GridService`
  - `PlacementService`
- Expose:
  - `IGrid`
  - `IPlacementService`

### Lifecycle
`Awake()` performs all wiring.  
If config missing → disables itself.

### Architectural Pattern
- Manual DI
- Single entry point for scene-level services
- No ServiceLocator pattern
- Centralized initialization

---

# 6. Gameplay Layer

## Nature
MonoBehaviour adapters over Core services.

Gameplay does not own logic — it delegates to Core.

---

## 6.1 Player Subsystem

### PlayerController2D
- Transform-based movement
- Input axes driven

### PlayerGridProbe
- Reads world position
- Uses `root.Grid.WorldToGrid`
- Stores current `GridCoord`

---

## 6.2 Build Mode

### BuildModeController
Pipeline per frame:
1. Mouse → World
2. World → GridCoord
3. Manhattan distance check
4. `Placement.CanPlace`
5. `TryPlace / TryRemove`

Rules:
- Range-based build restriction
- ElementType switching via numeric keys

No direct `GameObject` manipulation.

---

## 6.3 Node Visualization

### NodeSpawner
Reactive adapter:
- Subscribes to Placement events
- Converts GridCoord → world center
- Instantiates prefab
- Tracks by `NodeId.Value`

### NodeView
Stores:
- NodeId
- ElementType

No logic inside view.

---

## 6.4 GridGizmosRenderer
- `ExecuteAlways`
- Uses `GridConfigSO`
- Builds temporary `GridSettings`
- Draws lines + centers

Does not depend on runtime `GridService`.

---

# 7. Data Flow

## Placement Flow
```
Input → BuildModeController
→ PlacementService
→ Dictionary mutation
→ Event
→ NodeSpawner
→ Instantiate / Destroy
```

## Grid Conversion Flow
```
World Position
↓
WorldToGrid
↓
GridCoord
↓
GridToWorldCenter
```

---

# 8. Architectural Characteristics

## Strengths
- Clear layer separation
- Pure Core (testable)
- Event-driven integration
- Explicit dependency graph
- Config decoupled from logic
- No cyclic dependencies

## Constraints
- No persistence layer yet
- No multi-node per cell support
- No stackable placements
- Input tightly coupled to Unity Input system
- No UI abstraction

---

# 9. Extension Points

## Core.Grid
- Neighbor enumeration
- Iterators over `GridRect`
- Path utilities

## Core.Placement
- `Enumerate()`
- `Clear()`
- Bulk load mode
- Stackable rules

## Scenes
- Additional services (Linking, Ritual, Economy)
- Centralized save/load integration

## Gameplay
- Input abstraction
- UI layer
- Feedback systems
- Automation mechanics

---

# 10. Architectural Summary

Current architecture level:
> Stable foundational engine layer with scene wiring and functional gameplay prototype.

Core is sufficiently isolated and scalable.

Future growth is expected to occur by:
- Adding new Core services
- Expanding Gameplay adapters
- Introducing state persistence
- Introducing automation systems

No fundamental refactor is required at this stage.
