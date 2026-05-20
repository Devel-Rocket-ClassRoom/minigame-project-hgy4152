# Project Overview
- Game Title: Joker Mini Game
- High-Level Concept: A chain-matching game with RPG-style characters and a boss.
- Players: Single player
- Target Platform: Standalone Windows
- Render Pipeline: URP

# UI Sorting Fix Plan
## Issue
`JokerRewardUI` (and potentially other UI elements) is rendered behind world-space objects like characters and the boss because the `Canvas` is in `ScreenSpaceCamera` mode with a low `Sorting Order`.

## Solution
1. Increase the `Sorting Order` of the main `Canvas` to 100.
2. Verify `JokerRewardUI` and `StageIntroUI` are positioned correctly in the hierarchy to draw on top of other UI (e.g., behind the `SafeArea` or at the end of the children list).

# Key Asset & Context
- **Canvas**: The main UI container.
- **Sorting Order**: Property of the Canvas component.

# Implementation Steps
1. **Update Canvas Sorting Order**:
   - Select the `Canvas` object in the scene.
   - Set `Sorting Order` to `100` in the `Canvas` component.
   - Dependency: None.

2. **Verify Hierarchy**:
   - Ensure `JokerRewardUI` is at the bottom of the `Canvas` hierarchy so it appears on top of other UI elements.
   - Dependency: Step 1.

# Verification & Testing
- Run the game.
- Trigger the `StageIntroUI` (stage start) and `JokerRewardUI` (stage clear).
- Confirm they appear visually on top of characters and the boss.
