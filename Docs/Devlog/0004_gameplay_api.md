# devlog/0003_gameplay_api.md

## Модуль: Gameplay (рух гравця + build mode + візуалізація гріда)

Цей модуль — “ігровий шар” поверх Core-модулів. Він:
- дає простий контролер руху гравця,
- визначає, в якій клітинці гріда зараз гравець,
- дозволяє ставити/видаляти елементи (ноди) в межах радіусу,
- спавнить/деспавнить візуальні префаби нод по подіях Placement,
- малює Gizmos-сітку за конфігом.

---

## Залежності

### Посилання (namespaces)
- `Core.Grid` — координати, конвертація World ↔ Grid (`WorldToGrid`, `GridToWorldCenter`).
- `Core.Placement` — правила та операції розміщення (`CanPlace`, `TryPlace`, `TryRemove`), події `NodePlaced/NodeRemoved`.
- `Scenes` — `CompositionRoot` (точка доступу до `root.Grid`, `root.Placement`).
- `GameData` — `GridConfigSO` для налаштувань Gizmos-грида.

### Gameplay.asmdef
`Gameplay` — окрема збірка, `autoReferenced=false`, тобто її треба підключати вручну там, де потрібно.

---

## Склад модулю (файли і ролі)

### 1) Gameplay.Input.MouseWorld
**Файл:** `MouseWorld.cs`  
**Тип:** `static` утиліта

**Призначення:** отримати позицію миші в 2D world-space через камеру.

**API:**
- `Vector2 GetWorld2D(Camera cam)`
  - бере `Input.mousePosition`
  - конвертує через `cam.ScreenToWorldPoint`
  - повертає `(x,y)`

---

### 2) Gameplay.Player.PlayerController2D
**Файл:** `PlayerController2D.cs`  
**Призначення:** базовий рух гравця по WASD/стрілках.

**Поля:**
- `moveSpeed` (float, min 0.1)

**Логіка:**
- `Update()` читає:
  - `Input.GetAxisRaw("Horizontal")`
  - `Input.GetAxisRaw("Vertical")`
- нормалізує вектор і додає до `transform.position` з урахуванням `Time.deltaTime`.

> Це “найпростіший” контролер. Фізики (Rigidbody2D) тут нема — чисто transform-based.

---

### 3) Gameplay.Player.PlayerGridProbe
**Файл:** `PlayerGridProbe.cs`  
**Призначення:** тримає актуальну клітинку гріда, в якій стоїть гравець.

**Поля/властивості:**
- `root : CompositionRoot`
- `CurrentCoord : GridCoord` (public get)

**Життєвий цикл:**
- `Awake()`:
  - якщо `root` не заданий — шукає `FindFirstObjectByType<CompositionRoot>()`.
- `Update()`:
  - якщо `root.Grid` відсутній — вихід
  - бере позицію гравця `transform.position`
  - рахує `CurrentCoord = root.Grid.WorldToGrid(float2(x,y))`

**Gizmos:**
- `OnDrawGizmosSelected()` малює wire-сферу в центрі поточної клітинки.

---

### 4) Gameplay.Placement.BuildModeController
**Файл:** `BuildModeController.cs`  
**Призначення:** керує “режимом будівництва”: вибір таргет-клітинки мишею, перевірка дистанції до гравця, постановка/видалення нод.

**Залежності (Refs):**
- `root : CompositionRoot`
- `playerProbe : PlayerGridProbe`
- `cam : Camera`

**Rules:**
- `buildRadius : int` (Manhattan distance, min 0)

**Element:**
- `selectedType : ElementType` (стартово Water)

**Публічний стан (для UI/дебагу):**
- `TargetCoord : GridCoord`
- `TargetInRange : bool`
- `CanPlaceHere : bool`

**Алгоритм Update():**
1. **TargetCoord**  
   - `mw = MouseWorld.GetWorld2D(cam)`
   - `TargetCoord = root.Grid.WorldToGrid(mw)`
2. **Range check (Manhattan)**  
   - `pc = playerProbe.CurrentCoord`
   - `dist = abs(dx) + abs(dy)`
   - `TargetInRange = dist <= buildRadius`
3. **Placement check**  
   - `CanPlaceHere = TargetInRange && root.Placement.CanPlace(TargetCoord)`
