# devlog/0003_data_api.md

## Модуль: Data (ScriptableObjects-конфіги)

Ціль модуля **Data** — зберігати **налаштування/дані** у вигляді Unity `ScriptableObject` (і потенційно інші data-артефакти), не змішуючи це з геймплейною логікою.

Наразі модуль містить один ключовий конфіг: **GridConfigSO**.

---

## Залежності

### Data.asmdef
**Файл:** `Data.asmdef`

- `name`: `Data`
- `rootNamespace`: `GameData`
- `references`: містить посилання на **Core** (через GUID у списку references)
- `autoReferenced`: `false`  
  → збірка **не підхоплюється автоматично**, її треба явно підключати в тих `.asmdef`, де потрібні `GameData.*` типи.
- `noEngineReferences`: `false`  
  → ок, бо `ScriptableObject` використовує `UnityEngine`.

---

## Склад модулю (файли і ролі)

### 1) GameData.GridConfigSO
**Файл:** `GridConfigSO.cs`  
**Тип:** `sealed class GridConfigSO : ScriptableObject`  
**Namespace:** `GameData`

**CreateAssetMenu:**
- `Ritebound/Config/Grid Config`
- `fileName = "GridConfig"`

**Призначення:** централізовано зберігати параметри гріда:
- масштаб (розмір клітинки),
- точку відліку (origin у world),
- межі (bounds у grid-координатах),
- опції візуалізації Gizmos у редакторі.

---

## Поля і семантика

### World
- `cellSize : float`
  - Атрибути: `[Min(0.01f)]`
  - Значення: розмір клітинки в world units.
- `originWorld : Vector2`
  - Значення: world-позиція “нуля” гріда (від якої рахується конвертація).

### Bounds (grid coords)
- `minX, minY : int`
  - Мінімальні координати області гріда.
- `width, height : int`
  - Атрибути: `[Min(1)]`
  - Розмір області гріда (ширина/висота) в клітинках.

> Інтерпретація: bounds задаються в координатах гріда (не world), і описують прямокутну область.

### Gizmos
- `draw : bool`
  - Увімкнути/вимкнути малювання гріда (типово використовується в `OnDrawGizmos`).
- `drawCellCenters : bool`
  - Якщо `true` — додатково малювати центри клітинок.
- `centerMarkerSize : float`
  - Атрибути: `[Min(0.1f)]`
  - Розмір маркера центру клітинки.

---

## Як створити і підключити в проєкті

### Створення asset
1. У Project window: **Create → Ritebound → Config → Grid Config**
2. Задати параметри:
   - `cellSize` під масштаб спрайтів/тайлів,
   - `originWorld` під позицію “нуля” поля,
   - `minX/minY/width/height` під розмір рівня,
   - `draw*` — за потребою для dev-візуалізації.

> Примітка: у наданих файлах **немає** готового `GridConfigSO.asset`, тож дефолтні значення залежать від того, що ти виставиш у створеному asset.

### Використання
Очікуваний патерн:
- **Scene adapter / CompositionRoot** читає `GridConfigSO` і на його основі формує `GridSettings` для Core-грида.
- **Editor gizmo renderer** може малювати грід за `GridConfigSO` навіть без runtime-ініціалізації (як у Gameplay-модулі через `GridGizmosRenderer`).

---

## Scope guard (що тут НЕ робили)
- Немає runtime-логіки гри (руху, placement, спавну тощо).
- Немає “рівнів”, “гайдбуку”, або інших конфігів — поки тільки GridConfig.
- Модуль не ініціалізує Core-грид сам по собі; він лише *дає дані*.

---

## Нотатки / наступні кроки
- Коли з’являться інші системи (build, economy, levels) — додавати нові SO-конфіги в Data:
  - `LevelDefinitionSO`, `BuildRulesSO`, `EconomyConfigSO` тощо.
- За потреби: навести структуру папок у Data (наприклад `Config/` vs `Content/`), але зараз це не критично.
