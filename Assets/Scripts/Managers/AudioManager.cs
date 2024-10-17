// Developed by Sora
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAudioType
{
    JUMP,
    THROW,
    HOP,
    RIP,
    MERGE,
    SPLIT,
}

namespace Sora.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private SerializedDictionary<string, AudioClip> bgMusic = new SerializedDictionary<string, AudioClip>();
        [SerializeField] private SerializedDictionary<EAudioType, AudioClip>sfx = new SerializedDictionary<EAudioType, AudioClip>();

        private AudioSource audioSource;

        private void OnEnable()
        {

        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            audioSource.volume = PlayerPrefs.GetFloat("music", 0.2f);
            PlayMusic();            
        }

        private void PlayMusic()
        {
            if (bgMusic.ContainsKey("theme"))
            {
                audioSource.clip = bgMusic["theme"];
                audioSource.Play();
            }
            else
                Debug.Log("AudioManager:: Theme song not added. Make sure the clip is added or the clip's name matches the following: \"theme\".");
        }

        public void PlaySFX(EAudioType type)
        {
            audioSource.PlayOneShot(sfx[type]);
        }

        public void PauseBackgroundMusic()
        {
            audioSource.Pause();
        }

        public void ResumeBackgrounMusic()
        {
            audioSource.Play();
        }

        public void SetBackgroundVolume(float volume)
        {
            audioSource.volume = volume;
        }

        public void MuteMusic()
        {
            audioSource.volume = 0.0f;
            PlayerPrefs.SetFloat("music", 0.0f);
        }
    }
}