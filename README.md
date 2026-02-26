# GB-MagicalFactorioUnityGame

Unity проєкт з модульною архітектурою (Core, Data, Scenes, Gameplay) і чистим розділенням відповідальностей.

## Документація
- Поточний стан: `Docs/STATUS.md`
- Архітектура: `Docs/ARCHITECTURE.md`

Обидва файли є основним джерелом правди щодо стану проєкту та структури модулів. Перед змінами перевіряйте їх узгодженість.

## Структура Assets
- `Assets/Scripts/`
  - `Core/` — чиста логіка (grid, placement), без UnityEngine.
  - `Data/` — ScriptableObject-конфіги.
  - `Scenes/` — composition root, ініціалізація сервісів.
  - `Gameplay/` — MonoBehaviour-адаптери над Core.
  - `UI/` — UI-скрипти (за потреби).
- `Assets/Tests/`
  - Місце для unit та integration тестів (поки порожньо).

## Швидкі орієнтири
- Архітектурний принцип: **Data configs → Scene wiring → Core services → Gameplay adapters**.
- Стани модулів і борги — в `Docs/STATUS.md`.
- Детальний опис типів, залежностей і потоків — в `Docs/ARCHITECTURE.md`.
