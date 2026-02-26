# 0002 — Core.Grid (AISummary)

> Дата: 2026-02-24  
> Модуль: `Core.Grid`  
> Призначення: чиста (без `UnityEngine`) математика дискретної 2D-сітки: координати, межі, перетворення world↔grid, сніп до центру клітинки.

## Що дає модуль
- **Єдиний формат координат клітинки**: `GridCoord (X,Y)` з базовою арифметикою та конвертацією в `int2`.
- **Опис прямокутних меж**: `GridRect(minX,minY,width,height)` + `Contains()` + `Clamp()`.
- **Налаштування сітки**: `GridSettings(cellSize, originWorld, bounds)` з валідацією.
- **Контракт та реалізація**:
  - `IGrid` — інтерфейс для grid-механік.
  - `GridService` — реалізація “pure math”: world→grid, grid→центр world, snap до центру.

## Основні сутності
### GridCoord
- Immutable `struct`, `IEquatable<GridCoord>`.
- Оператори: `+`, `-`, `==`, `!=`.
- Корисно для навігації, індексації, масивів/словників.

### GridRect
- Immutable `struct` з валідацією `width/height > 0`.
- `Contains(GridCoord)` — перевірка входження.
- `Clamp(int2)` — затискає координати в межі (включно) по Min та (виключно) по Max.

### GridSettings
- `CellSize > 0`.
- `OriginWorld` — “кут” сітки для `(0,0)` (локальна система сітки).
- `Bounds` — допустимий діапазон координат.

### GridService (IGrid)
- `WorldToGrid(float2 world)`:
  - переводить у локальні координати: `(world - origin) / cellSize`
  - бере `floor` → індекс клітинки
- `GridToWorldCenter(GridCoord)`:
  - `origin + (coord + 0.5) * cellSize`
- `SnapWorldToCellCenter(world, clampToBounds=true)`:
  - `WorldToGrid` → (опційно) `Clamp` → `GridToWorldCenter`
- `IsInside` / `Clamp` — через `Settings.Bounds`.

## Залежності
- `Unity.Mathematics` (`float2`, `int2`, `math.floor`, `math.clamp`).
- Без `UnityEngine` (придатно для unit-тестів).

## Типові сценарії використання
- Переведення кліку/позиції миші в `GridCoord` (через `float2` world).
- Сніп placement/preview обʼєкта в центр клітинки.
- Обмеження руху/виділення в межах `Bounds`.

## Гарантії та edge cases
- `GridRect` не дозволяє `width<=0` або `height<=0` (кидає exception).
- `GridSettings` не дозволяє `cellSize<=0` (кидає exception).
- `WorldToGrid` використовує `floor`: для відʼємних координат world це важливо (наприклад `-0.1` → `-1`).
- `Clamp` працює за правилом MaxExclusive (останній валідний індекс = `MaxExclusive-1`).

## TODO / ідеї покращень
- Додати `GridDirections` (вже є) розширеним набором (діагоналі, список напрямків).
- Додати методи для переліку клітинок у `GridRect` (ітератор), якщо знадобиться.
- Додати `WorldToGridClamped` як sugar-метод (зараз є через `SnapWorldToCellCenter` + `Clamp`).
