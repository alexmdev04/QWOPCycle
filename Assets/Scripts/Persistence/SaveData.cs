using System;
using System.IO;
using QWOPCycle.Scoring;
using UnityEngine;

namespace QWOPCycle.Persistence {
    [Serializable]
    public sealed class SaveData {
        private const string FILENAME = "save.data";
        private static readonly string FILEPATH = Path.Combine(Application.persistentDataPath, FILENAME);

        public uint HighScore { get; private set; }

        public static SaveData Load() {
            if (!File.Exists(FILEPATH)) {
                var data = new SaveData();
                data.Save();
                return data;
            }

            string json = File.ReadAllText(FILEPATH);
            return JsonUtility.FromJson<SaveData>(json);
        }


        public void Save() {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(FILEPATH, json);
        }

        public bool PostScore(ScoreTracker score) {
            if (score.Score > HighScore) {
                HighScore = score.Score;
                Save();
                return true;
            }

            return false;
        }
    }
}
