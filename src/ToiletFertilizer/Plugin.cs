using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using ExiledToolkit;
using ToiletFertilizer.Adapters;
using ToiletFertilizer.Core;

namespace ToiletFertilizer;

[BepInPlugin(Guid, "Toilet Fertilizer", Version)]
[BepInDependency(ExiledToolkit.Plugin.Guid, ">=0.19.0")]
public sealed class Plugin : BasePlugin
{
    private const string Guid = "com.dev.survivallog.toiletfertilizer";
    private const string Version = "0.2.2";

    public override void Load()
    {
        var fertilizer = Config.Bind("Reward", "Fertilizer", ToiletReward.GameFertilizer, new ConfigDescription(
            "Which fertilizer the toilet gives: Basic, Compound or PremiumOrganic (Premium Organic Fertilizer, the best one). The game's toilet gives Basic.\n" +
            "马桶给的肥料种类：Basic（基础肥料）、Compound（复合肥料）或 PremiumOrganic（高级有机肥料，效果最好）。游戏原本给 Basic。",
            null, WorldRules.Tag));
        var amount = Config.Bind("Reward", "Amount", ToiletReward.GameAmount,
            new ConfigDescription("How many of that fertilizer each toilet use gives. The game's toilet gives 2.\n" +
            "每次上厕所给多少个这种肥料。游戏原本给 2 个。",
            new AcceptableValueRange<int>(ToiletReward.MinAmount, ToiletReward.MaxAmount), WorldRules.Tag));

        var tables = new RewardTables(message => Log.LogInfo(message));

        Log.LogInfo($"===== Toilet Fertilizer {Version} started");
        RewardTicker.Tables = tables;
        LiveSettings.Watch(() => tables.Want(fertilizer.Value, amount.Value), fertilizer, amount);
        AddComponent<RewardTicker>();
    }
}
