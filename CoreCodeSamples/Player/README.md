# Player System

## Focus
Handles all core player functionality in the survival game, including combat, tool interaction, environmental stat management, and item usage.  
This module defines how the player fights, gathers resources, maintains survival stats, and interacts with the world using various tools.

## Key Scripts
**PlayerSkill.cs** – Manages all player abilities including attacking with different tools, interacting with world objects, and using special skills (shoveling, watering crops, planting).  
Implements animation-driven attacks and supports picking up, throwing, and consuming items with basic inventory handling.

**PlayerStatsManager.cs** – Tracks and updates player stats (health, hunger, warmth).  
Applies environmental effects when stats are low, manages state transitions (alive/dead), and synchronizes values with UI bars.

**PlayerToolController.cs** – Controls tool equipping and switching (axe, hammer, shovel, shield).  
Ensures only the active tool is enabled and manages hitbox activation for accurate and responsive combat interactions.
