using System;
using System.Collections.Generic;

namespace Shelter2250.Gameplay
{
    public sealed class DwellerSystem
    {
        private readonly List<Dweller> dwellers;

        public float HungerRecoverPerTick = 0.25f;
        public float ThirstRecoverPerTick = 0.25f;

        public float HungerLossNoFoodPerTick = 1.5f;
        public float ThirstLossNoWaterPerTick = 2.0f;

        public DwellerSystem(List<Dweller> dwellers)
        {
            this.dwellers = dwellers ?? throw new ArgumentNullException(nameof(dwellers));
        }

        public void TickAll(ResourceSystem.Penalties penalties, int speedMultiplier)
        {
            for (var i = 0; i < dwellers.Count; i++)
            {
                Tick(dwellers[i], penalties, speedMultiplier);
            }
        }

        private void Tick(Dweller d, ResourceSystem.Penalties penalties, int speedMultiplier)
        {
            if (d.status == "dead") return;

            var step = Math.Max(1, speedMultiplier);

            var hungerDelta = penalties.noFood ? -HungerLossNoFoodPerTick * step : HungerRecoverPerTick * step;
            var thirstDelta = penalties.noWater ? -ThirstLossNoWaterPerTick * step : ThirstRecoverPerTick * step;

            d.hunger = Math.Clamp(d.hunger + hungerDelta, 0, 100);
            d.thirst = Math.Clamp(d.thirst + thirstDelta, 0, 100);

            if (d.hunger == 0) d.health = Math.Clamp(d.health - 0.2f * step, 0, d.maxHealth);
            else if (d.hunger < 25) d.health = Math.Clamp(d.health - 0.08f * step, 0, d.maxHealth);

            if (d.thirst == 0) d.health = Math.Clamp(d.health - 0.4f * step, 0, d.maxHealth);
            else if (d.thirst < 25) d.health = Math.Clamp(d.health - 0.12f * step, 0, d.maxHealth);

            if (penalties.noPower) d.happiness = Math.Clamp(d.happiness - 0.2f * step, 0, 100);

            if (d.hunger < 50 || d.thirst < 50) d.happiness = Math.Clamp(d.happiness - 0.3f * step, 0, 100);
            else if (!penalties.noFood && !penalties.noWater) d.happiness = Math.Clamp(d.happiness + 0.2f * step, 0, 100);

            d.radiation = Math.Max(0, d.radiation + 0.01f * step);
            if (d.radiation > 200) d.health = Math.Clamp(d.health - 0.1f * step, 0, d.maxHealth);

            var fatigueRate = penalties.noPower ? 2f : 1f;
            d.fatigue = Math.Clamp(d.fatigue + 0.2f * step * fatigueRate, 0, 100);

            if (d.health <= 0)
            {
                d.status = "dead";
            }
        }
    }
}

