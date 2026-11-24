using System;
using System.IO;
using UnityEngine;

namespace Aurora
{
    public static class SaveSystem
    {
        public static string SavePath => Path.Combine(Application.persistentDataPath, "aurora-save-v1.json");
        public static bool Exists => File.Exists(SavePath) || File.Exists(SavePath + ".bak");
        public static void Save(GameState state)
        {
            state.Validate();
            Directory.CreateDirectory(Application.persistentDataPath);
            string temp = SavePath + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(state, true));
            if (File.Exists(SavePath)) File.Copy(SavePath, SavePath + ".bak", true);
            // Mesma pasta e volume. Substituição atômica quando há arquivo anterior.
            if (File.Exists(SavePath)) File.Replace(temp, SavePath, null);
            else File.Move(temp, SavePath);
        }
        public static GameState Load()
        {
            foreach (string path in new[] { SavePath, SavePath + ".bak" })
            {
                try
                {
                    if (!File.Exists(path)) continue;
                    if (new FileInfo(path).Length > 100000) continue;
                    var state = JsonUtility.FromJson<GameState>(File.ReadAllText(path));
                    if (state == null) continue;
                    state.Validate(); if (state.hp == 0) state.hp = state.maxHp;
                    return state;
                }
                catch (Exception ex) { Debug.LogWarning("Salvamento não lido: " + ex.Message); }
            }
            return new GameState();
        }
    }
}
