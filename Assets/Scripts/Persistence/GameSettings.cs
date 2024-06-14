using System;
using System.IO;
using UnityEngine;

namespace QWOPCycle.Persistence {
    [Serializable]
    public struct AudioSettings {
        public bool IsMusicOn;
        public bool IsSfxOn;
        [Range(0f, 1f)] public float MusicVolume;
    }

    [Serializable]
    public sealed class GameSettings {
        private const string FILENAME = "settings.data";
        private static readonly string FILEPATH = Path.Combine(Application.persistentDataPath, FILENAME);


        [field: SerializeField] public AudioSettings Audio { get; private set; }


        public static GameSettings Load() {
            if (!File.Exists(FILEPATH)) {
                var data = new GameSettings();
                data.Save();
                return data;
            }

            string json = File.ReadAllText(FILEPATH);
            return JsonUtility.FromJson<GameSettings>(json);
        }

        public void Save() {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(FILEPATH, json);
        }
    }
}
