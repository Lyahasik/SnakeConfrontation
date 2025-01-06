using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MusicPlayer is responsible for playing seamless menu & ingame musics in a looped state. 
/// It also helps with keeping track of global sound & music states all throughout the game.
/// </summary>

namespace SnakeWarzIO
{
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer instance;
        public AudioClip main;
        public AudioClip loop;

        public static bool globalSoundState;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == "Game")
            {
                if (GetComponent<AudioSource>().clip == loop)
                    return;

                PlayBgm(loop);
            } 
            else
            {
                if (GetComponent<AudioSource>().clip == main)
                    return;

                PlayBgm(main);
            }
        }

        void Awake()
        {
            DontDestroyOnLoad(this);
            instance = this;

            //Enable audio & music in the first run
            if (!PlayerPrefs.HasKey("Inited"))
            {
                //print("Game is Inited!");
                PlayerPrefs.SetInt("Inited", 1);
                PlayerPrefs.SetInt("SoundState", 1);
                PlayerPrefs.SetInt("MusicState", 1);
            }

            //Check saved status below

            //Sound
            if (PlayerPrefs.GetInt("SoundState") == 1)
                globalSoundState = true;
            else
                globalSoundState = false;

            //music
            if (PlayerPrefs.GetInt("MusicState") == 0)
                ToggleMusic();
        }

        void Start()
        {
            PlayBgm(main);
        }

        void Update()
        {
            if (!GetComponent<AudioSource>().isPlaying)
            {
                PlayBgm(loop);
            }
        }

        void PlayBgm(AudioClip _clip)
        {
            GetComponent<AudioSource>().clip = _clip;
            GetComponent<AudioSource>().Play();
        }

        public void ToggleMusic()
        {
            GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
        }

        public void ToggleSound()
        {
            globalSoundState = !globalSoundState;
        }
    }
}