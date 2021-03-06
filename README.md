# Sparkle Kitty
An AR cat collecting game.

## Prerequisites
- [Unity 2017.3.1f1](https://store.unity.com/download)

## Basic stuff
- Scene called "Main" is where game is being built.
- Currently the Inventory UI shows a list of all items in game and allows you to drag them into play area.
- If you add assets to the project put them in correct folders.

## Adding assets
- add models to Models folder
- adjust scale factor on model
- create new game object (prefab) in Hierarchy
- make model child of game object by dragging it onto the prefab in the hierarchy panel and name the prefab the same as the model
- adjust position/rotation of selected child model
- update Data asset to reference the corresponding prefab
- commit and push to master

## Key Components
- DataManager: Contains all data for items in game. Data for each item is stored in a scriptable object.
- PlacementManager: Handles placing items into scene.
- PlayerManager: Contains current state of game, such as inventory, and handles changes to game state.
- UIManager: Handles switching between UI screens.
