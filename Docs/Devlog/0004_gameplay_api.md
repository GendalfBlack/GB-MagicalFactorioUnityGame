# Devlog — Gameplay

## Мета
`Gameplay` реалізує scene-рівневу поведінку поверх Core-сервісів: рух гравця, наведення/побудову, спавн візуальних нод, Gizmos.

## Склад модуля
- `PlayerController2D` — рух через `Input.GetAxisRaw` (Horizontal/Vertical).
- `PlayerGridProbe` — проєкція позиції гравця у `GridCoord` через `root.Grid.WorldToGrid`.
- `BuildModeController`:
  - читає позицію миші (`MouseWorld.GetWorld2D`)
  - рахує `TargetCoord`
  - перевіряє build-range (Manhattan distance)
  - викликає `Placement.CanPlace/TryPlace/TryRemove`
  - перемикає тип: `Alpha1 -> Water`, `Alpha2 -> Fire`
  - малює Gizmo підсвітку таргет-клітинки.
- `NodeSpawner`:
  - підписується на `NodePlaced/NodeRemoved`
  - інстанціює prefab у `GridToWorldCenter`
  - зберігає активні обʼєкти в `Dictionary<int, GameObject>` за `NodeId.Value`.
- `NodeView` — зберігає `NodeId` і `ElementType` на prefab-інстансі.
- `GridGizmosRenderer` (`[ExecuteAlways]`) — малює межу, лінії сітки та центри клітинок за `GridConfigSO`.
- `MouseWorld` — утиліта перетворення screen mouse position у world 2D.

## Крос-інтеграція
- Залежить від `Scenes.CompositionRoot` як точки доступу до `IGrid` та `IPlacementService`.
- Використовує `Core.Grid` (координати, перетворення, межі).
- Використовує `Core.Placement` (стан розміщення, події, типи елементів).
- Використовує `GameData.GridConfigSO` для editor Gizmos.

## Сценарій потоку
1. `CompositionRoot` ініціалізує `Grid`/`Placement`.
2. `PlayerGridProbe` щокадру оновлює координату гравця.
3. `BuildModeController` визначає таргет-клітинку курсора та доступність побудови.
4. LMB/RMB змінюють стан в `PlacementService`.
5. `NodeSpawner` реагує на події та оновлює візуальне представлення в сцені.

## Обмеження/нотатки
- У `BuildModeController` є зайвий `using UnityEngine.tvOS;` (не впливає на логіку, але варто прибрати).
- Частина коментарів у коді має проблеми кодування (бажано нормалізувати в UTF-8).
