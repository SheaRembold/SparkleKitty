# Sparkle Kitty
An AR cat collecting game.

## Prerequisites
- [Unity 2017.3.0p2](https://unity3d.com/unity/qa/patch-releases?version=2017.3)

## Basic stuff
- Scene called "Main" is where game is being built.
- Currently the Inventory UI shows a list of all items in game and allows you to drag them into play area.
- If you add assets to the project put them in correct folders.

## Key Components
- DataManager: Contains all data for items in game. Data for each item is stored in a scriptable object.
- PlacementManager: Handles placing items into scene.
- PlayerManager: Contains current state of game, such as inventory, and handles changes to game state.
- UIManager: Handles switching between UI screens.