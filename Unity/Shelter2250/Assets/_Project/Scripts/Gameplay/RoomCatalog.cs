namespace Shelter2250.Gameplay
{
    public static class RoomCatalog
    {
        public static RoomDefinition Get(RoomKind kind)
        {
            // Phase 1: hardcoded defaults (move to ScriptableObjects later)
            return kind switch
            {
                RoomKind.Power => new RoomDefinition(RoomKind.Power, "발전실", "⚡", 3, 2, 0.4f, ResourceKind.Power, 100, 50),
                RoomKind.Water => new RoomDefinition(RoomKind.Water, "정수실", "💧", 3, 2, 0.35f, ResourceKind.Water, 100, 50),
                RoomKind.Food => new RoomDefinition(RoomKind.Food, "식당", "🍖", 4, 2, 0.3f, ResourceKind.Food, 120, 60),
                RoomKind.Farm => new RoomDefinition(RoomKind.Farm, "재배실", "🌿", 3, 2, 0.4f, ResourceKind.Food, 130, 65),
                RoomKind.Living => new RoomDefinition(RoomKind.Living, "거주 공간", "🏠", 3, 2, 0f, ResourceKind.Caps, 80, 40, capacityIncrease: 4),
                RoomKind.Training => new RoomDefinition(RoomKind.Training, "훈련실", "💪", 2, 2, 0f, ResourceKind.Caps, 150, 75),
                RoomKind.Storage => new RoomDefinition(RoomKind.Storage, "창고", "📦", 3, 2, 0f, ResourceKind.Caps, 120, 60),
                RoomKind.Medical => new RoomDefinition(RoomKind.Medical, "의료실", "💊", 2, 2, 0.15f, ResourceKind.Medical, 180, 90),
                RoomKind.Lab => new RoomDefinition(RoomKind.Lab, "연구실", "🔬", 3, 2, 0.2f, ResourceKind.Research, 200, 100),
                RoomKind.Refinery => new RoomDefinition(RoomKind.Refinery, "제련공간", "⛓️", 3, 2, 0.25f, ResourceKind.Metal, 180, 90),
                _ => new RoomDefinition(RoomKind.Power, "발전실", "⚡", 3, 2, 0.4f, ResourceKind.Power, 100, 50)
            };
        }
    }
}

