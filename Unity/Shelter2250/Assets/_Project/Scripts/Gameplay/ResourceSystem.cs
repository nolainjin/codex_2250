using System;
using System.Collections.Generic;
using Shelter2250.Core;

namespace Shelter2250.Gameplay
{
    public sealed class ResourceSystem
    {
        private readonly List<ResourceMeter> resources;

        public float PowerPerRoomPerSecond = 0.05f;
        public float WaterPerDwellerPerSecond = 0.03f;
        public float FoodPerDwellerPerSecond = 0.025f;

        public ResourceSystem(List<ResourceMeter> resources)
        {
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public static List<ResourceMeter> CreateDefaultResources()
        {
            return new List<ResourceMeter>
            {
                new(ResourceKind.Power, current: 10, max: 10),
                new(ResourceKind.Water, current: 10, max: 10),
                new(ResourceKind.Food, current: 10, max: 10),
                new(ResourceKind.Medical, current: 5, max: 10),
                new(ResourceKind.Research, current: 0, max: 20),
                new(ResourceKind.Metal, current: 0, max: 30),
                new(ResourceKind.Parts, current: 20, max: 100),
                new(ResourceKind.Caps, current: 500, max: 0),
            };
        }

        public ResourceMeter Get(ResourceKind kind)
        {
            for (var i = 0; i < resources.Count; i++)
            {
                if (resources[i].kind == kind) return resources[i];
            }
            return null;
        }

        public void ResetProduction()
        {
            for (var i = 0; i < resources.Count; i++)
            {
                if (resources[i].kind == ResourceKind.Caps) continue;
                resources[i].productionPerSecond = 0f;
            }
        }

        public void AddProduction(ResourceKind kind, float amountPerSecond)
        {
            var meter = Get(kind);
            if (meter == null) return;
            if (kind == ResourceKind.Caps) return;
            meter.productionPerSecond += amountPerSecond;
        }

        public void AddCaps(float amount)
        {
            var caps = Get(ResourceKind.Caps);
            if (caps == null) return;
            caps.current += amount;
        }

        public bool SpendCaps(float amount)
        {
            var caps = Get(ResourceKind.Caps);
            if (caps == null) return false;
            if (caps.current < amount) return false;
            caps.current -= amount;
            return true;
        }

        public bool Spend(ResourceKind kind, float amount)
        {
            if (kind == ResourceKind.Caps) return SpendCaps(amount);

            var meter = Get(kind);
            if (meter == null) return false;
            if (meter.current < amount) return false;
            meter.current -= amount;
            return true;
        }

        public void Produce(ResourceKind kind, float amount)
        {
            var meter = Get(kind);
            if (meter == null) return;
            if (kind == ResourceKind.Caps)
            {
                meter.current += amount;
                return;
            }

            meter.current = Math.Clamp(meter.current + amount, 0, meter.max);
        }

        public Penalties Tick(GameState state, int activeRoomCount, int speedMultiplier)
        {
            var penalties = new Penalties();

            // Production: apply once per tick (1 second real-time), scaled by speed
            for (var i = 0; i < resources.Count; i++)
            {
                var meter = resources[i];
                if (meter.kind == ResourceKind.Caps) continue;

                if (meter.productionPerSecond <= 0f) continue;

                if (meter.kind != ResourceKind.Power && Get(ResourceKind.Power)?.current <= 0)
                {
                    penalties.noPower = true;
                    continue;
                }

                Produce(meter.kind, meter.productionPerSecond * speedMultiplier);
            }

            // Consumption
            var dwellers = state.dwellers.Count;
            var powerNeed = activeRoomCount * PowerPerRoomPerSecond * speedMultiplier;
            var waterNeed = dwellers * WaterPerDwellerPerSecond * speedMultiplier;
            var foodNeed = dwellers * FoodPerDwellerPerSecond * speedMultiplier;

            Consume(ResourceKind.Power, powerNeed, ref penalties.noPower);
            Consume(ResourceKind.Water, waterNeed, ref penalties.noWater);
            Consume(ResourceKind.Food, foodNeed, ref penalties.noFood);

            return penalties;
        }

        private void Consume(ResourceKind kind, float amount, ref bool flagIfEmpty)
        {
            var meter = Get(kind);
            if (meter == null) return;
            if (kind == ResourceKind.Caps) return;

            meter.current = Math.Clamp(meter.current - amount, 0, meter.max);
            if (meter.current <= 0f) flagIfEmpty = true;
        }

        public struct Penalties
        {
            public bool noFood;
            public bool noWater;
            public bool noPower;
        }
    }
}

