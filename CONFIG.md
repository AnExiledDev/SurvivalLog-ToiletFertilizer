# Toilet Fertilizer settings

Settings live in `BepInEx\config\com.dev.survivallog.toiletfertilizer.cfg`, created the first time the game starts with the mod. Change them in game on the Mod Manager's Mods screen, where they take effect at once, or edit the file while the game is closed. Delete the file to get the defaults back. Without Exiled's Toolkit installed, changes made on the Mods screen take effect at the next game start instead.

## Reward

| Setting | Default | What it does |
| --- | --- | --- |
| `Fertilizer` | `PremiumOrganic` | Which fertilizer the toilet gives: `Basic` (Basic Fertilizer), `Compound` (Compound Fertilizer) or `PremiumOrganic` (Premium Organic Fertilizer, the best one). |
| `Amount` | `4` | How many of that fertilizer each toilet use gives, from 1 to 20 (one full stack). The game's own toilet gives 2. |

To get the game's own reward back while keeping the mod, set `Fertilizer` to `Basic` and `Amount` to `2`.
