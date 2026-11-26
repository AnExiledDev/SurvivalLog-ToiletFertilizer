using ToiletFertilizer.Core;
using Xunit;

namespace ToiletFertilizer.Tests;

public sealed class TableCheckTests
{
    [Fact]
    public void Tables_without_the_toilet_package_get_it_written()
    {
        // The game can build its tables again at start up, which drops the package. The toilet must not go back to
        // its normal reward then.
        Assert.Equal(TableCheck.Write, ToiletReward.Check(isWantedChanged: false, hasToiletPackage: false, isUsable: false));
    }

    [Fact]
    public void A_new_reward_is_written_even_when_the_package_is_fine()
    {
        Assert.Equal(TableCheck.Write, ToiletReward.Check(isWantedChanged: true, hasToiletPackage: true, isUsable: true));
    }

    [Fact]
    public void A_package_another_mod_broke_is_repaired()
    {
        Assert.Equal(TableCheck.Repair, ToiletReward.Check(isWantedChanged: false, hasToiletPackage: true, isUsable: false));
    }

    [Fact]
    public void A_package_that_is_still_fine_is_left_alone()
    {
        Assert.Equal(TableCheck.Skip, ToiletReward.Check(isWantedChanged: false, hasToiletPackage: true, isUsable: true));
    }
}
