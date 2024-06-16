using System;
using System.IO;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Persistence {
    [Serializable]
    public struct SaveData {
        private const string FILENAME = "save.data";

        [field: SerializeField] public double HighScore { get; private set; }

        [field: SerializeField] public float BestDistance { get; private set; }

        [SerializeField] private double _bestRunTime;

        public TimeSpan BestRunTime => TimeSpan.FromSeconds(_bestRunTime);

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

        public bool PostScore(double score) {
            if (score <= HighScore) return false;

        public void Save(uint score, float distance, TimeSpan runTime) {
            HighScore = score > HighScore ? score : HighScore;
            BestDistance = distance > BestDistance ? distance : BestDistance;
            _bestRunTime = runTime.TotalSeconds > _bestRunTime ? runTime.TotalSeconds : _bestRunTime;
            Save();
        }
    }
}
