using System;
using System.Collections.Generic;

namespace Shelter2250.Gameplay
{
    [Serializable]
    public sealed class RoomInstance
    {
        public string id;
        public RoomKind kind;

        public int x;
        public int y;
        public int width;
        public int height;

        public int level = 1;

        public bool isBuilding = true;
        public float buildProgressSeconds = 0f;
        public float buildDurationSeconds = 5f;

        public List<string> assignedDwellerIds = new List<string>();

        public static RoomInstance Create(RoomKind kind, int x, int y)
        {
            var def = RoomCatalog.Get(kind);
            return new RoomInstance
            {
                id = Guid.NewGuid().ToString("N"),
                kind = kind,
                x = x,
                y = y,
                width = def.width,
                height = def.height,
            };
        }

        public bool TickBuild(float seconds)
        {
            if (!isBuilding) return false;
            buildProgressSeconds += seconds;
            if (buildProgressSeconds >= buildDurationSeconds)
            {
                isBuilding = false;
                return true;
            }
            return false;
        }
    }
}

