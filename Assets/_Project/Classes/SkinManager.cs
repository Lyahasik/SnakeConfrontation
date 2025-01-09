using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wherever we want to change an skin, we need to have an instance of this class present.
/// This class holds a reference to all available snake heads & bodyparts and also to all available game backgrounds. So this can be used as a central point to add/edit/remove skins from the game.
/// </summary>

namespace SnakeWarzIO
{
    public class SkinManager : MonoBehaviour
    {
        public static SkinManager instance;
        public static int totalAvailableSkins;

        [Header("Skins")]
        public Sprite[] availableHeadSkins;
        public Sprite[] availableBodySkins;

        [Header("Game Background")]
        public Renderer gameBG;                     //Must be set while inside the "Game" scene. Otherwise, it's not needed.
        public Material[] availableBackgrounds;


        private void Awake()
        {
            instance = this;
            totalAvailableSkins = availableBodySkins.Length;
        }

        void Start()
        {
            //If we like to use a randomized pit background on every play
            //RandomizeMainBG();

            //If we like to use the pit background player selected via settings panel
            UpdatePitBackground();
        }

        /// <summary>
        /// Assing the main pit background based on player selection in settings panel
        /// </summary>
        public void UpdatePitBackground()
        {
            //Only needed for the game scene
            if (gameBG)
                gameBG.material = availableBackgrounds[PlayerPrefs.GetInt("SelectedPitSkinID", 0)];
        }

        /// <summary>
        /// Selects and assigns a random background for the game scene
        /// </summary>
        public void RandomizeMainBG()
        {
            int rndBgID = Random.Range(0, availableBackgrounds.Length);

            //Only needed for the game scene
            if (gameBG)
                gameBG.material = availableBackgrounds[rndBgID];
        }

        /// <summary>
        /// Get the indicated snake head sprite based on the given ID
        /// </summary>
        /// <param name="skinID"></param>
        /// <returns></returns>
        public Sprite GetHeadSkin(int skinID = -1)
        {
            if (skinID != -1)
                return availableHeadSkins[skinID];
            else
                return availableHeadSkins[Random.Range(0, availableHeadSkins.Length)];
        }

        /// <summary>
        /// Get the indicated snake bodypart sprite based on the given ID
        /// </summary>
        public Sprite GetBodySkin(int skinID = -1)
        {
            if (skinID != -1)
                return availableBodySkins[skinID];
            else
                return availableBodySkins[Random.Range(0, availableHeadSkins.Length)];
        }
    }
}