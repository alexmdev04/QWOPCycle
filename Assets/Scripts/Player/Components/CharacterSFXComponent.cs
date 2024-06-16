using System;
using System.Collections;
using System.Collections.Generic;
using QWOPCycle.Player;
using QWOPCycle.Persistence;
using SideFX.Events;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QWOPCycle
{
    public class CharacterSFXComponent : MonoBehaviour {
        #region System
        private GameSettings _settings;
        [Header("Audio Sources")]
        public AudioSource balanceAudioSource;
        public AudioSource pedalAudioSource;
        public AudioSource fallDownAudioSource;
        public AudioSource smackAudioSource;
        #endregion
        #region Vars
        [Header("Audio Clips")]
        public AudioClip[] balanceSound;
        public AudioClip[] pedalSound;
        public AudioClip[] fallDownSound;
        public AudioClip[] smackSound;
        public bool CanPlaySfx { get; set; }
        #endregion
        #region Event Bus
        private EventBinding<PlayerFellOver> _playerFellOverBinding;
        #endregion
#region Intialisation
        private void OnEnable() {
            BindEventsToAudio();
        }
        private void OnDisable() {
            UnBindEventsToAudio();
        }
        private void BindEventsToAudio() {
            _playerFellOverBinding = new EventBinding<PlayerFellOver>(PlayFallDownSfx);
            EventBus<PlayerFellOver>.Register(_playerFellOverBinding);
        }
        private void UnBindEventsToAudio() {
            EventBus<PlayerFellOver>.Deregister(_playerFellOverBinding);
        }
#endregion
        #region Audio Logic
        public void PlayBalanceSfx() {
            if (!CanPlaySfx) return;
            if (balanceAudioSource != null
                && balanceSound != null) {
                CheckVolume(balanceAudioSource);
                balanceAudioSource.clip = ChooseRandomClip(balanceSound);
                balanceAudioSource.Play();
            } else {Debug.LogWarning("Character SFX Component : balance audio source or sound not set.");}
        }
        public void PlayPedalSfx() {
            if (!CanPlaySfx) return;
            if (pedalAudioSource.isPlaying) return;
            if (pedalAudioSource != null && pedalSound != null) {
                CheckVolume(pedalAudioSource);
                pedalAudioSource.clip = ChooseRandomClip(pedalSound);
                pedalAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Pedal audio source or sound not set.");}
        }
        private void PlayFallDownSfx() {
            if (!CanPlaySfx) return;
            if (fallDownAudioSource != null
                && fallDownSound != null) {
                CheckVolume(fallDownAudioSource);
                fallDownAudioSource.clip = ChooseRandomClip(fallDownSound);
                fallDownAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Fall down audio source or sound not set.");}

            CanPlaySfx = false;
        }
        public void PlaySmackSfx() {
            if (!CanPlaySfx) return;
            if (smackAudioSource != null
                && smackSound != null) {
                CheckVolume(smackAudioSource);
                smackAudioSource.clip = ChooseRandomClip(smackSound);
                smackAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Smack audio source or sound not set.");}
        }
        private AudioClip ChooseRandomClip(AudioClip[] clipArray) {
            if (clipArray == null
                || clipArray.Length == 0) {
                Debug.LogError($"Character SFX Component : The clip array {clipArray} is null or empty.");
                return null;
            }
            int randomIndex = Random.Range(0, clipArray.Length);
            return clipArray[randomIndex];
        }
        private void CheckVolume(AudioSource source) {
            _settings = SaveDataManager.Instance.Settings;
            source.volume = _settings.Audio.sfxVolume;
        }
        #endregion
    }
}
