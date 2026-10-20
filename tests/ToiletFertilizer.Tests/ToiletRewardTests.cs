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
