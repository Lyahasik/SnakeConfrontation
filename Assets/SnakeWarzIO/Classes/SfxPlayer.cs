using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main class for playing everything sfx related. We load all available audio files in this class and other classes can call "PlaySfx" to play the requested audio clip.
/// </summary>

namespace SnakeWarzIO
{
    public class SfxPlayer : MonoBehaviour
    {
        public static SfxPlayer instance;
        public AudioClip[] availableSfx;
        public AudioClip[] availableAnnounce;

        [Header("Sources")]
        public AudioSource oneshotAso;
        public AudioSource loopedAso;

        private void Awake()
        {
            instance = this;
        }
        
        /// <summary>
        /// Play the given audio clip with the indicated volume
        /// </summary>
        /// <param name="sfxID"></param>
        /// <param name="vol"></param>
        public void PlaySfx(int sfxID = -1, float vol = 1f)
        {
            int clipID = sfxID;
            if (clipID == -1)
                clipID = Random.Range(0, availableSfx.Length);

            if (MusicPlayer.globalSoundState)
                oneshotAso.PlayOneShot(availableSfx[clipID], vol);
        }

        /// <summary>
        /// Play the given announcement/encouragement audio
        /// </summary>
        /// <param name="sfxID"></param>
        /// <param name="vol"></param>
        public void PlayAnnounce(int sfxID = -1, float vol = 0.6f)
        {
            int clipID = sfxID;
            if (clipID == -1)
                clipID = Random.Range(0, availableAnnounce.Length);

            if (MusicPlayer.globalSoundState)
                oneshotAso.PlayOneShot(availableAnnounce[clipID], vol);
        }

        /// <summary>
        /// Play the given audio clip in a looped state. Please note that you need to stop the looped audios using "StopLoopedSfx" method.
        /// </summary>
        /// <param name="sfxID"></param>
        public void PlaySfxLooped(int sfxID)
        {
            loopedAso.clip = availableSfx[sfxID];
            loopedAso.loop = true;
            loopedAso.volume = 0.5f;

            if (MusicPlayer.globalSoundState)
            {
                if (!loopedAso.isPlaying)
                    loopedAso.Play();
            }
        }

        /// <summary>
        /// Stop the given looped audio clip.
        /// </summary>
        /// <param name="sfxID"></param>
        public void StopLoopedSfx(int sfxID)
        {
            loopedAso.Stop();
        }

    }
}