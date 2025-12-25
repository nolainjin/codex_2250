using System;
using System.IO;
using UnityEngine;

namespace Shelter2250.Core
{
    public sealed class SaveService
    {
        private const string SaveFileName = "shelter2250_save.json";
        private const string WebGlPlayerPrefsKey = "shelter2250_save_json";

        public bool HasSave()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return PlayerPrefs.HasKey(WebGlPlayerPrefsKey);
            }

            return File.Exists(GetSavePath());
        }

        public void Save(GameState state)
        {
            var json = JsonUtility.ToJson(state, prettyPrint: true);

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                PlayerPrefs.SetString(WebGlPlayerPrefsKey, json);
                PlayerPrefs.Save();
                return;
            }

            var path = GetSavePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? Application.persistentDataPath);
            File.WriteAllText(path, json);
        }

        public bool TryLoad(out GameState state)
        {
            state = null;

            try
            {
                string json;
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    if (!PlayerPrefs.HasKey(WebGlPlayerPrefsKey)) return false;
                    json = PlayerPrefs.GetString(WebGlPlayerPrefsKey);
                }
                else
                {
                    var path = GetSavePath();
                    if (!File.Exists(path)) return false;
                    json = File.ReadAllText(path);
                }

                if (string.IsNullOrWhiteSpace(json)) return false;
                state = JsonUtility.FromJson<GameState>(json);
                return state != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Save load failed: {ex.Message}");
                return false;
            }
        }

        public void DeleteSave()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                PlayerPrefs.DeleteKey(WebGlPlayerPrefsKey);
                return;
            }

            var path = GetSavePath();
            if (File.Exists(path)) File.Delete(path);
        }

        private static string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, SaveFileName);
        }
    }
}

