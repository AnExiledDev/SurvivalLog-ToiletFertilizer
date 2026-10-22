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

/// <summary>The freshness range a package hands its items out with, as fractions of a new item.</summary>
public readonly record struct LifeRange(float Min, float Max);

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

    /// <summary>The shared package's own freshness range in the game's data.</summary>
    public static readonly LifeRange DefaultLife = new(0.5f, 0.6f);

    /// <summary>
    /// Far above any real freshness. The game turns freshness times 1000 into an int, and past about 2 million that
    /// overflows into a range it throws on.
    /// </summary>
    public const float MaxLife = 1000f;

    /// <summary>
    /// The game picks freshness with Random.Next(min, max), which throws when min is above max and the toilet then gives
    /// nothing, so only a range of real numbers with min at or below max is safe to hand it.
    /// </summary>
    public static bool IsUsable(float min, float max) => IsReal(min) && IsReal(max) && min <= max;

    /// <summary>A freshness range the game can use: the given one, turned around if inverted, or the game's own if it is not a real number.</summary>
    public static LifeRange UsableLife(float min, float max)
    {
        if (!IsReal(min) || !IsReal(max)) return DefaultLife;

        return min <= max ? new LifeRange(min, max) : new LifeRange(max, min);
    }

    private static bool IsReal(float life) => float.IsFinite(life) && Math.Abs(life) <= MaxLife;

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
