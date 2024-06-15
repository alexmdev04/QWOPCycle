using SideFX.Events;
using Unity.Logging;
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

        private EventBinding<SaveGameEvent> _saveGameBinding;
        private EventBinding<SaveSettingsEvent> _saveSettingsBinding;

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void OnEnable() {
            Save = SaveData.Load();
            Settings = GameSettings.Load();

            _saveGameBinding = new EventBinding<SaveGameEvent>(OnSaveGame);
            _saveSettingsBinding = new EventBinding<SaveSettingsEvent>(OnSaveSettings);

            EventBus<SaveGameEvent>.Register(_saveGameBinding);
            EventBus<SaveSettingsEvent>.Register(_saveSettingsBinding);
        }

        private void OnDisable() {
            EventBus<SaveGameEvent>.Deregister(_saveGameBinding);
            EventBus<SaveSettingsEvent>.Deregister(_saveSettingsBinding);
        }

        private void OnSaveGame() {
            Log.Debug("SaveDataManager : Saving Player Data");
            Save.Save();
        }

        private void OnSaveSettings() {
            Log.Debug("SaveDataManager : Saving Settings");
            Settings.Save();
        }
    }
}