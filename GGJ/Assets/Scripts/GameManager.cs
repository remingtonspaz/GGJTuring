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
        public GameObject loseText;
        public GameObject winText;
        public GameObject gameText;
        public CreaturePrefabs[] creaturePrefabs;
        public FloorScript floorScript;
        public Transform playerTransform;
        public int[] creatureCountPerRound;

        int round;
        int getcount;
        List<GameObject> createdCreatures;

        // Use this for initialization
        void Start()
        {
            instance = this;
            winText.SetActive(false);
            loseText.SetActive(false);
            gameText.SetActive(false);
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
            startPoint.gameObject.SetActive(false);
            round = rnd;
            winText.SetActive(false);
            loseText.SetActive(false);
            gameText.SetActive(true);
            getcount = 0;
            spawnCreatures();
        }

        public void endGame(bool win)
        {
            destroyCreatures();
            if (win)
            {
                winText.SetActive(true);
                loseText.SetActive(false);
                gameText.SetActive(false);
                startPoint.gameObject.SetActive(true);
                round++;
            }
            else
            {
                winText.SetActive(false);
                loseText.SetActive(true);
                gameText.SetActive(false);
                startPoint.gameObject.SetActive(true);
            }
        }

        void destroyCreatures()
        {
            if (createdCreatures != null && createdCreatures.Count > 0)
            {
                for (int i = 0; i < createdCreatures.Count; i++)
                {
                    GameObject creature = createdCreatures[i];
                    createdCreatures.Remove(creature);
                    Destroy(creature);
                }
            }
        }

        void spawnCreatures()
        {
            createdCreatures = new List<GameObject>();
            int idx = round - 1;
            float diagonal = Mathf.Sqrt(floorScript.width*floorScript.width + floorScript.length*floorScript.length);
            float chanceMultiplier = diagonal / (float)(creatureCountPerRound[idx] * 2);
            GameObject[] creaturesToSpawn = creaturePrefabs[idx].goodBadCreatures;

            Vector3 lastPlaced = floorScript.tiles[0].transform.position;
            bool goodplaced = false;
            int placed = 0;
            int rowcount = Mathf.RoundToInt(floorScript.width / floorScript.floorTileSize);
            int colcount = Mathf.RoundToInt(floorScript.length / floorScript.floorTileSize);
            gameText.GetComponent<TextMesh>().text += "row count"+rowcount+", "+"column count"+colcount+" \n";
            int row = 0;
            int col = 0;

            foreach (GameObject tile in floorScript.tiles)
            {
                if (row > 4 && row < rowcount - 4 && col > 4 && col < colcount - 4)
                {
                    if (placed >= creatureCountPerRound[idx]) break;
                    float dist = Vector3.Distance(tile.transform.position, lastPlaced);
                    float chance = dist / chanceMultiplier;

                    Random.InitState(Mathf.RoundToInt(tile.transform.position.x * tile.transform.position.z));
                    if (Random.Range(0.0f, 1.0f) < chance)
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
                                }
                                else
                                {
                                    tospawn = creaturesToSpawn[1];
                                }
                            }
                        }
                        else
                        {
                            tospawn = creaturesToSpawn[1];
                        }
                        GameObject spawned = Instantiate(tospawn, 
                            new Vector3(tile.transform.position.x, tile.transform.position.y+.5f, tile.transform.position.z),
                            Quaternion.identity, null) as GameObject;
                        gameText.GetComponent<TextMesh>().text += "spawning creature \n";
                        createdCreatures.Add(spawned);
                        lastPlaced = tile.transform.position;
                        placed++;
                    }

                    
                }

                row++;

                if (row >= rowcount - 1)
                {
                    col++;
                    row = 0;
                }
            }
        }

        public void getGoodCreature()
        {
            endGame(true);
        }
    }
}
