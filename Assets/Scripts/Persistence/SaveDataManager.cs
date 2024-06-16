using QWOPCycle.Gameplay;
using SideFX.Events;
using UnityEngine;

namespace QWOPCycle.Persistence {
    /// <summary>
    /// Send this via the event bus to write the current save data to disk
    /// </summary>
    public readonly struct SaveGameEvent : IEvent { }

    /// <summary>
    /// Send this via the event bus to write the current game settings to disk
    /// </summary>
    public readonly struct SaveSettingsEvent : IEvent { }

    public sealed class SaveDataManager : MonoBehaviour {
        public static SaveDataManager Instance;
        public SaveData Save { get; private set; }
        public GameSettings Settings { get; private set; }

        private EventBinding<GameOver> _gameOverBinding;

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void OnEnable() {
            Save = SaveData.Load();
            Settings = GameSettings.Load();

            _gameOverBinding = new EventBinding<GameOver>(OnGameOver);

            EventBus<GameOver>.Register(_gameOverBinding);
        }

        private void OnDisable() {
            Save.Save();
            Settings.Save();
            EventBus<GameOver>.Deregister(_gameOverBinding);
        }


        private void OnGameOver(GameOver e) {
            Save.Save(e.Score, e.Distance, e.RunTime);
        }
    }
}
