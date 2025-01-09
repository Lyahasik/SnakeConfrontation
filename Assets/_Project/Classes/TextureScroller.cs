using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeWarzIO
{
    public class TextureScroller : MonoBehaviour
    {
        private Renderer r;

        void Awake()
        {
            r = GetComponent<Renderer>();
        }

        void Update()
        {
            r.material.SetTextureOffset("_MainTex", new Vector2(Time.time / -15f, Time.time / -25f));
        }
    }
}