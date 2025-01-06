using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This small class is extra usefull as it ensures that the game will always run from the init scene.
/// </summary>

namespace SnakeWarzIO
{
    public class InitLoader : MonoBehaviour
    {
        void Awake()
        {
            if (!GameObject.FindGameObjectWithTag("MusicPlayer"))
                SceneManager.LoadScene("Init");
        }
    }
}