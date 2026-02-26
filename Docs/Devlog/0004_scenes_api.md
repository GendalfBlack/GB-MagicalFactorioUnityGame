# devlog/0004_scenes_api.md

## Модуль: Scenes (Composition Root / Dependency Wiring)

Цей модуль — “точка складання” сцени. Він ініціалізує Core-сервіси та дає іншим компонентам доступ до них через один компонент `CompositionRoot`.

---

## Залежності

### Namespace / Using
- `Core.Grid` — `IGrid`, `GridService`, `GridSettings`, `GridRect`
- `Core.Placement` — `IPlacementService`, `PlacementService`
- `GameData` — `GridConfigSO`
- `Unity.Mathematics` — `float2`
- `UnityEngine` — `MonoBehaviour`, `SerializeField`, `Debug`

### Scenes.asmdef
- Assembly: `Scenes`
- `rootNamespace: "Scenes"`
- `autoReferenced: false` (підключати вручну там, де треба)
- `references`: через GUID (Core/Grid, Core/Placement, GameData та ін.)

---

## Склад модулю

### 1) Scenes.CompositionRoot
**Файл:** `CompositionRoot.cs`  
**Тип:** `MonoBehaviour` (sealed)

**Призначення:**
- Підхоплює `GridConfigSO` зі сцени
- Створює `GridService` з `GridSettings`
- Створює `PlacementService`, який працює поверх `IGrid`
- Експортує сервіси у вигляді властивостей:
  - `public IGrid Grid { get; private set; }`
  - `public IPlacementService Placement { get; private set; }`

**SerializeFields:**
- `gridConfig : GridConfigSO`

**Життєвий цикл:**
- `Awake()`:
  1. Перевіряє `gridConfig != null`
     - якщо null → `Debug.LogError`, `enabled = false`, `return`
  2. Збирає bounds:
     - `new GridRect(gridConfig.minX, gridConfig.minY, gridConfig.width, gridConfig.height)`
  3. Збирає settings:
     - `new GridSettings(gridConfig.cellSize, float2(originWorld.x, originWorld.y), bounds)`
  4. Ініціалізує:
     - `Grid = new GridService(settings)`
     - `Placement = new PlacementService(Grid)`
  5. Логує факт ініціалізації (bounds + cell size)

**Ідея розширення (коментар у коді):**
- тут надалі будуть створюватись інші Core-сервіси: Placement, Linking, Ritual тощо.

---

## Як використовується в інших модулях
- Gameplay-компоненти (`BuildModeController`, `PlayerGridProbe`, `NodeSpawner`) шукають `CompositionRoot` у сцені (`FindFirstObjectByType`) або отримують посилання через Inspector.
- Вся взаємодія з грідом та постановкою об’єктів іде через:
  - `root.Grid`
  - `root.Placement`

---

## Вимоги до сцени
1. У сцені має бути GameObject з `CompositionRoot`.
2. У `CompositionRoot.gridConfig` має бути призначений валідний `GridConfigSO`.

---

## Нотатки
- У коді видно артефакти кодування в одному коментарі (ймовірно файл збережений не в UTF-8).
- `autoReferenced=false` у asmdef — це добре для контролю залежностей, але треба слідкувати, щоб потрібні assembly references були явно додані.