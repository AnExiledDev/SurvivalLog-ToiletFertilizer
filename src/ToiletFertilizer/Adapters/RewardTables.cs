using System;
using ExiledToolkit.Game;
using GameCore.HotUpdate;
using ToiletFertilizer.Core;
using Il2CppInts = Il2CppSystem.Collections.Generic.List<int>;

namespace ToiletFertilizer.Adapters;

/// <summary>
/// Writes the toilet's own package into the game's package table and points the toilet effects at it, once the game has
/// loaded its tables. The game builds its tables once, at game start, so this runs only when it is due: at start, when
/// the reward setting changes, and when a save loads (in case another mod changed the package). While the tables are
/// not loaded yet it tries again each second, and stops once it has written.
/// </summary>
internal sealed class RewardTables
{
    private const float RetrySeconds = 1f;

    private readonly Action<string> _log;
    private (Fertilizer Fertilizer, int Amount) _wanted = (ToiletReward.GameFertilizer, ToiletReward.GameAmount);
    private (IntPtr Packages, IntPtr Effects) _applied;
    private bool _isWantedChanged;

    /// <summary>Set once a rewrite after another mod broke the package is logged, so a mod that keeps breaking it does not fill the log.</summary>
    private bool _isRepairLogged;

    private bool _isDue = true;
    private float _nextTryAt;

    public RewardTables(Action<string> log) => _log = log;

    public void Want(Fertilizer fertilizer, int amount)
    {
        _wanted = (fertilizer, amount);
        _isWantedChanged = true;
        _isDue = true;
    }

    /// <summary>A save loaded: check the package once more at the next frame.</summary>
    public void Redo() => _isDue = true;

    public void Tick(float now)
    {
        if (!_isDue || now < _nextTryAt) return;

        _nextTryAt = now + RetrySeconds;

        var config = GameSingleton.Existing<ConfigManager>();
        var packages = config?._Config_Package_Dict;
        var effects = config?._Config_Effect_Dict;

        if (packages == null || effects == null || packages.Count == 0 || effects.Count == 0) return;

        _isDue = false;

        var tables = (packages.Pointer, effects.Pointer);

        var isRepair = tables == _applied && !_isWantedChanged;

        if (isRepair && IsToiletPackageUsable(packages)) return;

        _applied = tables;
        _isWantedChanged = false;

        try
        {
            Write(packages, effects, isQuiet: isRepair && _isRepairLogged);
            _isRepairLogged |= isRepair;
        }
        catch (Exception ex)
        {
            _log("could not change the toilet reward, so the toilet gives its normal reward: " + ex);
        }
    }

    private void Write(
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Package> packages,
        Il2CppSystem.Collections.Generic.Dictionary<int, Config_Effect> effects,
        bool isQuiet)
    {
        Action<string> log = isQuiet ? _ => { } : _log;

        if (!packages.ContainsKey(ToiletReward.SharedPackageId))
        {
            log($"package {ToiletReward.SharedPackageId} is missing from the game, so the toilet gives its normal reward");

            return;
        }

        var reward = ToiletReward.For(_wanted.Fertilizer, _wanted.Amount);

        packages[ToiletReward.ToiletPackageId] = ToiletPackage(packages[ToiletReward.SharedPackageId], reward, log);

        var repointed = 0;

        foreach (var effectId in ToiletReward.ToiletEffectIds)
        {
            if (effects.ContainsKey(effectId) && RepointRewards(effects[effectId].ItemGetEnd)) repointed++;
        }

        log($"the toilet now gives {reward.Amount} {ToiletReward.Name(_wanted.Fertilizer)} ({repointed} of {ToiletReward.ToiletEffectIds.Count} toilet effects use it)");
    }

    /// <summary>
    /// False when something has changed the toilet package's freshness range since it was written into one the game
    /// throws on, so this check writes it again.
    /// </summary>
    private static bool IsToiletPackageUsable(Il2CppSystem.Collections.Generic.Dictionary<int, Config_Package> packages)
    {
        if (!packages.ContainsKey(ToiletReward.ToiletPackageId)) return true;

        var row = packages[ToiletReward.ToiletPackageId];

        return row == null || ToiletReward.IsUsable(row.LifeMin, row.LifeMax);
    }

    /// <summary>A copy of the shared package with the chosen reward, so the game reads it like its own.</summary>
    private static Config_Package ToiletPackage(Config_Package shared, Reward reward, Action<string> log)
    {
        var life = ToiletReward.UsableLife(shared.LifeMin, shared.LifeMax);

        if (life != new LifeRange(shared.LifeMin, shared.LifeMax))
        {
            log($"package {ToiletReward.SharedPackageId} has a freshness range the game cannot use ({shared.LifeMin} to {shared.LifeMax}), so the toilet uses {life.Min} to {life.Max}");
        }

        var row = new Config_Package
        {
            ID = ToiletReward.ToiletPackageId,
            PackageName = shared.PackageName,
            PackageName_Local = shared.PackageName_Local,
            RandomID = new Il2CppInts(),
            RandomAmount = new Il2CppInts(),
            ItemIdList = new Il2CppInts(),
            ItemCountList = new Il2CppInts(),
            LifeMin = life.Min,
            LifeMax = life.Max,
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
