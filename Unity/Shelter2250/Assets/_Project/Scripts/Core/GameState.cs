using System;
using System.Collections.Generic;
using Shelter2250.Gameplay;

namespace Shelter2250.Core
{
    [Serializable]
    public sealed class GameState
    {
        public int schemaVersion = 1;

        public GameTime time = new GameTime();

        public List<ResourceMeter> resources = new List<ResourceMeter>();
        public List<RoomInstance> rooms = new List<RoomInstance>();
        public List<Dweller> dwellers = new List<Dweller>();

        public int gameSpeed = GameSpeed.X1;

        public static GameState CreateNew()
        {
            var state = new GameState();
            state.resources = ResourceSystem.CreateDefaultResources();
            state.dwellers.Add(Dweller.CreateRandom());
            state.dwellers.Add(Dweller.CreateRandom());
            return state;
        }
    }
}

