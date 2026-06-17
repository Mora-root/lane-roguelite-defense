## Project

This is a Unity 2D mobile lane battler / roguelite defense prototype.

The player has a base on the left side.
The enemy has a base on the right side.
Units move along one horizontal lane.
The player spends energy to summon units and push toward the enemy base.

The main goal of each battle is to destroy the enemy base.

## Current gameplay direction

* 2D side-view
* Portrait mobile screen
* One horizontal lane
* Short 1–2 minute battles
* Hero-led combat
* Unit summoning using energy
* Enemy base destruction as the win condition
* Roguelite upgrades between waves
* Permanent rewards and progression after defeat

## Development rules

* Keep code simple and modular.
* Prefer small focused components over large manager classes.
* Use ScriptableObjects for unit, wave, hero, ability, and upgrade configuration.
* Do not introduce third-party packages without approval.
* Do not implement networking, ads, IAP, analytics, backend, or multiplayer in the MVP.
* Do not modify unrelated files.
* Do not change project settings unless explicitly requested.
* Avoid global singletons for gameplay systems.
* Avoid `FindObjectOfType`, `GameObject.Find`, and hidden scene lookups in gameplay logic.
* Keep code readable for a solo developer.

## DI-friendly architecture

The project does not use a DI framework yet.

However, code should be written in a DI-friendly way so that Zenject, VContainer, or another DI framework can be introduced later if needed.

Rules:

* Do not use global service singletons for gameplay systems.
* Do not use Service Locator patterns unless explicitly approved.
* Prefer explicit dependencies through `SerializeField` for MonoBehaviours in the MVP.
* Prefer constructor injection for plain C# classes.
* Keep pure gameplay logic separate from Unity-specific MonoBehaviour code when reasonable.
* Use interfaces only when they make dependencies clearer, not everywhere by default.
* Systems should expose events or methods instead of directly controlling unrelated systems.
* UI should observe gameplay state and send player commands, not contain core gameplay rules.
* Factories and spawners should be responsible for creating runtime objects like units, projectiles, and VFX.

## Folder structure

Scripts should go under:

Assets/Game/Scripts/

Subfolders:

* Core
* Battle
* Units
* Hero
* Energy
* Waves
* Upgrades
* Rewards
* Save
* UI

ScriptableObjects should go under:

Assets/Game/ScriptableObjects/

Subfolders:

* Units
* Heroes
* Abilities
* Waves
* Upgrades

Prefabs should go under:

Assets/Game/Prefabs/

Subfolders:

* Units
* Bases
* UI
* Projectiles
* VFX

Scenes should go under:

Assets/Game/Scenes/

Art should go under:

Assets/Game/Art/

Audio should go under:

Assets/Game/Audio/

## MVP scope

The MVP includes:

* One battle scene
* Main camera and battle layout
* Player base and enemy base
* One hero
* Energy generation
* Unit summoning
* Basic unit movement
* Basic melee/ranged combat
* Enemy spawning
* Wave progression
* Temporary run upgrades
* Simple rewards after defeat

The MVP does not include:

* PvP
* Clans
* Ads
* In-app purchases
* Backend
* Analytics
* Complex meta progression
* Multiple biomes
* Final art
* Final UI
* Advanced animation system

## Coding style

* Use C# events for UI updates where appropriate.
* Use `SerializeField` for scene references in the early MVP.
* Use plain C# classes for pure logic where possible.
* Keep MonoBehaviour classes focused on Unity lifecycle, scene references, and presentation.
* Keep runtime object creation inside factories/spawners.
* Add short comments only when logic is not obvious.
* Prefer clear names over clever abstractions.
* Avoid premature optimization.
* Avoid overengineering before the core battle loop is playable.

## Scene rules

The main prototype scene is:

Assets/Game/Scenes/BattleScene.unity

The battle scene should contain:

* Main Camera
* BattleRoot
* PlayerBaseSpawn
* EnemyBaseSpawn
* PlayerUnitSpawn
* EnemyUnitSpawn
* LaneRoot
* UnitsRoot
* ProjectilesRoot
* VFXRoot
* BattleCanvas
* EventSystem

Runtime-spawned units should be parented under `UnitsRoot`.

Runtime-spawned projectiles should be parented under `ProjectilesRoot`.

Runtime-spawned visual effects should be parented under `VFXRoot`.

## Current priority

The current priority is to create a playable battle prototype.

Order of implementation:

1. Project folder structure
2. BattleScene setup
3. Team enum
4. Health component
5. BaseController
6. EnergySystem
7. UnitConfig ScriptableObject
8. UnitController
9. UnitSpawner
10. Basic combat
11. Battle UI
12. WaveManager
13. Run upgrades
14. Rewards after defeat

Do not skip ahead to monetization, meta systems, or content expansion before the core battle loop is playable.
