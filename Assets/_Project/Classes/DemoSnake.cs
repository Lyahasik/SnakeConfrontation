using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Demo snake (GameObject) is used in the Menu & shop scenes to display a preview for the skin that player is currently selected.
/// </summary>

namespace SnakeWarzIO
{
    public class DemoSnake : MonoBehaviour
    {
        [Header("Body Parts")]
        public SpriteRenderer headShape;
        public GameObject headPart;
        public GameObject bodyPart;
        public GameObject bodyPartsHolder;
        public int totalBodyParts;
        public List<GameObject> bodyParts;
        public GameObject lastBodypart;

        [Header("Skin Settings")]
        public int skinID;

        private void Awake()
        {
            totalBodyParts = 0;
            bodyParts = new List<GameObject>();
            lastBodypart = null;

            skinID = PlayerPrefs.GetInt("SelectedSkinID", 0);
        }

        void Start()
        {
            CreateDemoSnake(15);
        }

        /// <summary>
        /// Create the demo snake with the given length (ie, number of bodyparts)
        /// </summary>
        /// <param name="totalBodyparts"></param>
        public void CreateDemoSnake(int totalBodyparts = 10)
        {
            headShape.sprite = SkinManager.instance.GetHeadSkin(skinID);
            AddBodypart(totalBodyparts);
        }

        public void AddBodypart(int amount = 1)
        {
            StartCoroutine(AddBodypartCo(amount));
        }

        public IEnumerator AddBodypartCo(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                AddBodypartMain();

                //yield return new WaitForSeconds(0.03f);
                yield return new WaitForEndOfFrame();
            }
        }

        public void AddBodypartMain()
        {
            //Handle exceptions
            if (!bodyPartsHolder)
                return;

            Vector3 addPos = Vector3.zero;
            if (lastBodypart)
            {
                addPos = lastBodypart.transform.position;
            }
            else
            {
                addPos = transform.position;
            }

            //add demo offset
            addPos += new Vector3(-0.75f, 0, 0);

            GameObject nbp = Instantiate(bodyPart, addPos, Quaternion.Euler(90, 0, 0)) as GameObject;
            bodyParts.Add(nbp);
            nbp.GetComponent<DemoBodypart>().partIndex = totalBodyParts;
            nbp.GetComponent<DemoBodypart>().owner = this.gameObject;

            if (lastBodypart)
                nbp.GetComponent<DemoBodypart>().target = lastBodypart.transform;
            else
                nbp.GetComponent<DemoBodypart>().target = headPart.transform;

            nbp.GetComponent<DemoBodypart>().bodyShape.sprite = SkinManager.instance.GetBodySkin(skinID);

            nbp.name = "Body-" + totalBodyParts;
            nbp.transform.parent = bodyPartsHolder.transform;
            nbp.transform.localPosition = new Vector3(
                nbp.transform.localPosition.x,
                nbp.transform.localPosition.y,
                ((totalBodyParts + 1) * 0.0001f));

            totalBodyParts++;

            //Set last bodypart
            lastBodypart = nbp;
        }

        public void PreviewSkin(int skinID = 0)
        {
            //Apply the selected skin to head and bodyparets
            headShape.sprite = SkinManager.instance.GetHeadSkin(skinID);
            foreach (GameObject go in bodyParts)
            {
                go.GetComponent<DemoBodypart>().bodyShape.sprite = SkinManager.instance.GetBodySkin(skinID);
            }
        }
    }
}