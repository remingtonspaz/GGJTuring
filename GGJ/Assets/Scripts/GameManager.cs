using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;
        public Transform startPoint;
        public Text startText;
        public Text loseText;
        public Text winText;
        public Text gameText;
        public GameObject[] goodCreaturePrefabs;
        public GameObject[] badCreaturePrefabs;
        public FloorScript floorScript;
        public Transform playerTransform;
        public int[] creatureCountPerRound;

        int round;

        // Use this for initialization
        void Start()
        {
            instance = this;
            startText.enabled = true;
            winText.enabled = false;
            loseText.enabled = false;
            gameText.enabled = false;
            round = -1;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void startGame()
        {
            startPoint.transform.position = playerTransform.position;
            startPoint.GetComponent<Collider>().enabled = false;
            startText.enabled = false;
            winText.enabled = false;
            loseText.enabled = false;
            gameText.enabled = true;
        }

        public void endGame(bool win)
        {
            if (win)
            {
                startText.enabled = false;
                winText.enabled = true;
                loseText.enabled = false;
                gameText.enabled = false;
            }
            else
            {
                startText.enabled = false;
                winText.enabled = false;
                loseText.enabled = true;
                gameText.enabled = false;
            }
        }

        void spawnCreatures()
        {

        }

        public void getGoodCreature()
        {

        }
    }
}
