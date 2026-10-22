using ToiletFertilizer.Core;
using Xunit;

namespace ToiletFertilizer.Tests;

public sealed class ToiletRewardTests
{
    [Theory]
    [InlineData(Fertilizer.Basic, 15501)]
    [InlineData(Fertilizer.Compound, 15502)]
    [InlineData(Fertilizer.PremiumOrganic, 15503)]
    public void Each_fertilizer_choice_hands_out_that_game_item(Fertilizer fertilizer, int itemId)
    {
        Assert.Equal(itemId, ToiletReward.For(fertilizer, 4).ItemId);
    }

    [Fact]
    public void The_chosen_amount_is_handed_out_as_set()
    {
        Assert.Equal(7, ToiletReward.For(Fertilizer.PremiumOrganic, 7).Amount);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(21, 20)]
    [InlineData(999, 20)]
    public void An_amount_outside_one_stack_is_held_to_between_one_and_a_full_stack(int asked, int given)
    {
        Assert.Equal(given, ToiletReward.For(Fertilizer.Basic, asked).Amount);
    }

    [Fact]
    public void A_toilet_effect_that_gave_the_shared_package_gives_the_toilet_package()
    {
        Assert.Equal(ToiletReward.ToiletPackageId, ToiletReward.Repoint(ToiletReward.SharedPackageId));
    }

    [Theory]
    [InlineData(30002)]
    [InlineData(104)]
    public void Other_rewards_of_a_toilet_effect_are_left_alone(int packageId)
    {
        Assert.Equal(packageId, ToiletReward.Repoint(packageId));
    }

    [Fact]
    public void Repointing_twice_keeps_the_toilet_package()
    {
        Assert.Equal(ToiletReward.ToiletPackageId, ToiletReward.Repoint(ToiletReward.Repoint(ToiletReward.SharedPackageId)));
    }

    [Fact]
    public void The_toilet_package_is_a_new_id_and_not_the_shared_one()
    {
        Assert.NotEqual(ToiletReward.SharedPackageId, ToiletReward.ToiletPackageId);
    }

    [Fact]
    public void Only_the_toilet_effects_are_repointed_never_research_or_item_use()
    {
        Assert.Equal(new[] { 112, 9075 }, ToiletReward.ToiletEffectIds);
        Assert.DoesNotContain(1921, ToiletReward.ToiletEffectIds);
        Assert.DoesNotContain(100024115, ToiletReward.ToiletEffectIds);
    }
}

public sealed class ToiletLifeTests
{
    [Fact]
    public void A_usable_freshness_range_from_the_shared_package_is_kept()
    {
        Assert.Equal(new LifeRange(0.5f, 0.6f), ToiletReward.UsableLife(0.5f, 0.6f));
    }

    [Fact]
    public void A_single_freshness_value_is_kept()
    {
        Assert.Equal(new LifeRange(0.7f, 0.7f), ToiletReward.UsableLife(0.7f, 0.7f));
    }

    [Fact]
    public void A_freshness_range_with_min_above_max_is_turned_around_so_the_game_does_not_throw()
    {
        var life = ToiletReward.UsableLife(0.6f, 0.5f);

        Assert.Equal(new LifeRange(0.5f, 0.6f), life);
        Assert.True(ToiletReward.IsUsable(life.Min, life.Max));
    }

    [Theory]
    [InlineData(float.NaN, 0.6f)]
    [InlineData(0.5f, float.NaN)]
    [InlineData(float.PositiveInfinity, 0.6f)]
    [InlineData(0.5f, float.NegativeInfinity)]
    [InlineData(0.5f, 3e6f)]
    [InlineData(-3e6f, 0.6f)]
    public void A_freshness_value_that_is_not_a_number_falls_back_to_the_games_own_range(float min, float max)
    {
        Assert.Equal(ToiletReward.DefaultLife, ToiletReward.UsableLife(min, max));
    }

    [Theory]
    [InlineData(0.6f, 0.5f)]
    [InlineData(float.NaN, 0.6f)]
    [InlineData(0.5f, float.PositiveInfinity)]
    [InlineData(0.5f, 3e6f)]
    public void A_range_the_game_would_throw_on_is_not_usable(float min, float max)
    {
        Assert.False(ToiletReward.IsUsable(min, max));
    }

    [Fact]
    public void The_fallback_is_the_game_toilet_packages_own_range()
    {
        Assert.Equal(new LifeRange(0.5f, 0.6f), ToiletReward.DefaultLife);
    }
}
