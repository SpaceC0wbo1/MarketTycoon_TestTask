using System;

namespace MarketTycoon.Gameplay.Building
{
    public class BuildingSpotModel
    {
        private readonly BuildingConfigSO _config;
        private bool _isUnlocked;

        public string BuildingId => _config.BuildingId;
        public string Title => _config.BuildingTitle;
        public int Cost => _config.BuildCost;
        public BuildingType Type => _config.Type;
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
    }
}