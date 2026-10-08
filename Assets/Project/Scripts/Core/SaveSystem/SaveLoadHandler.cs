using System;
using System.IO;
using UnityEngine;

namespace MarketTycoon.Core.SaveSystem
{
    public static class SaveLoadManager
    {
        private static readonly string SaveFileName = "savegame.json";
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public static void Save(GameSaveData data)
        {
            try
            {
                if (data == null) return;

                data.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SaveFilePath, json);

                Debug.Log($"[SaveSystem] Progress saved successfully to: {SaveFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to save game data: {ex.Message}");
            }
        }

        public static GameSaveData Load()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    Debug.Log("[SaveSystem] No save file found. Initializing new game profile.");
                    return new GameSaveData();
                }

                string json = File.ReadAllText(SaveFilePath);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                Debug.Log("[SaveSystem] Progress loaded successfully.");
                return data ?? new GameSaveData();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to load data: {ex.Message}. Returning defaults.");
                return new GameSaveData();
            }
        }

        public static void ClearSave()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveSystem] Save file deleted.");
                }

                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Failed to delete save file: {ex.Message}");
            }
        }
    }
}