# Enemy & Boss AI System

## Focus
Implements the full creature and boss behavior system for the survival game, including docile wildlife, aggressive predators, and two multi-phase boss encounters.  
This module defines AI state machines, attack logic, health handling, animations, and special abilities that drive all hostile and non-hostile interactions.

## Key Scripts

**GenerAnimalController_2.cs** – Controls docile animals that wander, flee when approached, and rest after fleeing.  
Manages health, death, and resource drops, integrating animations and environment-driven behavior logic.

**AttackAnimalController.cs** – Handles aggressive animals that attack when provoked.  
Transitions between wander, chase, and attack states, and manages health and resource drops on death.

**FindPlayerAnimalController.cs** – Controls predator animals that actively hunt the player.  
Includes teleportation logic for inaccessible player locations and manages death behavior and resource spawning.

**BossController.cs** – Manages the first boss (Dragon) with multi-mode attacks such as Fireball, Firebreath, and Poison.  
Handles attack animations, delays, transitions, and dynamic flying movement around the player.

**BossHealthController.cs** – Tracks and updates boss health values and UI.  
Triggers special boss events (timelines, phase transitions) based on health thresholds.

**Boss_2Controller.cs** – Implements the final boss (Demon) with a full state machine including melee, fire breath, and whip attacks.  
Transitions between idle, chase, and attack states, and controls special combat effects and attack sequences.
