using System;
using UnityEngine;

namespace ToiletFertilizer.Adapters;

/// <summary>Gives the reward tables a frame clock, so they can wait for the game to load.</summary>
public sealed class RewardTicker : MonoBehaviour
{
    internal static RewardTables? Tables;

    public RewardTicker(IntPtr pointer) : base(pointer)
    {
    }

    private void Update() => Tables?.Tick(Time.unscaledTime);
}
