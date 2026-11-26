using ExiledToolkit.Game;
using GameCore.HotUpdate.Battle.Logic;

namespace ToiletFertilizer.Adapters;

/// <summary>
/// Asks the reward tables for one more check when a save loads or a trip starts (AgentManager.DeduplicateAgentSavesForLoad
/// runs for both). The target has no relative jump or call in its first 14 bytes; Game Rules, Package Bank and Wind
/// Power hook it too. Static because a Harmony patch has nowhere else to look.
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
            "DeduplicateAgentSavesForLoad",
            postfix: nameof(AfterLoad),
            consequence: "if another mod changes the toilet's package after the game starts, it is not written again until the setting changes");
    }

    /// <summary>Only sets a flag; the check runs at the next frame, outside the game's load.</summary>
    private static void AfterLoad() => Tables?.Redo();
}
