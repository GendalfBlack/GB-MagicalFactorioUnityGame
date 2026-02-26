# Core.Placement — короткий підсумок (aisummary)

## Призначення
Модуль **Core.Placement** відповідає за **розміщення/видалення елементів (нод)** на ігровій сітці та за зберігання стану “що стоїть у якій клітинці”.

Модуль **не малює**, **не створює GameObject**, і **не знає нічого про UI** — лише чиста логіка та події.

## Що входить
- `ElementType` — перелік типів елементів, які можна ставити на сітку.
- `NodeId` — легкий ідентифікатор ноди (інкрементальний int).
- `PlacedNode` — “снімок” розміщеної ноди: `Id`, `Type`, `Coord`.
- `IPlacementService` — контракт сервісу розміщення (події + методи).
- `PlacementService` — реалізація сервісу (in-memory словник за координатою).

## Залежності
- **Core.Grid**
  - `GridCoord` — координата клітинки
  - `IGrid` — перевірка меж сітки (`IsInside`)

## Основні правила
- Ставити можна лише якщо:
  1) координата всередині сітки (`_grid.IsInside`)
  2) клітинка порожня (немає запису в `_byCoord`)
- Видалення можливе лише якщо в клітинці щось є.

## Події (реактивність)
- `NodePlaced(PlacedNode)` — викликається після успішного `TryPlace`.
- `NodeRemoved(PlacedNode)` — викликається після успішного `TryRemove`.

> Ці події — “точки інтеграції” для UI/візуалізації/ефектів: інші модулі можуть підписатися і відмалювати/прибрати відповідні об’єкти.

## API (коротко)
- `bool CanPlace(GridCoord coord)`
- `bool TryPlace(ElementType type, GridCoord coord, out PlacedNode node)`
- `bool TryRemove(GridCoord coord, out PlacedNode removed)`
- `bool TryGet(GridCoord coord, out PlacedNode node)`

## Нотатки / edge cases
- `NodeId` генерується інкрементально (`_nextId++`) і **не перевикористовується** після видалення.
- Дані зберігаються в пам’яті в `Dictionary<GridCoord, PlacedNode>` (не серіалізуються самі по собі).
