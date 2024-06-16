using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QWOPCycle
{
    public class CharacterSFXComponent : MonoBehaviour {
        #region System
        [Header("Audio Sources")]
        public AudioSource balanceAudioSource;
        public AudioSource pedalAudioSource;
        public AudioSource fallDownAudioSource;
        public AudioSource smackAudioSource;
        #endregion
        #region Vars
        [Header("Audio Clips")]
        public AudioClip balanceSound;
        public AudioClip pedalSound;
        public AudioClip fallDownSound;
        public AudioClip smackSound;
        public bool CanPlaySfx { get; set; }
        #endregion
        #region Audio Logic
        public void PlayBalanceSfx() {
            if (!CanPlaySfx) return;
            if (balanceAudioSource != null
                && balanceSound != null) {
                balanceAudioSource.clip = balanceSound;
                balanceAudioSource.Play();
            } else {Debug.LogWarning("Character SFX Component : balance audio source or sound not set.");}
        }
        public void PlayPedalSfx() {
            if (!CanPlaySfx) return;
            if (pedalAudioSource != null && pedalSound != null) {
                pedalAudioSource.clip = pedalSound;
                pedalAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Pedal audio source or sound not set.");}
        }
        public void PlayFallDownSfx() {
            if (!CanPlaySfx) return;
            if (fallDownAudioSource != null
                && fallDownSound != null) {
                fallDownAudioSource.clip = fallDownSound;
                fallDownAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Fall down audio source or sound not set.");}
        }
        public void PlaySmackSfx() {
            if (!CanPlaySfx) return;
            if (smackAudioSource != null
                && smackSound != null) {
                smackAudioSource.clip = smackSound;
                smackAudioSource.Play();
            } else {Debug.LogError("Character SFX Component : Smack audio source or sound not set.");}
        }
        #endregion
    }
}
