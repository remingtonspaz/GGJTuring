using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoloToolkit.Unity.SpatialMapping.Tests
{

    public class FloorScript : MonoBehaviour
    {

        public float floorTileSize = 1.0f;
        public GameObject tilePrefab;
        public Material largestFloorMaterial;
        List<GameObject> floors;

        // Use this for initialization
        void Start()
        {
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete += SurfaceMeshesToPlanes_MakePlanesComplete;
        }

        private void SurfaceMeshesToPlanes_MakePlanesComplete(object source, System.EventArgs args)
        {
            // Collection of floor planes that we can use to set horizontal items on.
            floors = new List<GameObject>();
            floors = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Floor);

            createTiles();
        }

        GameObject getLargestFloor()
        {
            GameObject largestFloor = null;
            float largestVolume = float.MinValue;

            foreach (GameObject floor in floors)
            {
                MeshRenderer floorRenderer = floor.GetComponent<MeshRenderer>();
                float volume = floorRenderer.bounds.size.x * floorRenderer.bounds.size.z * floorRenderer.bounds.size.y;
                if (volume > largestVolume)
                {
                    largestFloor = floor;
                    largestVolume = volume;
                }
            }

            largestFloor.GetComponent<MeshRenderer>().material = largestFloorMaterial;
            return largestFloor;
        }

        void createTiles()
        {
            GameObject largestFloor = getLargestFloor();
            MeshRenderer floorRenderer = largestFloor.GetComponent<MeshRenderer>();
            float width = floorRenderer.bounds.size.x;
            float length = floorRenderer.bounds.size.z;
            float startX = largestFloor.transform.position.x - width / 2;
            float startZ = largestFloor.transform.position.z - length / 2;

            bool white = true;

            for (float x = startX; x <= startX+width; x += floorTileSize)
            {
                bool startwhite = white;
                for (float z = startZ; z <= startZ+length; z += floorTileSize)
                {
                    GameObject tileObject = Instantiate(tilePrefab, new Vector3(x - floorTileSize / 2, 
                        largestFloor.transform.position.y, z - floorTileSize / 2),
                        Quaternion.identity, null) as GameObject;
                    tileObject.transform.localScale = new Vector3(floorTileSize, .1f, floorTileSize);
                    tileObject.GetComponent<MeshRenderer>().material.color = (white) ? Color.white : Color.black;
                    white = !white;
                }
                white = !startwhite;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
