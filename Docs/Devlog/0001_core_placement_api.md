# Devlog — Core.Placement

## Мета
`Core.Placement` зберігає стан розміщених елементів на сітці та надає подієвий API для gameplay-візуалізації.

## Склад модуля
- `ElementType` — перелік типів: `Water`, `Fire`.
- `NodeId` — типобезпечний ідентифікатор ноди (`int Value`, `IEquatable`).
- `PlacedNode` — immutable-DTO: `Id`, `Type`, `Coord`.
- `IPlacementService` — контракт розміщення.
- `PlacementService` — реалізація на `Dictionary<GridCoord, PlacedNode>`.

## Публічний API
- Події:
  - `NodePlaced(PlacedNode)`
  - `NodeRemoved(PlacedNode)`
- Методи:
  - `CanPlace(GridCoord coord)` — координата має бути в межах `IGrid` та вільною.
  - `TryPlace(ElementType type, GridCoord coord, out PlacedNode node)` — додає ноду та генерує `NodePlaced`.
  - `TryRemove(GridCoord coord, out PlacedNode removed)` — видаляє ноду та генерує `NodeRemoved`.
  - `TryGet(GridCoord coord, out PlacedNode node)` — читання стану за координатою.

## Логіка та гарантії
- Валідація меж делегована в `IGrid.IsInside`.
- `NodeId` генерується інкрементально від `1` (`_nextId`).
- Події викликаються лише після успішної зміни стану.
- Методи `Try*` повертають `false` замість exceptions у штатних сценаріях.

## Крос-інтеграція
- Потребує `Core.Grid` (`GridCoord`, `IGrid`).
- Створюється в `Scenes.CompositionRoot` як `new PlacementService(Grid)`.
- Використовується в `Gameplay.Placement.BuildModeController` для `CanPlace/TryPlace/TryRemove`.
- Події підписуються в `Gameplay.Placement.NodeSpawner` для spawn/destroy prefab-обʼєктів.

## Обмеження
- Модуль не створює Unity-обʼєкти, не працює з input/UI та не виконує персистентність.
