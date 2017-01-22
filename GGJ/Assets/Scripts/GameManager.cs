using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    [System.Serializable]
    public class CreaturePrefabs
    {
        public GameObject[] goodBadCreatures;
    }

    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;
        public Transform startPoint;
        public Text startText;
        public Text loseText;
        public Text winText;
        public Text gameText;
        public CreaturePrefabs[] creaturePrefabs;
        public FloorScript floorScript;
        public Transform playerTransform;
        public int[] creatureCountPerRound;

        int round;
        int getcount;

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
            startRound(1);
        }

        public void startCurrentRound()
        {
            startRound(round);
        }

        public void startRound(int rnd)
        {
            startPoint.GetComponent<Collider>().enabled = false;
            round = rnd;
            startText.enabled = false;
            winText.enabled = false;
            loseText.enabled = false;
            gameText.enabled = true;
            getcount = 0;
            spawnCreatures();
        }

        public void endGame(bool win)
        {
            if (win)
            {
                startText.enabled = false;
                winText.enabled = true;
                loseText.enabled = false;
                gameText.enabled = false;
                startPoint.GetComponent<Collider>().enabled = true;
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
            int idx = round - 1;
            float diagonal = Mathf.Sqrt(floorScript.width*floorScript.width + floorScript.length*floorScript.length);
            float chanceMultiplier = diagonal / (float)creatureCountPerRound[idx];
            GameObject[] creaturesToSpawn = creaturePrefabs[idx].goodBadCreatures;

            Vector3 lastPlaced = Vector3.zero;
            bool goodplaced = false;
            int placed = 0;

            foreach (GameObject tile in floorScript.tiles)
            {
                if (placed >= creatureCountPerRound[idx]) break;
                float dist = Vector3.Distance(tile.transform.position, lastPlaced);
                float chance = dist / chanceMultiplier;

                Random.InitState(Mathf.RoundToInt(tile.transform.position.x * tile.transform.position.z));
                if (Random.Range(0.0f,1.0f) < chance)
                {
                    GameObject tospawn;
                    if (!goodplaced)
                    {
                        if (placed == creatureCountPerRound[idx] - 1)
                        {
                            tospawn = creaturesToSpawn[0];
                            goodplaced = true;
                        }
                        else
                        {
                            if (Random.Range(0, 100) < 50)
                            {
                                tospawn = creaturesToSpawn[0];
                                goodplaced = true;
                            }else
                            {
                                tospawn = creaturesToSpawn[1];
                            }
                        }
                    }else
                    {
                        tospawn = creaturesToSpawn[1];
                    }
                    Instantiate(tospawn, new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z),
                        Quaternion.identity, null);
                    placed++;
                }
            }
        }

        public void getGoodCreature()
        {
            endGame(true);
        }
    }
}
