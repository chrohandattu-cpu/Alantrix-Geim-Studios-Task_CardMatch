# ARCHITECTURE

## Overview

The project is a Memory Card Matching game implemented in Unity. The gameplay loop consists of selecting cards, revealing their front faces, matching pairs, tracking score, and completing the board when all cards have been cleared.

The implementation follows a lightweight manager-driven architecture suitable for the project's scope.

## Main Components

### CardManager

CardManager acts as the central gameplay controller and is responsible for:

* Game initialization
* Board generation
* Card placement and scaling
* Pair allocation
* Match validation
* Score tracking
* Save/Load management
* Level selection
* Win condition evaluation

CardManager owns the collection of active cards and coordinates interactions between gameplay systems.

### Card

Card is responsible for:

* Card flip animation
* Front/back sprite display
* User interaction
* Match state tracking
* Visual fade-out behaviour
* Blank card handling

Each card manages its own visual state while CardManager owns game rules and progression.

### SaveManager

SaveManager is responsible for persistence.

Each level uses an independent save slot to allow progress to be restored separately.

Example:

* save_Level1.json
* save_Level2.json
* save_Level3.json

Saved information includes:

* Grid dimensions
* Card sprite assignments
* Matched card states
* Score
* Turn count

Transient animation state is intentionally not serialized.

### AudioHandler

AudioHandler provides centralized sound playback for:

* Card flips
* Victory feedback

## Data Flow

1. Player selects a level.
2. CardManager configures board dimensions.
3. A save file is checked.
4. Existing progress is restored if available.
5. Otherwise a new board is generated.
6. Cards report selections back to CardManager.
7. CardManager validates matches and updates score.
8. Progress is automatically saved during application pause and back-button events.

## Blank Card Strategy

Odd-sized boards such as:

* 3x3
* 5x3
* 5x5

cannot be filled exclusively with matching pairs.

To preserve a complete visual grid, a single blank card is inserted.

When revealed:

* The card disappears.
* It is marked as completed.
* It is persisted as completed in saved games.

This approach keeps board layouts visually consistent while maintaining pair-based gameplay.

## Save/Load Design

The save system restores a stable board state rather than attempting to restore animation state.

Persisted information:

* Card assignments
* Matched cards
* Turns
* Score

On load:

* Matched cards remain removed.
* Remaining cards are restored face-down.
* Gameplay continues normally.

This approach prioritizes consistency and avoids invalid states caused by interrupted animations.

## Trade-off

The primary trade-off is the use of a centralized CardManager.

For a larger project I would move matching logic, persistence, and scoring into dedicated systems to improve scalability and testability.

Given the project scope, the manager-based approach allowed rapid implementation while keeping complexity low.
# Alantrix-Geim-Studios-Task_CardMatch
