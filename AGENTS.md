# AGENTS.md

## Project Direction

This is a Unity 3D mobile isometric survival-strategy RPG prototype.

The player controls a hero, develops a camp, commands squads, builds defenses, and survives night monster attacks.

The project is no longer a 2D lane battler. New gameplay should target 3D.

## Core Gameplay

The mission/round is divided into repeated day and night phases.

### Day Phase

During the day, the player can:

* Control the hero with a joystick.
* Explore a small map.
* Clear neutral camps.
* Collect resources.
* Build or upgrade camp structures.
* Prepare squads and defenses before the night attack.

Construction uses fixed building slots, not free placement.

### Night Phase

During the night:

* Monsters attack the camp in waves.
* Enemies come through clearly defined attack directions/passages.
* For MVP, use one attack direction.
* The player defends the MainBuilding.
* The hero auto-attacks enemies within range.
* Player squads and defensive buildings help defend the base.
* Building and repairing at night should be restricted or disabled for MVP.

## Win / Lose Conditions

* Win condition: survive all night waves in the mission/round.
* Lose condition: MainBuilding is destroyed.
* Hero death is not an immediate loss. The hero may respawn later.

Do not use enemy base destruction as a win condition.

## Camp / Buildings

The camp consists of multiple buildings.

Important building types:

* MainBuilding - critical structure; if destroyed, the player loses.
* Walls - block or delay enemies on attack passages.
* Towers - defensive buildings.
* Barracks - provide access to player squads.
* ResourceBuildings - generate or improve resource income.
* SupportBuildings - healing, buffs, magic, utility, etc.

MVP should start with:

* MainBuilding.
* One attack passage.
* One wall slot.
* One barracks slot.
* Optional tower slot later.

## Hero

The hero is controlled directly by the player.

Rules:

* Movement: joystick.
* Combat: auto-attack enemies in range.
* Abilities can be added later.
* Hero death does not fail the mission.

## Squad-Based Combat

Both player forces and ordinary enemy forces are represented as squads.

A squad is one gameplay entity with:

* Shared HP.
* Shared movement.
* Shared targeting.
* Shared commands or AI.
* One controller.

A squad may visually contain multiple members, but for MVP it can be represented by one placeholder object.

Later, squad damage can scale with alive member count.

Example:

* 3 members alive = 100% damage.
* 2 members alive = reduced damage.
* 1 member alive = reduced damage.

Player squads:

* Can persist between waves until destroyed.
* Can be selected.
* Can receive commands:
  * Move to position.
  * Attack target.

Enemy squads:

* Ordinary enemies should spawn as enemy squads, not many independent units.
* Enemy squads attack the camp during night waves.

Elite enemies and bosses:

* Elite enemies may be represented as individual enemy heroes.
* Bosses may be represented as unique individual boss entities.

## Resources

Use two main resource types:

### Gold

Used for:

* Building structures.
* Repairing structures.
* Upgrading structures.

### Energy / Command

Used for:

* Summoning squads.
* Restoring squads.
* Possibly using commands or abilities.

The existing EnergySystem can remain as the temporary combat resource system unless renamed later.

Add a separate Gold/resource system later.

## Enemy Design

Enemies should support different future roles and priorities.

Possible enemy types later:

* Basic melee squad.
* Ranged squad.
* Siege squad.
* Jumper that bypasses walls.
* Bomber that damages structures.
* Assassin that targets hero or squads.
* Enemy hero.
* Boss.

For MVP:

* Use one basic enemy squad.
* It moves toward the camp/main target.
* It attacks valid targets in range.
* It uses simple direct movement.

## Technical Direction

Use Unity 3D for new gameplay.

Rules:

* Use 3D scene objects.
* Use 3D physics for new gameplay.
* Units move on the XZ plane.
* Y is height.
* Use Collider / Rigidbody when needed.
* Do not use Collider2D / Rigidbody2D for new gameplay.
* Use Physics.OverlapSphere for simple detection in MVP.
* Do not add NavMesh/pathfinding yet.
* Use simple direct movement first.
* Use an orthographic isometric/top-down camera.
* Keep mobile performance in mind.

## Architecture Rules

Keep code simple, modular, and MVP-focused.

General rules:

* Prefer small focused components over large manager classes.
* Use ScriptableObjects for configuration.
* Use serialized references in MonoBehaviours for MVP.
* Avoid global singletons for gameplay systems.
* Avoid Service Locator unless explicitly approved.
* Avoid FindObjectOfType and GameObject.Find in gameplay logic.
* Do not introduce third-party packages without approval.
* Do not implement ads, IAP, analytics, backend, multiplayer, or online features in MVP.
* Do not modify unrelated files.
* Do not change project settings unless explicitly requested.
* Do not modify Unity scenes unless explicitly requested.
* Do not use Unity MCP tools unless explicitly requested.

## Suggested Core Components

Reusable systems:

* Team
* Health
* BaseController
* EnergySystem
* GoldResourceSystem
* WaveConfig
* WaveManager
* HealthBarView

3D gameplay systems to add or adapt:

* HeroController
* HeroAutoAttack
* SquadConfig
* SquadController
* SquadCommandController
* EnemySquadController
* BuildingController
* BuildingSlot
* PhaseManager
* MissionManager
* AttackDirection
* EnemySpawner3D

## Scene Rules

The old BattleScene can remain as a prototype backup.

New 3D prototype scene:

Assets/Game/Scenes/BattleScene3D.unity

Expected scene structure:

* Main Camera
* Directional Light
* BattleRoot
  * ArenaRoot
  * MainBuilding
  * HeroSpawn
  * SquadSpawn
  * EnemyAttackDirections
  * BuildingSlots
  * UnitsRoot
  * ProjectilesRoot
  * VFXRoot
  * BattleSystems
  * BattleCanvas
  * EventSystem

Runtime-spawned squads/enemies should be parented under UnitsRoot.

Runtime-spawned projectiles should be parented under ProjectilesRoot.

Runtime-spawned VFX should be parented under VFXRoot.

## MVP Priority

Current development priority:

1. Save current prototype before 3D pivot.
2. Create BattleScene3D.
3. Set up 3D arena, orthographic isometric camera, and basic lighting.
4. Add MainBuilding with Health and BaseController.
5. Add hero joystick movement.
6. Add hero auto-attack.
7. Add day/night PhaseManager.
8. Add one enemy attack direction.
9. Add enemy squad movement on XZ plane.
10. Add one player squad as a shared-HP gameplay object.
11. Add basic squad selection.
12. Add squad move command.
13. Add squad attack command.
14. Add Gold resource system.
15. Keep Energy/Command for squad summon/restore.
16. Add fixed building slots.
17. Add one barracks slot.
18. Add one wall slot.
19. Connect night wave spawning and win/lose conditions.

## Avoid

Do not describe the current game as:

* 2D lane battler.
* One horizontal lane.
* Side-view base push game.
* Enemy base destruction game.

Do not use for new gameplay:

* Physics2D.
* Collider2D.
* Rigidbody2D.
* One-dimensional X-axis movement.
* 2D lane-specific assumptions.
