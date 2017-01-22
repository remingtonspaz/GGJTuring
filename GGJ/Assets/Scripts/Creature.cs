using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.SpatialMapping.Tests
{
    public class Creature : MonoBehaviour,IInputClickHandler {

        public bool bad;
        public AudioClip goodClip;
        public AudioClip badClip;
        bool lookbad;

        AudioSource audioSource;
        MeshRenderer meshRenderer;

        // Use this for initialization
        void Start() {
            audioSource = GetComponent<AudioSource>();
            meshRenderer = GetComponent<MeshRenderer>();
            lookbad = false;
            if (bad)
            {
                if (Random.Range(0,100) < 50)
                {
                    //make sound bad
                    audioSource.clip = badClip;
                }else
                {
                    //make image bad
                    lookbad = true;
                }
            }
        }

        // Update is called once per frame
        void Update() {
            if (lookbad)
            {
                meshRenderer.material.SetColor("Emissive", Color.green * Mathf.Sin(Time.time));
            }
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
                    GameManager.instance.endGame(false);
                }else
                {
                    GameManager.instance.endGame(true);
                }
            }
        }
    }
}
