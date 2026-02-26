# Devlog — Core.Grid

## Мета
`Core.Grid` надає чисту (без `UnityEngine`) математику сітки: координати, межі, конвертації world↔grid та helper-напрямки.

## Склад модуля
- `GridCoord` — immutable координата клітинки (`X`, `Y`), оператори `+`/`-`, `ToInt2/FromInt2`.
- `GridDirections` — базові напрямки: `Up/Down/Left/Right`.
- `GridRect` — межі сітки (`MinX`, `MinY`, `Width`, `Height`, `Contains`, `Clamp`).
- `GridSettings` — конфігурація: `CellSize`, `OriginWorld`, `Bounds`.
- `IGrid` — контракт сервісу сітки.
- `GridService` — реалізація `IGrid`.

## Публічний API
- `WorldToGrid(float2 world)` — переводить world у `GridCoord` через `math.floor`.
- `GridToWorldCenter(GridCoord coord)` — центр клітинки у world.
- `SnapWorldToCellCenter(float2 world, bool clampToBounds = true)` — привʼязка позиції до центру клітинки.
- `IsInside(GridCoord coord)` — перевірка меж через `GridRect.Contains`.
- `Clamp(GridCoord coord)` — обмеження координати межами `GridRect`.

## Логіка та гарантії
- `GridRect` не приймає `width <= 0` або `height <= 0`.
- `GridSettings` не приймає `cellSize <= 0`.
- `WorldToGrid` використовує `floor`, тож відʼємні world-координати мапляться в нижчу клітинку (наприклад `-0.1 -> -1`).
- `GridToWorldCenter` рахується як `origin + (coord + 0.5) * cellSize`.

## Крос-інтеграція
- `Scenes.CompositionRoot` створює `GridService` із даних `GridConfigSO`.
- `Core.Placement.PlacementService` залежить від `IGrid` для перевірки меж.
- `Gameplay.Player.PlayerGridProbe` читає `WorldToGrid` для координати гравця.
- `Gameplay.Placement.BuildModeController` конвертує позицію курсора через `WorldToGrid`.
- `Gameplay.Grid.GridGizmosRenderer` відтворює ту ж геометрію сітки на основі `GridSettings`.

## Обмеження
- Модуль не знає про scene lifecycle, MonoBehaviour чи prefab-інстанціювання.
