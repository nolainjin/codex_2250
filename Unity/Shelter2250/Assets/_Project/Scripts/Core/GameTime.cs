using System;

namespace Shelter2250.Core
{
    [Serializable]
    public sealed class GameTime
    {
        public int year = 2250;
        public int month = 1;
        public int day = 1;
        public int hour = 0;
        public int minute = 0;

        public long totalMinutes = 0;

        public void AdvanceMinutes(int minutesToAdd)
        {
            if (minutesToAdd <= 0) return;

            totalMinutes += minutesToAdd;
            minute += minutesToAdd;

            while (minute >= 60)
            {
                minute -= 60;
                hour += 1;
            }

            while (hour >= 24)
            {
                hour -= 24;
                day += 1;
            }

            while (month > 12)
            {
                month -= 12;
                year += 1;
            }

            while (day > DaysInMonth(year, month))
            {
                day -= DaysInMonth(year, month);
                month += 1;
                if (month > 12)
                {
                    month = 1;
                    year += 1;
                }
            }
        }

        private static int DaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(Math.Max(1, year), Math.Clamp(month, 1, 12));
        }

        public override string ToString()
        {
            return $"{year}년 {month}월 {day}일 {hour:D2}:{minute:D2}";
        }
    }
}

