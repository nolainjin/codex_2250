using System;

namespace Shelter2250.Gameplay
{
    [Serializable]
    public sealed class RoomDefinition
    {
        public RoomKind kind;
        public string name;
        public string icon;
        public int width;
        public int height;

        public float baseProductionPerSecond;
        public ResourceKind produces;

        public float buildCostCaps;
        public float upgradeCostCaps;

        public int capacityIncrease;

        public RoomDefinition(
            RoomKind kind,
            string name,
            string icon,
            int width,
            int height,
            float baseProductionPerSecond,
            ResourceKind produces,
            float buildCostCaps,
            float upgradeCostCaps,
            int capacityIncrease = 0)
        {
            this.kind = kind;
            this.name = name;
            this.icon = icon;
            this.width = width;
            this.height = height;
            this.baseProductionPerSecond = baseProductionPerSecond;
            this.produces = produces;
            this.buildCostCaps = buildCostCaps;
            this.upgradeCostCaps = upgradeCostCaps;
            this.capacityIncrease = capacityIncrease;
        }
    }
}

