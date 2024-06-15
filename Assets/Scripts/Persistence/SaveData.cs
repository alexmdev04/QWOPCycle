using System;
using System.IO;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Persistence {
    [Serializable]
    public sealed class SaveData {
        private const string FILENAME = "save.data";

        [field: SerializeField] public uint HighScore { get; private set; }

        [field: SerializeField] public float BestDistance { get; private set; }

        public static SaveData Load() {
            string filepath = Path.Combine(Application.persistentDataPath, FILENAME);
            if (!File.Exists(filepath)) {
                Log.Info("Creating new Save File");

                var data = new SaveData();
                data.Save();

                return data;
            }

            string json = File.ReadAllText(filepath);
            return JsonUtility.FromJson<SaveData>(json);
        }


        public void Save() {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, FILENAME), json);
        }

        public bool PostScore(uint score) {
            if (score <= HighScore) return false;

            HighScore = score;
            Save();
            return true;
        }

        public bool PostDistance(float distance) {
            if (distance <= BestDistance) return false;

            BestDistance = distance;
            Save();
            return true;
        }
    }
}
