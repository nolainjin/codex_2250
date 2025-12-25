using Shelter2250.Gameplay;
using UnityEngine;

namespace Shelter2250.Core
{
    public sealed class GameController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TickRunner tickRunner;

        private SaveService saveService;
        private GameState state;

        private ResourceSystem resourceSystem;
        private ShelterGrid shelter;
        private DwellerSystem dwellerSystem;

        public GameState State => state;
        public ResourceSystem ResourceSystem => resourceSystem;

        public float LastCapsIncomePerTick { get; private set; }

        private void Awake()
        {
            saveService = new SaveService();

            if (!tickRunner)
            {
                tickRunner = gameObject.AddComponent<TickRunner>();
            }

            if (saveService.TryLoad(out var loaded))
            {
                state = loaded;
            }
            else
            {
                state = GameState.CreateNew();
            }

            RebuildSystems();

            tickRunner.SetSpeed(state.gameSpeed);
            tickRunner.OnTick += OnTick;
        }

        private void OnDestroy()
        {
            if (tickRunner != null)
            {
                tickRunner.OnTick -= OnTick;
            }
        }

        private void RebuildSystems()
        {
            resourceSystem = new ResourceSystem(state.resources);
            shelter = new ShelterGrid(state.rooms);
            dwellerSystem = new DwellerSystem(state.dwellers);
        }

        public void SetSpeed(int speed)
        {
            state.gameSpeed = speed;
            tickRunner.SetSpeed(speed);
        }

        public void Save()
        {
            saveService.Save(state);
        }

        public void DeleteSave()
        {
            saveService.DeleteSave();
        }

        public bool TryBuildRoom(RoomKind kind, int x, int y, out string reason)
        {
            var room = RoomInstance.Create(kind, x, y);
            var def = RoomCatalog.Get(kind);

            if (!resourceSystem.SpendCaps(def.buildCostCaps))
            {
                reason = "not enough caps";
                return false;
            }

            var allowIsolated = state.rooms.Count == 0;
            if (!shelter.TryAdd(room, allowIsolated, out reason))
            {
                resourceSystem.AddCaps(def.buildCostCaps);
                return false;
            }

            // Auto assign first idle dwellers if any
            for (var i = 0; i < state.dwellers.Count && room.assignedDwellerIds.Count < 2; i++)
            {
                var d = state.dwellers[i];
                if (string.IsNullOrEmpty(d.assignedRoomId) && d.status != "dead")
                {
                    AssignDwellerToRoom(d.id, room.id);
                }
            }

            reason = null;
            return true;
        }

        public bool AssignDwellerToRoom(string dwellerId, string roomId)
        {
            var dweller = state.dwellers.Find(d => d.id == dwellerId);
            var room = state.rooms.Find(r => r.id == roomId);
            if (dweller == null || room == null) return false;
            if (dweller.status == "dead") return false;

            // Unassign from previous room
            if (!string.IsNullOrEmpty(dweller.assignedRoomId))
            {
                var prev = state.rooms.Find(r => r.id == dweller.assignedRoomId);
                prev?.assignedDwellerIds.Remove(dweller.id);
            }

            dweller.assignedRoomId = room.id;
            dweller.status = "working";
            if (!room.assignedDwellerIds.Contains(dweller.id))
            {
                room.assignedDwellerIds.Add(dweller.id);
            }
            return true;
        }

        public void UnassignDweller(string dwellerId)
        {
            var dweller = state.dwellers.Find(d => d.id == dwellerId);
            if (dweller == null) return;

            if (!string.IsNullOrEmpty(dweller.assignedRoomId))
            {
                var room = state.rooms.Find(r => r.id == dweller.assignedRoomId);
                room?.assignedDwellerIds.Remove(dweller.id);
            }

            dweller.assignedRoomId = null;
            if (dweller.status != "dead") dweller.status = "idle";
        }

        private void OnTick(int speedMultiplier)
        {
            if (speedMultiplier == GameSpeed.Paused) return;

            state.time.AdvanceMinutes(speedMultiplier);

            // Build progress
            for (var i = 0; i < state.rooms.Count; i++)
            {
                state.rooms[i].TickBuild(seconds: 1f * speedMultiplier);
            }

            // Production (rooms -> resource production)
            resourceSystem.ResetProduction();
            var activeRooms = 0;
            for (var i = 0; i < state.rooms.Count; i++)
            {
                var room = state.rooms[i];
                if (room.isBuilding) continue;
                activeRooms++;

                var def = RoomCatalog.Get(room.kind);
                if (def.baseProductionPerSecond <= 0f) continue;
                resourceSystem.AddProduction(def.produces, def.baseProductionPerSecond + CalculateRoomEfficiencyBonus(room));
            }

            var penalties = resourceSystem.Tick(state, activeRooms, speedMultiplier);

            // Dwellers
            dwellerSystem.TickAll(penalties, speedMultiplier);

            // Caps income loop (simple baseline)
            var capsIncome = CalculateCapsIncome(penalties, speedMultiplier);
            LastCapsIncomePerTick = capsIncome;
            if (capsIncome > 0) resourceSystem.AddCaps(capsIncome);
        }

        private float CalculateRoomEfficiencyBonus(RoomInstance room)
        {
            if (room.assignedDwellerIds.Count == 0) return 0f;

            var sum = 0f;
            for (var i = 0; i < room.assignedDwellerIds.Count; i++)
            {
                var d = state.dwellers.Find(x => x.id == room.assignedDwellerIds[i]);
                if (d == null) continue;
                sum += d.GetEfficiency(room.kind) * 0.1f;
            }
            return sum;
        }

        private float CalculateCapsIncome(ResourceSystem.Penalties penalties, int speedMultiplier)
        {
            if (penalties.noFood || penalties.noWater) return 0f;

            var basePerWorkerPerSecond = 0.08f;
            var income = 0f;

            for (var i = 0; i < state.dwellers.Count; i++)
            {
                var d = state.dwellers[i];
                if (d.status != "working") continue;

                var happinessFactor = 0.4f + (Mathf.Clamp01(d.happiness / 100f) * 0.8f);
                var charismaFactor = 0.6f + (Mathf.Clamp(d.charisma, 1, 10) / 10f) * 0.8f;
                var luckFactor = 0.8f + (Mathf.Clamp(d.luck, 1, 10) / 10f) * 0.4f;

                var workerIncome = basePerWorkerPerSecond * happinessFactor * charismaFactor * luckFactor;

                if (d.hunger < 50) workerIncome *= 0.85f;
                if (d.thirst < 50) workerIncome *= 0.8f;
                if (d.fatigue > 60) workerIncome *= 0.85f;
                if (d.radiation > 150) workerIncome *= 0.9f;

                income += workerIncome;
            }

            if (state.dwellers.Count > 0)
            {
                var sumHappy = 0f;
                for (var i = 0; i < state.dwellers.Count; i++) sumHappy += state.dwellers[i].happiness;
                var avgHappy = sumHappy / state.dwellers.Count;
                income += 0.01f * state.dwellers.Count * (0.5f + (avgHappy / 100f) * 0.5f);
            }

            if (penalties.noPower) income *= 0.7f;

            income *= Mathf.Max(1, speedMultiplier);
            return Mathf.Max(0, income);
        }
    }
}

