# Devlog — Scenes (CompositionRoot)

## Мета
`Scenes` містить точку композиції застосунку: створює Core-сервіси на основі data-конфігу та віддає їх gameplay-модулям.

## Склад модуля
- `Scenes.CompositionRoot : MonoBehaviour`
  - serializable dependency: `GridConfigSO gridConfig`
  - runtime properties:
    - `IGrid Grid { get; private set; }`
    - `IPlacementService Placement { get; private set; }`

## Ініціалізація (Awake)
1. Валідує, що `gridConfig` призначений.
2. Створює `GridRect(minX, minY, width, height)`.
3. Створює `GridSettings(cellSize, originWorld, bounds)`.
4. Ініціалізує `Grid = new GridService(settings)`.
5. Ініціалізує `Placement = new PlacementService(Grid)`.
6. Логує параметри ініціалізації.

## Крос-інтеграція
- Джерело конфігурації: `GameData.GridConfigSO` (модуль `Data`).
- Створює сервіси з модулів `Core.Grid` і `Core.Placement`.
- Споживачі: `Gameplay.Player.PlayerGridProbe`, `Gameplay.Placement.BuildModeController`, `Gameplay.Placement.NodeSpawner`.

## Обмеження
- Якщо `gridConfig` не заданий, компонент disable-иться і решта gameplay-ланцюга не ініціалізується.
- Модуль не реалізує gameplay-правила напряму; лише збирає залежності.
