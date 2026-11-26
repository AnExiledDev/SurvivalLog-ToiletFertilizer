using ExiledToolkit.Game;
using GameCore.HotUpdate.Battle.Logic;
using GameCore.HotUpdate;

namespace ToiletFertilizer.Adapters;

/// <summary>
/// Asks the reward tables for one more check when a run starts: a save load, a trip, and a new game's first start
/// (AgentManager.StartBattle runs for all of them). Static because a Harmony patch has nowhere else to look.
/// </summary>
internal static class LoadHook
{
    internal static RewardTables? Tables;

    public static void Apply(PatchKit hooks, RewardTables tables)
    {
        Tables = tables;
        hooks.Patch(
            typeof(LoadHook),
            typeof(AgentManager),
            nameof(AgentManager.StartBattle),
            new[] { typeof(GameSaveData), typeof(GameMode) },
            postfix: nameof(AfterLoad),
            consequence: "if another mod changes the toilet's package after the game starts, it is not written again until the setting changes");
    }

    /// <summary>Only sets a flag; the check runs at the next frame, outside the game's load.</summary>
    private static void AfterLoad() => Tables?.Redo();
}
