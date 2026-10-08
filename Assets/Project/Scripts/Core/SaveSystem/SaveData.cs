using System;
using System.Collections.Generic;

namespace MarketTycoon.Core.SaveSystem
{
    [Serializable]
    public class BuildingSpotSaveEntry
    {
        public string SpotId;
        public bool IsUnlocked;
    }

    [Serializable]
    public class GameSaveData
    {
        public int Money;
        public long LastSaveTimestamp;
        public List<BuildingSpotSaveEntry> BuildingSpots = new List<BuildingSpotSaveEntry>();

        public GameSaveData()
        {
            Money = 0;
            LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            BuildingSpots = new List<BuildingSpotSaveEntry>();
        }
    }
}