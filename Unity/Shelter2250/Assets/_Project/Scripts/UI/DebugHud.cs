using Shelter2250.Core;
using Shelter2250.Gameplay;
using UnityEngine;

namespace Shelter2250.UI
{
    public sealed class DebugHud : MonoBehaviour
    {
        [SerializeField] private GameController game;

        private void Awake()
        {
            if (!game) game = FindFirstObjectByType<GameController>();
        }

        private void OnGUI()
        {
            if (!game || game.State == null) return;

            GUILayout.BeginArea(new Rect(10, 10, 420, 600), GUI.skin.box);

            GUILayout.Label($"SHELTER 2250 (Simulation Only)");
            GUILayout.Label($"Time: {game.State.time}");
            GUILayout.Label($"Speed: x{game.State.gameSpeed}");

            GUILayout.Space(10);
            DrawResources();

            GUILayout.Space(10);
            DrawDwellers();

            GUILayout.Space(10);
            DrawControls();

            GUILayout.EndArea();
        }

        private void DrawResources()
        {
            GUILayout.Label("Resources");

            foreach (var r in game.State.resources)
            {
                if (r.kind == ResourceKind.Caps)
                {
                    GUILayout.Label($"- Caps: {r.current:0}  (+{game.LastCapsIncomePerTick:0.00}/tick)");
                }
                else
                {
                    GUILayout.Label($"- {r.kind}: {r.current:0.0}/{r.max:0.0}  (prod {r.productionPerSecond:0.00}/s)");
                }
            }
        }

        private void DrawDwellers()
        {
            GUILayout.Label($"Dwellers ({game.State.dwellers.Count})");
            for (var i = 0; i < game.State.dwellers.Count; i++)
            {
                var d = game.State.dwellers[i];
                GUILayout.Label($"- {d.name} [{d.status}] HP {d.health:0}/{d.maxHealth:0} H {d.hunger:0} T {d.thirst:0} ☢ {d.radiation:0}");
            }
        }

        private void DrawControls()
        {
            GUILayout.Label("Controls");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Pause")) game.SetSpeed(GameSpeed.Paused);
            if (GUILayout.Button("1x")) game.SetSpeed(GameSpeed.X1);
            if (GUILayout.Button("2x")) game.SetSpeed(GameSpeed.X2);
            if (GUILayout.Button("6x")) game.SetSpeed(GameSpeed.X6);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) game.Save();
            if (GUILayout.Button("Delete Save")) game.DeleteSave();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.Label("Build (caps only, placement adjacency required after first)");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("⚡")) game.TryBuildRoom(RoomKind.Power, 0, 0, out _);
            if (GUILayout.Button("💧")) game.TryBuildRoom(RoomKind.Water, 3, 0, out _);
            if (GUILayout.Button("🍖")) game.TryBuildRoom(RoomKind.Food, 6, 0, out _);
            GUILayout.EndHorizontal();
        }
    }
}

