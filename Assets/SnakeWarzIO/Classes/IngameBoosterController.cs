using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class belongs to booster prefabs that we scatter inside the game scene at random positions,
/// to give players an incetive to search through the scenes and to find these boosters. 
/// 
/// Ingame boosters will be consumed instantly upon picking up.
/// </summary>

namespace SnakeWarzIO
{
    public class IngameBoosterController : MonoBehaviour
    {
        //When a booster is picked up, it will go disable temporarily for short period of time. This is to ensure player won't bump into two instances of the same booster in a short span of time.
        public const float repositionCooldown = 5f;

        //Type of this booster item
        public BoostersController.BoosterTypes boosterID = BoostersController.BoosterTypes.Unzoom;

        //This ID needs to match the vfxID on "VfxSpawner->FBParticleManager->availableParticles" that you want to display after this booster is picked up.
        public int vfxID = -1;  

        [Header("Components")]
        public GameObject bodyParent;
        public GameObject minimapIcon;
        private SphereCollider boosterCollider;

        private void Awake()
        {
            boosterCollider = GetComponent<SphereCollider>();
        }

        void Start()
        {
            ApplyReposition();
        }

        void Update()
        {

        }

        public void ApplyReposition()
        {
            StartCoroutine(ApplyRepositionCo());
        }

        /// <summary>
        /// When the game is just started, or if this booster is picked up, we need to change the position of this ingame booster item to a new random one.
        /// Once the repositioning is being done, we temporarily disable to booster object as well.
        /// </summary>
        /// <returns></returns>
        public IEnumerator ApplyRepositionCo()
        {
            //Temporarily make this booster unusable after respositioning
            bodyParent.SetActive(false);
            minimapIcon.SetActive(false);
            boosterCollider.enabled = false;

            //Reposition
            transform.position = GameController.instance.GetRandomPositionInMap();

            //Wait
            yield return new WaitForSeconds(repositionCooldown);

            //Then make it usable again
            bodyParent.SetActive(true);
            minimapIcon.SetActive(true);
            boosterCollider.enabled = true;            
        }

    }
}