using UnityEngine;

namespace MarketTycoon.Gameplay.Building
{
    public enum BuildingType
    {
        Shelf,
        Checkout,
        RoomExpansion
    }

    [CreateAssetMenu(fileName = "BuildingConfig_New", menuName = "Market Tycoon/Configs/Building Config")]
    public class BuildingConfigSO : ScriptableObject
    {
        [Header("General")]
        [SerializeField] private string _buildingId = "spot_shelf_02";
        [SerializeField] private string _buildingTitle = "New Bread Shelf";
        [SerializeField] private BuildingType _type = BuildingType.Shelf;

        [Header("Economy")]
        [Min(0)]
        [SerializeField] private int _buildCost = 150;

        [Header("Visuals")]
        [Tooltip("A prefab or a game object that becomes operational once construction is complete")]
        [SerializeField] private GameObject _constructedPrefab;

        public string BuildingId => _buildingId;
        public string BuildingTitle => _buildingTitle;
        public BuildingType Type => _type;
        public int BuildCost => _buildCost;
        public GameObject ConstructedPrefab => _constructedPrefab;
    }
}