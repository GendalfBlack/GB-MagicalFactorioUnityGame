# Core.Placement — devlog API (0001_core_placement_api)

## Мета модуля
**Core.Placement** — центральний “стан” для логіки **розміщення** елементів на клітинках сітки:
- перевіряє можливість розміщення
- додає/видаляє ноди
- дозволяє швидко запитати, що стоїть у клітинці
- розсилає події для зовнішніх систем (UI/візуалізація/FX)

---

## Залежності та межі відповідальності

### Залежить від
- `Core.Grid.GridCoord` — адреса клітинки
- `Core.Grid.IGrid` — перевірка, чи координата в межах сітки (`IsInside`)

### Не робить
- не створює/не знищує Unity-об’єкти
- не працює з input/UI
- не займається збереженням на диск

---

## Файли модуля

### ElementType.cs
**Namespace:** `Core.Placement`

```csharp
public enum ElementType { Water = 0, Fire = 1 }
```

**Роль:** мінімальний набір типів елементів, які можна ставити.  
**Розширення:** додавання нових типів не ламає сервіс, але може вимагати оновлення візуалізатора/фабрики, якщо вона є в інших модулях.

---

### NodeId.cs
**Namespace:** `Core.Placement`

`readonly struct NodeId : IEquatable<NodeId>`
- Поле: `public readonly int Value;`
- Порівняння: `Equals`, `==`, `!=`
- `ToString()` повертає `Value.ToString()`

**Роль:** типобезпечний ID, щоб не плутати “просто int” з ідентифікатором ноди.  
**Важливо:** `GetHashCode()` повертає `Value` — достатньо для цього сценарію.

---

### PlacedNode.cs
**Namespace:** `Core.Placement`

`readonly struct PlacedNode`
- `NodeId Id`
- `ElementType Type`
- `GridCoord Coord`

**Роль:** незмінний DTO/знімок розміщення, який:
- зручно передавати в подіях
- зручно кешувати/логувати
- не прив’язаний до Unity

---

### IPlacementService.cs
**Namespace:** `Core.Placement`

#### Події
- `event Action<PlacedNode> NodePlaced;`
- `event Action<PlacedNode> NodeRemoved;`

#### Методи
- `bool CanPlace(GridCoord coord);`
- `bool TryPlace(ElementType type, GridCoord coord, out PlacedNode node);`
- `bool TryRemove(GridCoord coord, out PlacedNode removed);`
- `bool TryGet(GridCoord coord, out PlacedNode node);`

**Інтенція контракту:**
- “Try*”-методи не кидають exceptions на валідних сценаріях — повертають `false`.
- Події — головний механізм реакції зовнішніх систем.

---

### PlacementService.cs
**Namespace:** `Core.Placement`  
**Тип:** `sealed class PlacementService : IPlacementService`

#### Поля
- `private readonly IGrid _grid;`  
  Зовнішня сітка для валідації меж.
- `private readonly Dictionary<GridCoord, PlacedNode> _byCoord = new();`  
  Основне сховище стану: *координата → нода*.
- `private int _nextId = 1;`  
  Інкрементальний генератор `NodeId`.

#### Події
- `NodePlaced`
- `NodeRemoved`

#### Конструктор
`public PlacementService(IGrid grid)`
- приймає залежність `IGrid`
- не робить важких ініціалізацій

---

## Життєвий цикл та поведінка API

### CanPlace(GridCoord coord)
```csharp
if (!_grid.IsInside(coord)) return false;
return !_byCoord.ContainsKey(coord);
```

**Правило:** місце доступне, якщо координата в межах і порожня.  
**Складність:** `O(1)` середня (dictionary).

---

### TryPlace(ElementType type, GridCoord coord, out PlacedNode node)
Алгоритм:
1. `node = default`
2. `if (!CanPlace(coord)) return false`
3. створити `id = new NodeId(_nextId++)`
4. `node = new PlacedNode(id, type, coord)`
5. `_byCoord.Add(coord, node)`
6. `NodePlaced?.Invoke(node)`
7. `return true`

**Гарантії:**
- подія викликається тільки якщо запис реально додано
- ID монотонно зростає і не перевикористовується

---

### TryRemove(GridCoord coord, out PlacedNode removed)
Алгоритм:
1. якщо `_byCoord.TryGetValue(coord, out removed)`:
   - `_byCoord.Remove(coord)`
   - `NodeRemoved?.Invoke(removed)`
   - `return true`
2. інакше `return false`

**Гарантії:**
- подія викликається тільки якщо було що видаляти

---

### TryGet(GridCoord coord, out PlacedNode node)
```csharp
return _byCoord.TryGetValue(coord, out node);
```
**Складність:** `O(1)` середня.

---

## Scene wiring / DI-підключення (рекомендація)
Сервіс очікує `IGrid`, тому типова композиція виглядає так:
- створити/отримати екземпляр `IGrid` (з модуля Core.Grid)
- створити `PlacementService(grid)`
- роздати `IPlacementService` системам, що:
  - реагують на `NodePlaced/NodeRemoved` (візуалізація)
  - ініціюють `TryPlace/TryRemove` (input/gameplay)

> Конкретний DI/ServiceLocator тут не визначений — це свідомо, щоб модуль лишався чистим.

---

## Перевірки та потенційні покращення
- Немає методу “отримати всі розміщення” (може знадобитись для збереження/відновлення або побудови візуалізації при старті).
- Немає “bulk load” (відновлення стану без тригеру подій або з контрольованим режимом подій).
- Потенційно можна додати:
  - `IReadOnlyCollection<PlacedNode> All` або `IEnumerable<PlacedNode> Enumerate()`
  - `Clear()` (з подіями або без)
  - підтримку stackable-об’єктів (якщо потрібно кілька сутностей у клітинці) — зараз **одна клітинка = одна нода**.
