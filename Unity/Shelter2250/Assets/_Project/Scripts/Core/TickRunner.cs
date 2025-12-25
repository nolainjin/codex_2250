using System;
using UnityEngine;

namespace Shelter2250.Core
{
    public sealed class TickRunner : MonoBehaviour
    {
        [Tooltip("Real-time seconds per simulation tick.")]
        [SerializeField] private float tickIntervalSeconds = 1f;

        private float accumulator;

        public int CurrentSpeed { get; private set; } = GameSpeed.X1;

        public event Action<int> OnTick;

        public void SetSpeed(int speed)
        {
            CurrentSpeed = speed;
        }

        private void Update()
        {
            if (CurrentSpeed == GameSpeed.Paused) return;

            accumulator += Time.unscaledDeltaTime;
            while (accumulator >= tickIntervalSeconds)
            {
                accumulator -= tickIntervalSeconds;
                OnTick?.Invoke(CurrentSpeed);
            }
        }
    }
}