4. **Input**
   - **ЛКМ (0):** якщо `CanPlaceHere` → `TryPlace(selectedType, TargetCoord, out node)` → Debug.Log
   - **ПКМ (1):** `TryRemove(TargetCoord, out removed)` → Debug.Log
   - **1:** вибір `ElementType.Water`
   - **2:** вибір `ElementType.Fire`

**Gizmos:**
- `OnDrawGizmos()` малює напівпрозорий кубик у центрі `TargetCoord`
  - зелений якщо `CanPlaceHere`
  - червоний якщо ні

> Примітка: у файлі є `using UnityEngine.tvOS;` — виглядає зайвим (не використовується).

---

### 5) Gameplay.Placement.NodeSpawner
**Файл:** `NodeSpawner.cs`  
**Призначення:** слухає події `Placement` і створює/видаляє префаби нод у світі.

**Поля:**
- `root : CompositionRoot`
- Prefabs:
  - `waterNodePrefab`
  - `fireNodePrefab`
- `nodesParent : Transform` (куди складати інстанси)
- `_spawned : Dictionary<int, GameObject>` (NodeId.Value → інстанс)

**Життєвий цикл:**
- `Awake()`:
  - підхоплює `root` через `FindFirstObjectByType` якщо null
  - `nodesParent = transform` якщо null
- `OnEnable()`:
  - підписується на `root.Placement.NodePlaced` і `NodeRemoved`
- `OnDisable()`:
  - відписується

**OnNodePlaced(PlacedNode node):**
- вибір prefab по `node.Type` (Water/Fire)
- `center = root.Grid.GridToWorldCenter(node.Coord)`
- `Instantiate(prefab, center, parent)`
- якщо на префабі є `NodeView` → `Initialize(node.Id, node.Type)`
- запис у `_spawned`

**OnNodeRemoved(PlacedNode node):**
- якщо інстанс існує → `Destroy(go)`
- видалити з `_spawned`

---

### 6) Gameplay.Placement.NodeView
**Файл:** `NodeView.cs`  
**Призначення:** мінімальна “в’юшка” для збереження метаданих у інстансі префаба.

**API:**
- `Initialize(NodeId id, ElementType type)`
- властивості:
  - `NodeId : int`
  - `Type : ElementType`

---

### 7) Gameplay.Grid.GridGizmosRenderer
**Файл:** `GridGizmosRenderer.cs`  
**Атрибут:** `[ExecuteAlways]`

**Призначення:** малює gizmos-грид у сцені за `GridConfigSO`, без залежності від runtime-ініціалізації реального Grid (будує тимчасові `GridSettings`).

**Поля:**
- `gridConfig : GridConfigSO`

**OnDrawGizmos():**
- якщо `gridConfig == null` або `!gridConfig.draw` → вихід
- створює:
  - `GridRect(bounds)` з `minX/minY/width/height`
  - `GridSettings(cellSize, originWorld, bounds)`
- викликає `DrawGrid(settings)`

**DrawGrid():**
- малює рамку гріда (border)
- малює вертикальні/горизонтальні лінії
- опційно малює центри клітинок (`drawCellCenters` + `centerMarkerSize`)

---

## Як підключити у сцені (мінімально)

1. **CompositionRoot** у сцені повинен мати доступні `Grid` та `Placement`.
2. **Player**:
   - `PlayerController2D`
   - `PlayerGridProbe` (root можна не вказувати — знайде сам)
3. **BuildModeController**:
   - посилання на `root`, `playerProbe`, `cam` (можна лишити порожнім — підхопить в Awake, але краще явно)
4. **NodeSpawner**:
   - задати `waterNodePrefab`, `fireNodePrefab`
   - (опційно) `nodesParent`
5. **GridGizmosRenderer**:
   - задати `GridConfigSO`

---

## Керування (дефолт)
- Рух: WASD / стрілки (через Input axes)
- Build:
  - ЛКМ — поставити ноду
  - ПКМ — прибрати ноду
  - `1` — Water
  - `2` — Fire

---

## Нотатки/покращення (коротко)
- Винести Input (клік/клавіші) в окремий сервіс або Input System, якщо масштабуватимеш.
- Додати UI-підказки (тип, радіус, можна/не можна).
- Опційно: кешувати prefab map (ElementType → prefab) і валідувати на старті.
- Перевірити зайві using та “биті” коментарі (видно сліди проблеми з кодуванням у кількох місцях).