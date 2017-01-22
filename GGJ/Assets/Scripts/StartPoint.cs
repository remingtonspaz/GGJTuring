using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    public class StartPoint : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        void OnTriggerEnter()
        {
            GameManager.instance.startCurrentRound();
        }
    }
}
