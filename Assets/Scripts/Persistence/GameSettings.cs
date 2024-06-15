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


        [field: SerializeField] public AudioSettings Audio { get; private set; }


        public static GameSettings Load() {
            string filepath = Path.Combine(Application.persistentDataPath, FILENAME);
            if (!File.Exists(filepath)) {
                var data = new GameSettings();
                data.Save();
                return data;
            }

            string json = File.ReadAllText(filepath);
            return JsonUtility.FromJson<GameSettings>(json);
        }

        public void Save() {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, FILENAME), json);
        }
    }
}
