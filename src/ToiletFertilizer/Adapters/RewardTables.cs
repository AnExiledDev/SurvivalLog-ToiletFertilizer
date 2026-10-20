using System;
using GameCore.HotUpdate;
using ToiletFertilizer.Core;
using Il2CppInts = Il2CppSystem.Collections.Generic.List<int>;

namespace ToiletFertilizer.Adapters;

/// <summary>
/// Writes the toilet's own package into the game's package table and points the toilet effects at it, once the game has
/// loaded its tables. It writes again when the game loads the tables afresh or the reward setting changes.
/// </summary>
internal sealed class RewardTables
{
    private const float CheckSeconds = 1f;

    private readonly Action<string> _log;
    private (Fertilizer Fertilizer, int Amount) _wanted = (Fertilizer.PremiumOrganic, 4);
    private (IntPtr Packages, IntPtr Effects) _applied;
    private bool _isWantedChanged;
    private float _nextCheckAt;

    public RewardTables(Action<string> log) => _log = log;

    public void Want(Fertilizer fertilizer, int amount)
    {
        _wanted = (fertilizer, amount);
        _isWantedChanged = true;
    }

    public void Tick(float now)
    {
        if (now < _nextCheckAt) return;

        _nextCheckAt = now + CheckSeconds;

        if (!BaseSingleton<ConfigManager>.IsInstanceCreated) return;

        var config = BaseSingleton<ConfigManager>.Instance;
        var packages = config?._Config_Package_Dict;
        var effects = config?._Config_Effect_Dict;

        if (packages == null || effects == null || packages.Count == 0 || effects.Count == 0) return;

        var tables = (packages.Pointer, effects.Pointer);

        if (tables == _applied && !_isWantedChanged) return;

        _applied = tables;
        _isWantedChanged = false;

        try
        {
            Write(packages, effects);
        }
        catch (Exception ex)
        {
            _log("could not change the toilet reward, so the toilet keeps the game's own: " + ex);
        }
    }

    private void Write(
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Package> packages,
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Effect> effects)
    {
        if (!packages.ContainsKey(ToiletReward.SharedPackageId))
        {
            _log($"package {ToiletReward.SharedPackageId} is missing from the game, so the toilet keeps the game's own reward");

            return;
        }

        var reward = ToiletReward.For(_wanted.Fertilizer, _wanted.Amount);

        packages[ToiletReward.ToiletPackageId] = ToiletPackage(packages[ToiletReward.SharedPackageId], reward);

        var repointed = 0;

        foreach (var effectId in ToiletReward.ToiletEffectIds)
        {
            if (effects.ContainsKey(effectId) && RepointRewards(effects[effectId].ItemGetEnd)) repointed++;
        }

        _log($"the toilet now gives {reward.Amount} {ToiletReward.Name(_wanted.Fertilizer)} ({repointed} of {ToiletReward.ToiletEffectIds.Count} toilet effects use it)");
    }

    /// <summary>A copy of the shared package with the chosen reward, so the game reads it like its own.</summary>
    private static Config_Package ToiletPackage(Config_Package shared, Reward reward)
    {
        var row = new Config_Package
        {
            ID = ToiletReward.ToiletPackageId,
            PackageName = shared.PackageName,
            PackageName_Local = shared.PackageName_Local,
            RandomID = new Il2CppInts(),
            RandomAmount = new Il2CppInts(),
            ItemIdList = new Il2CppInts(),
            ItemCountList = new Il2CppInts(),
            LifeMin = shared.LifeMin,
            LifeMax = shared.LifeMax,
        };

        row.ItemIdList.Add(reward.ItemId);
        row.ItemCountList.Add(reward.Amount);

        return row;
    }

    /// <summary>True when the effect hands out the toilet's package after the swap.</summary>
    private static bool RepointRewards(Il2CppInts? rewards)
    {
        if (rewards == null) return false;

        var isToiletPackage = false;

        for (var i = 0; i < rewards.Count; i++)
        {
            rewards[i] = ToiletReward.Repoint(rewards[i]);
            isToiletPackage |= rewards[i] == ToiletReward.ToiletPackageId;
        }

        return isToiletPackage;
    }
}
