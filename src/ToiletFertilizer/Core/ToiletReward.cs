using System;
using System.Collections.Generic;

namespace ToiletFertilizer.Core;

/// <summary>The three fertilizers the game has, weakest first.</summary>
public enum Fertilizer
{
    Basic,
    Compound,
    PremiumOrganic,
}

/// <summary>What one toilet use hands out: an item and how many.</summary>
public readonly record struct Reward(int ItemId, int Amount);

/// <summary>
/// The toilet's reward rules. The game's toilet hands out package 30001 (2 Basic Fertilizer), but research and one
/// item use the same package, so the toilet is pointed at a package of its own instead of editing the shared one.
/// </summary>
public static class ToiletReward
{
    public const int SharedPackageId = 30001;

    /// <summary>Unused by the game, whose packages stop at 30039.</summary>
    public const int ToiletPackageId = 39501;

    public const int MinAmount = 1;

    /// <summary>One full stack of fertilizer.</summary>
    public const int MaxAmount = 20;

    /// <summary>The end effects of the toilet actions, the only rows whose reward is repointed.</summary>
    public static readonly IReadOnlyList<int> ToiletEffectIds = new[] { 112, 9075 };

    public static Reward For(Fertilizer fertilizer, int amount) => new(ItemId(fertilizer), Math.Clamp(amount, MinAmount, MaxAmount));

    public static int ItemId(Fertilizer fertilizer) => fertilizer switch
    {
        Fertilizer.Basic => 15501,
        Fertilizer.Compound => 15502,
        Fertilizer.PremiumOrganic => 15503,
        _ => throw new ArgumentOutOfRangeException(nameof(fertilizer), fertilizer, null),
    };

    public static string Name(Fertilizer fertilizer) => fertilizer switch
    {
        Fertilizer.Basic => "Basic Fertilizer",
        Fertilizer.Compound => "Compound Fertilizer",
        Fertilizer.PremiumOrganic => "Premium Organic Fertilizer",
        _ => throw new ArgumentOutOfRangeException(nameof(fertilizer), fertilizer, null),
    };

    /// <summary>A toilet effect's reward entry after the swap: the shared package becomes the toilet's own, the rest stay.</summary>
    public static int Repoint(int packageId) => packageId == SharedPackageId ? ToiletPackageId : packageId;
}
