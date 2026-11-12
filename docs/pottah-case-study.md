# POTTAH! / Engineering case study

## Goal

Develop a narrative-game prototype with branching choices, persistent story state and replay-aware interactions, using native Godot systems.

The current development target is a Windows game. Android, iOS and storefront availability are not claimed here.

## Systems implemented in the prototype

### Branching story and memory
The game tracks choices and evidence to drive route and ending conditions. Replay-aware behavior uses explicit game state rather than inspecting personal files.

### Save and recovery
Save/load and migration must handle interrupted, incomplete and malformed data. Missing history must remain unknown rather than becoming an invented player choice.

### Input and accessibility
Keyboard navigation and reduced-motion behavior are included in the engineering scope. Regression cases cover held keys, repeated events, pauses and returning to the title during transitions. Physical-controller and fresh native visual checks remain separate release gates.

### Audio
Music, voice and sound effects use separate controls. Recent work adds current-line replay/stop, missing-recording feedback and stale-audio cleanup. Test audio is not a substitute for approved production recordings or a listening review.

### Packaging
Tests exercise source behavior and packaged game data. A passing headless package test does not prove that a standalone Windows build works on a clean machine.

## Selected problem-solving example

At an independently reviewed checkpoint, a validation wrapper performed its import before assigning an isolated data directory. The import could log errors but still return a success exit code.

The review identified two requirements: set all isolated directories before import, and detect logged import errors rather than trusting the exit code alone. This illustrates why passing assertions and a green command are not enough on their own.

## Public presentation boundary

This case study describes observable product engineering. It does not publish private production systems, source archives, story scripts or media. Screenshots and downloadable builds are absent rather than represented by mockups.
