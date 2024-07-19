// Developed by Pluto
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sora.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private SerializedDictionary<string, AudioClip> audioClips = new SerializedDictionary<string, AudioClip>();
        private AudioSource audioSource;

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            PlayMusic();            
        }

        private void PlayMusic()
        {
            if (audioClips.ContainsKey("theme"))
            {
                audioSource.clip = audioClips["theme"];
                audioSource.Play();
            }
            else
                Debug.Log("AudioManager:: Theme song not added. Make sure the clip is added or the clip's name matches the following: \"theme\".");
        }

        public void PlaySFX(string clipName)
        {
            if (audioClips.ContainsKey(clipName))
                audioSource.PlayOneShot(audioClips[clipName]);
            else
                Debug.Log("AudioManager:: Audio Clip name mismatch. The clip name you provided does not exist.");
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
    }
}