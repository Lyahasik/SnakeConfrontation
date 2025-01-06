using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central point for creating all sort of particles and visual effects.
/// You can list all particles in "availableParticles" array and call to create the required particle from anywhere inside the game.
/// </summary>

namespace SnakeWarzIO
{
    public class FBParticleManager : MonoBehaviour
    {
        public static FBParticleManager instance;

        public GameObject[] availableParticles;

        void Awake()
        {
            instance = this;
        }

        public void CreateParticle(int pID, Vector3 pos, bool makeParent = false, GameObject parent = null, float rotX = 0, float rotY = 0, float rotZ = 0, float scale = 1f)
        {
            int particleID = pID;
            if (pID == -1)
                particleID = Random.Range(0, availableParticles.Length);

            GameObject p = Instantiate(availableParticles[particleID], pos, Quaternion.Euler(rotX, rotY, rotZ)) as GameObject;
            p.name = "Particle";

            p.transform.localScale *= scale;

            if (makeParent)
                p.transform.parent = parent.transform;
        }

    }
}