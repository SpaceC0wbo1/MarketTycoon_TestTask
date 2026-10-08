using System;

namespace MarketTycoon.Gameplay.Building
{
    public class BuildingSpotModel
    {
        private readonly BuildingConfigSO _config;
        private bool _isUnlocked;

        public string Title => _config != null ? _config.BuildingTitle : string.Empty;
        public int Cost => _config != null ? _config.BuildCost : 0;
        public BuildingType Type => _config != null ? _config.Type : BuildingType.Shelf;
        public bool IsUnlocked => _isUnlocked;

        public event Action OnUnlocked;

        public BuildingSpotModel(BuildingConfigSO config, bool initiallyUnlocked = false)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _isUnlocked = initiallyUnlocked;
        }

        public bool TryUnlock()
        {
            if (_isUnlocked) return false;

            _isUnlocked = true;
            OnUnlocked?.Invoke();
            return true;
        }

        public void RestoreUnlockedState(bool isUnlocked)
        {
            _isUnlocked = isUnlocked;
        }
    }
}