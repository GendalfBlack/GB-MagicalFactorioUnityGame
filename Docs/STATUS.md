# STATUS.md

## Поточний стан проєкту

Проєкт має чітко розділену модульну архітектуру з чистими Core-сервісами та окремим Scene Composition Root. Основні системи гріда та placement повністю реалізовані та інтегровані в простий gameplay-прототип.

Архітектура відповідає принципу:
> **Core (pure logic) → Scene wiring → Gameplay layer → Data configs**

---

# 1. Core.Grid ✅ (готово)

Джерело: devlog/0002_core_grid_api.md

## Статус
✔ Повністю реалізований  
✔ Pure math (без UnityEngine)  
✔ Engine-agnostic (Unity.Mathematics)  

## Можливості
- World ↔ Grid конвертація
- Snap до центру клітинки
- Clamp в межах bounds
- Перевірка IsInside
- Immutable GridCoord
- Dictionary-safe hash

## Архітектурна оцінка
- Чистий сервіс
- Не має runtime-залежностей
- Готовий до масштабування (сусіди, ітератори, helper-и)

---

# 2. Core.Placement ✅ (готово)

Джерело: devlog/0001_core_placement_api.md

## Статус
✔ Повністю реалізований  
✔ Dictionary-based storage  
✔ Подієва модель  

## Можливості
- CanPlace
- TryPlace
- TryRemove
- TryGet
- Події NodePlaced / NodeRemoved
- Типобезпечний NodeId
- Immutable PlacedNode

## Поточна модель

1 клітинка = 1 нода

## Архітектурна оцінка
- Чистий state-сервіс
- Не залежить від Unity
- Готовий для збереження/серіалізації
- Легко розширюється (Enumerate, Clear, bulk load)

---

# 3. Data (ScriptableObjects) ✅ (мінімальна база готова)

Джерело: devlog/0003_data_api.md

## Статус
✔ GridConfigSO реалізований  
✔ Розділення логіки і даних  

## Призначення
- Зберігання параметрів гріда
- Editor-візуалізація
- Конфіг для CompositionRoot

## Обмеження
- Поки лише один SO
- Немає LevelDefinition / Economy / BuildRules

---

# 4. Scenes (Composition Root) ✅

Джерело: devlog/0004_scenes_api.md

## Статус
✔ Реалізований  
✔ Коректна ініціалізація сервісів  

## Відповідальність
- Створення GridService
- Створення PlacementService
- Ін’єкція через властивості:
  - IGrid
  - IPlacementService

## Архітектурна якість
- Чітка точка входу
- Немає ServiceLocator
- Мінімальний DI-підхід

---

# 5. Gameplay Layer ✅ (прототип працює)

Джерело: devlog/0003_gameplay_api.md

## Реалізовано

### Рух гравця
- Transform-based movement

### PlayerGridProbe
- Визначення поточної клітинки

### BuildModeController
- Manhattan-distance обмеження
- Вибір типу (Water/Fire)
- TryPlace / TryRemove
- Gizmos preview

### NodeSpawner
- Слухає Placement події
- Створює / видаляє prefab
- Веде локальний dictionary інстансів

### GridGizmosRenderer
- Editor-only грід
- Не залежить від runtime GridService

---

# Поточна функціональність гри

Гравець може:
- Рухатись по сцені
- Бачити свою клітинку
- Обирати тип елемента (1/2)
- Ставити елемент у радіусі
- Видаляти елемент
- Бачити візуальний prefab
- Бачити grid Gizmos

---

# Архітектурна зрілість

| Шар | Стан | Коментар |
|------|------|----------|
| Core.Grid | ✅ Стабільний | Чистий, масштабований |
| Core.Placement | ✅ Стабільний | Подієва модель |
| Data | ⚠ Мінімальний | Лише GridConfig |
| Scenes | ✅ Коректний | Чіткий CompositionRoot |
| Gameplay | ⚠ Прототип | Input напряму через Unity |

---

# Технічні борги / наступні кроки

## 1. Placement
- Додати Enumerate()
- Додати Clear()
- Додати Bulk load режим

## 2. Gameplay
- Винести Input у сервіс
- Додати UI-індикатори
- Кешувати prefab map
- Прибрати зайві using

## 3. Data
- LevelDefinitionSO
- BuildRulesSO
- EconomyConfigSO

## 4. Grid
- Neighbors helpers
- All4 / All8 directions
- Iterator по GridRect

---

# Загальна оцінка

Проєкт знаходиться на етапі:

> **Стабільна базова архітектура + робочий build-прототип**

Core-шари вже достатньо чисті, щоб:
- розширювати механіки
- додавати економіку
- вводити automation
- починати системну геймплейну логіку

Фундамент закладено правильно.  
Подальший розвиток — це вже розширення систем, а не рефакторинг основ.