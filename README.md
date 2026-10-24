# Toilet Fertilizer

The toilet gives 4 Premium Organic Fertilizer instead of 2 Basic Fertilizer. The fertilizer and the amount can be changed.

## Features

- **Better fertilizer**: each toilet use gives Premium Organic Fertilizer, the best fertilizer in the game, instead of Basic Fertilizer.
- **More of it**: 4 per use by default instead of 2.
- **Your choice**: pick Basic, Compound or Premium Organic Fertilizer, and any amount from 1 to 20 (one full stack).
- **Nothing else changes**: research and other things that hand out the same fertilizer package as the toilet keep the game's own reward.

Toilet Fertilizer adds nothing to your save and does not edit game files. You can remove it at any time; the toilet goes back to 2 Basic Fertilizer at the next game start.

Every config setting is explained in English and Simplified Chinese.

## How it works

The game's toilet hands out a fertilizer package that research and one item also use. Instead of changing that shared package, the mod adds a package of its own once the game has loaded its tables, holding the fertilizer and amount you chose, and points only the toilet at it.

## Known issues and limits

- Only the toilet's reward changes. Anything else that gives fertilizer is left as the game has it.
- A game update that changes the toilet's reward may stop the mod from working until it is updated. If it cannot find the toilet's package, it says so in `BepInEx\LogOutput.log` and leaves the toilet alone.

## Requirements

- Survival Log 1.1.19611. Built for this version; a game update can break it until the mod is updated.
- The [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12), the BepInEx 6 (IL2CPP) build for the game.
- Optional: [Exiled's Toolkit](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit) 0.4.0 or newer. With it, settings changed on the Mod Manager's Mods screen take effect without restarting the game. Without it, they take effect at the next game start.

## Install

1. Install the [BepInEx Pack for Survival Log](https://www.nexusmods.com/survivallog/mods/12) (if no other mods were installed before, start the game once so BepInEx finishes setup, then quit).
2. Optional, to change settings without a restart: install Exiled's Toolkit. Download `ExiledToolkit-x.y.z.zip` from [its latest release](https://github.com/AnExiledDev/SurvivalLog-ExiledToolkit/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/61), and extract it into the game folder (the folder with `SurvivalLog.exe`).
3. Download `ToiletFertilizer-x.y.z.zip` from the [latest release](https://github.com/AnExiledDev/SurvivalLog-ToiletFertilizer/releases/latest) or from [Nexus Mods](https://www.nexusmods.com/survivallog/mods/77).
4. Extract the zip into the game folder (the folder with `SurvivalLog.exe`). It holds a `BepInEx` folder, so the DLL lands in the right place. When asked, merge the folders. The result should be:
   `C:\Program Files (x86)\Steam\steamapps\common\Survival Log\BepInEx\plugins\ToiletFertilizer.dll`

## Uninstall

Delete `ToiletFertilizer.dll` from the `BepInEx\plugins` folder. For a clean slate, also delete `BepInEx\config\com.dev.survivallog.toiletfertilizer.cfg`.

## Configuration

Settings live in `BepInEx\config\com.dev.survivallog.toiletfertilizer.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. See [CONFIG.md](CONFIG.md) for every setting. Without Exiled's Toolkit installed, changes made on the Mods screen take effect at the next game start instead.

## Troubleshooting

Built against BepInEx `6.0.0-be.788`.

Check `BepInEx\LogOutput.log` for `===== Toilet Fertilizer 0.1.1 started`. Once a game has loaded, a line starting with `the toilet now gives` shows the fertilizer and amount in use and how many of the 2 toilet effects use it.

## Feedback and bug reports

Found a bug, have an idea, or something feels off? Come say so on the Core Directive Discord: https://discord.gg/trTsyxUFqg . You can also post in the Posts tab of the mod's Nexus Mods page. When reporting a bug, attach `BepInEx\LogOutput.log`.

To reach me directly, message `exiled.dev` on Discord; you can find me in the [official Survival Log Discord](https://discord.gg/MZGg4gQAUe). I'm also on X as [@AnExiledDev](https://x.com/AnExiledDev).

A Survival Log wiki with detailed documentation for all of my mods is coming soon.

## Build

This is a BepInEx 6 IL2CPP plugin. It compiles against BepInEx and the game's IL2CPP interop assemblies, so a game install with BepInEx set up and started once is required. Those assemblies come from the game and are not part of this repo. The .NET 8 SDK is required.

```
dotnet build src/ToiletFertilizer -c Release -p:GameDir="C:\path\to\Survival Log"
```

`GameDir` is the game folder. The default in the project file is a local path you will almost certainly need to override. The output DLL is at `src\ToiletFertilizer\bin\Release\ToiletFertilizer.dll`.

The unit tests cover the pure reward rules and need nothing from the game:

```
dotnet test tests/ToiletFertilizer.Tests
```

## License

MIT. Copyright (c) 2026 AnExiledDev. See [LICENSE](LICENSE).
