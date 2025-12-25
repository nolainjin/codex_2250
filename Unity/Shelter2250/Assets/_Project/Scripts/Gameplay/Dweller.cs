using System;
using System.Collections.Generic;

namespace Shelter2250.Gameplay
{
    [Serializable]
    public sealed class Dweller
    {
        public string id;
        public string name;

        public int strength;
        public int perception;
        public int endurance;
        public int charisma;
        public int intelligence;
        public int agility;
        public int luck;

        public float health = 100;
        public float maxHealth = 100;
        public float happiness = 80;

        public float hunger = 100;
        public float thirst = 100;
        public float radiation = 0;
        public float fatigue = 0;

        public string status = "idle"; // idle, working, injured, dead
        public string assignedRoomId = null;

        public List<string> traits = new List<string>();

        public static Dweller CreateRandom(string forcedName = null)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            return new Dweller
            {
                id = Guid.NewGuid().ToString("N"),
                name = forcedName ?? RandomKoreanName(r),
                strength = RollStat(r),
                perception = RollStat(r),
                endurance = RollStat(r),
                charisma = RollStat(r),
                intelligence = RollStat(r),
                agility = RollStat(r),
                luck = RollStat(r),
                happiness = 75 + r.Next(0, 26),
            };
        }

        private static int RollStat(Random r) => r.Next(1, 11);

        private static string RandomKoreanName(Random r)
        {
            var surnames = new[]
            {
                "김","이","박","최","정","강","조","윤","장","임",
                "한","오","서","신","권","황","안","송","류","홍"
            };
            var given = new[]
            {
                "민수","영희","철수","지은","다은","민호","서연","준혁",
                "예린","태양","하늘","우진","유진","도현","지훈","수빈",
                "지민","현우","소연","승현","가영","진우","하린","민재"
            };

            return surnames[r.Next(0, surnames.Length)] + given[r.Next(0, given.Length)];
        }

        public float GetEfficiency(RoomKind room)
        {
            var baseStat = room switch
            {
                RoomKind.Power => strength,
                RoomKind.Water => perception,
                RoomKind.Food => agility,
                RoomKind.Farm => agility,
                RoomKind.Training => endurance,
                RoomKind.Medical => intelligence,
                RoomKind.Lab => intelligence,
                RoomKind.Refinery => strength,
                RoomKind.Living => charisma,
                _ => intelligence
            };

            var multiplier = 1f;

            if (hunger < 25) multiplier *= 0.6f;
            else if (hunger < 50) multiplier *= 0.85f;

            if (thirst < 25) multiplier *= 0.5f;
            else if (thirst < 50) multiplier *= 0.8f;

            if (fatigue > 80) multiplier *= 0.7f;
            else if (fatigue > 50) multiplier *= 0.9f;

            if (radiation > 150) multiplier *= 0.85f;

            if (status == "injured") multiplier *= 0.7f;
            if (status == "dead") multiplier = 0f;

            return baseStat * multiplier;
        }
    }
}

