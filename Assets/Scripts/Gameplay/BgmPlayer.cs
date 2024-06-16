using System.Collections;
using System.Collections.Generic;
using QWOPCycle.Persistence;
using UnityEngine;

namespace QWOPCycle
{
    public class BgmPlayer : MonoBehaviour {
        private GameSettings _settings;
        [Header("Audio")]
        public AudioSource gameMusicSource;
        public AudioClip gameMusic;

        [Header("Settings")]
        public int pitchTickRate = 2;
        private int _pitchTick = 0;
        // Start is called before the first frame update
        void Start() {
            PlayMusic();
        }

        private void PlayMusic() {
            if (gameMusicSource.isPlaying) return;
            if (gameMusicSource == null
                || gameMusic == null) return;
            _settings = SaveDataManager.Instance.Settings;
            gameMusicSource.volume = _settings.Audio.MusicVolume;
            gameMusicSource.clip = gameMusic;
            gameMusicSource.loop = true;
            gameMusicSource.Play();
        }

        // Update is called once per frame
        void FixedUpdate() {
            PitchAudioOverTime();
        }

        private void PitchAudioOverTime() {
            _pitchTick += 1;
            if (_pitchTick < pitchTickRate) return;
            if (gameMusicSource == null
                || gameMusic == null) return;
            gameMusicSource.pitch = Mathf.Clamp(gameMusicSource.pitch + 0.01f, 1, 2);
            _pitchTick = 0;
        }
    }
}
