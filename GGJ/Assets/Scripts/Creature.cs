using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class Creature : MonoBehaviour,IInputClickHandler {

        public bool bad;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "MainCamera")
            {
                if (bad)
                {

                }
            }
        }
    }
}
