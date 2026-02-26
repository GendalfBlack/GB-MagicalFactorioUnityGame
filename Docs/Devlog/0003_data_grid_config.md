# Devlog — Data (GridConfigSO)

## Мета
`Data` містить ScriptableObject-конфігурації, які підключаються в scene-рівні модулі без hardcode параметрів.

## Склад модуля
- `GameData.GridConfigSO`:
  - World: `cellSize`, `originWorld`
  - Bounds: `minX`, `minY`, `width`, `height`
  - Gizmos: `draw`, `drawCellCenters`, `centerMarkerSize`
- `Data.asmdef` — окрема збірка для data-шару.
- У репозиторії присутній приклад asset: `Assets/Data/GridConfigSO.asset`.

## Правила валідації
- Атрибути Inspector:
  - `[Min(0.01f)]` для `cellSize`
  - `[Min(1)]` для `width`, `height`
  - `[Min(0.1f)]` для `centerMarkerSize`

## Крос-інтеграція
- `Scenes.CompositionRoot` читає `GridConfigSO` і ініціалізує:
  - `GridRect`
  - `GridSettings`
  - `GridService`
  - `PlacementService`
- `Gameplay.Grid.GridGizmosRenderer` використовує той самий `GridConfigSO` для візуалізації сітки в Gizmos.
- Таким чином один asset синхронізує runtime-логіку (`Core.Grid`) і editor-візуалізацію (`Gameplay.Grid`).

## Обмеження
- `Data` не містить runtime-логіку gameplay.
- При `autoReferenced: false` у asmdef залежні збірки мають явно посилатися на `Data`.
