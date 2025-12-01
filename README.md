# Resilient: Echoes of Life

## Overview
Resilient: Echoes of Life is a third-person survival adventure game developed in Unity, inspired by Valheim and Sons of the Forest. The project features a modular survival gameplay architecture including resource gathering, crafting, farming, hunting, environmental hazards, and multi-phase boss encounters.

This repository contains the full Unity project for technical review, along with documentation that highlights the most critical systems and scripts through the CoreCodeSamples/ directory.

## Technical Highlights
- Modular Survival Framework – Covers core mechanics including resource gathering, crafting, hunting, cooking, farming, and environmental stat management.
- Dynamic Player Ability System – Supports tool-based interactions (axe, spear, shovel, hammer, shield), contextual actions, and animation-driven combat.
- Environmental Stat System – Real-time player management of health, hunger, and warmth, with environmental penalties and recovery mechanics.
- Animal AI System – Includes docile wildlife (wander/flee), aggressive animals (provoked attack), and predators that pursue the player dynamically.
- Multi-Stage Boss Architecture – Two full-featured bosses with state machines, attack modes, animation events, and HP-based phase transitions.
- Event-Driven Gameplay Flow – Uses Unity events and state checks to manage interactions such as chopping, harvesting, feeding, crafting, and combat.

## How to Use

### Requirements
- Unity 2021.3 LTS or later
- Windows
- No additional packages required

### Setup
1. Clone or download the repository.
2. Open the project using Unity Hub.
3. Load any level scene such as:
   Assets/Scenes/Level1.unity
4. Press Play to begin exploring, crafting, hunting, and progressing through the survival levels.

## Core System Architecture
This project includes a complete Unity implementation, while the CoreCodeSamples/ folder highlights the most essential gameplay systems for review and documentation. Each submodule represents a major component of the survival and combat framework, reflecting a clean, modular, and scalable architecture.

### CoreCodeSamples/Player/
Contains the systems governing the player's combat, interaction, stat management, and tool handling. Includes logic for attacking, equipping tools, consuming items, updating health/hunger/warmth, and handling survival-related state transitions.

### CoreCodeSamples/Survival/
Contains the scripts supporting resource gathering, crafting, harvesting, and environmental interaction. Includes chopping trees, spawning materials, physics-based resource drops, and various survival world interactions.

### CoreCodeSamples/EnemyBossAI/
Implements all wildlife AI and the two multi-phase bosses. Includes docile animals, aggressive animals, predators, and bosses with state machines, attacks, animations, and HP-based phase changes.

## Links
- 🌐 [Portfolio Page](https://www.henrywang.online/copy-of-minetruck) – Full project breakdown and gameplay demo video 
