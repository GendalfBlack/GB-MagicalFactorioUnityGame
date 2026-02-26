# 0002 — Core.Grid (Devlog API)

> Дата: 2026-02-24  
> Namespace: `Core.Grid`  
> Принцип: **pure math**, без `UnityEngine`, тільки `Unity.Mathematics`.

## Склад модуля (файли)
- `IGrid.cs` — контракт grid-сервісу.
- `GridService.cs` — реалізація перетворень і clamp/inside.
- `GridSettings.cs` — конфіг сітки (cell size, origin, bounds) з валідацією.
- `GridRect.cs` — прямокутні межі в grid-координатах + clamp.
- `GridCoord.cs` — value-type координати клітинки + оператори.
- `GridDirections.cs` — базові напрямки (Up/Down/Left/Right).

---

## IGrid.cs
**Роль:** абстракція для систем, що працюють із сіткою (рух, плейсмент, selection).

### Public API
- `GridSettings Settings { get; }`
- `GridCoord WorldToGrid(float2 world)`
- `float2 GridToWorldCenter(GridCoord coord)`
- `float2 SnapWorldToCellCenter(float2 world, bool clampToBounds = true)`
- `bool IsInside(GridCoord coord)`
- `GridCoord Clamp(GridCoord coord)`

### Нотатки
- Інтерфейс оперує `float2/int2` з `Unity.Mathematics`, щоб залишатися “engine-agnostic” (без `Vector2`).

---

## GridSettings.cs
**Роль:** immutable-конфіг, який передається в `GridService` і в будь-які системи, що потребують однакових правил перетворення.

### Поля
- `float CellSize` — **world units per cell**
- `float2 OriginWorld` — позиція “кута” клітинки `(0,0)`
- `GridRect Bounds` — допустимі координати

### Валідація
- `CellSize` має бути `> 0`, інакше `ArgumentOutOfRangeException`.

### Конструктор
- `GridSettings(float cellSize, float2 originWorld, GridRect bounds)`

---

## GridCoord.cs
**Роль:** дискретна координата клітинки, придатна для словників/навігації/логіки.

### Тип
- `readonly struct GridCoord : IEquatable<GridCoord>`
- `[Serializable]`

### Поля
- `int X`
- `int Y`

### Методи
- `int2 ToInt2()`
- `static GridCoord FromInt2(int2 v)`
- `bool Equals(GridCoord other)`
- `override bool Equals(object obj)`
- `override int GetHashCode()`
- `override string ToString()` → `"(X,Y)"`

### Оператори
- `==`, `!=`
- `+`, `-`

### Нотатки
- Immutable + `GetHashCode()` через `HashCode.Combine` → ок для `Dictionary<GridCoord, ...>`.

---

## GridDirections.cs
**Роль:** готові офсети для руху/сусідів.

### Константи
- `Up = (0,1)`
- `Down = (0,-1)`
- `Left = (-1,0)`
- `Right = (1,0)`

---

## GridRect.cs
**Роль:** опис прямокутних меж сітки (min + size) і clamp.

### Поля
- `int MinX`
- `int MinY`
- `int Width`
- `int Height`

### Derived properties
- `int MaxXExclusive => MinX + Width`
- `int MaxYExclusive => MinY + Height`

### Валідація
- `Width > 0`, `Height > 0`, інакше `ArgumentOutOfRangeException`.

### Методи
- `bool Contains(GridCoord c)`
  - `c.X in [MinX, MaxXExclusive)`
  - `c.Y in [MinY, MaxYExclusive)`
- `int2 Clamp(int2 v)`
  - `x = clamp(v.x, MinX, MaxXExclusive-1)`
  - `y = clamp(v.y, MinY, MaxYExclusive-1)`
- `override string ToString()`

### Edge cases
- Важливо: Max — **exclusive**. Останній валідний індекс = `MaxExclusive - 1`.

---

## GridService.cs
**Роль:** реалізація `IGrid` як pure-math сервісу.

### Поля/властивості
- `GridSettings Settings { get; }`

### Конструктор
- `GridService(GridSettings settings)` — зберігає `Settings`.

### API
- `bool IsInside(GridCoord coord)` → `Settings.Bounds.Contains(coord)`

- `GridCoord Clamp(GridCoord coord)`
  1. `coord.ToInt2()`
  2. `Settings.Bounds.Clamp(int2)`
  3. `GridCoord.FromInt2(...)`

- `GridCoord WorldToGrid(float2 world)`
  1. `local = (world - OriginWorld) / CellSize`
  2. `x = floor(local.x)`, `y = floor(local.y)`
  3. `return new GridCoord(x,y)`

- `float2 GridToWorldCenter(GridCoord coord)`
  - `OriginWorld + (float2(coord.X+0.5, coord.Y+0.5) * CellSize)`

- `float2 SnapWorldToCellCenter(float2 world, bool clampToBounds = true)`
  1. `c = WorldToGrid(world)`
  2. if `clampToBounds` → `c = Clamp(c)`
  3. return `GridToWorldCenter(c)`

### Поведінкові нюанси
- `floor` важливий для негативних значень (стандартне “попадання” в клітинку).
- `SnapWorldToCellCenter` — основний “user friendly” метод для UI preview/placement.

---

## Мінімальне підключення в коді (приклад)
```csharp
using Core.Grid;
using Unity.Mathematics;

var bounds = new GridRect(minX: 0, minY: 0, width: 5, height: 5);
var settings = new GridSettings(cellSize: 1f, originWorld: new float2(0,0), bounds: bounds);
IGrid grid = new GridService(settings);

float2 world = new float2(2.2f, 3.7f);
GridCoord c = grid.WorldToGrid(world);           // (2,3)
float2 snap = grid.SnapWorldToCellCenter(world); // (2.5,3.5)
```

## TODO / розширення
- Додати діагональні напрямки та масив `All4/All8`.
- Додати helper-и: `GetNeighbors4(GridCoord)` / `TryStep(GridCoord, dir)`.
- Додати ітератор по `GridRect` (Cells()) якщо знадобляться батч-операції.
