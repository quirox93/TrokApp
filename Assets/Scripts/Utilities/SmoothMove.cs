 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;
 using UnityEngine.SceneManagement;
 
 namespace WARBEN
 {
    public class SmoothMove : MonoBehaviour 
    {
        public float speed = 5f;
        public float duration = 0.5f;
        public AudioSource audioSource;
        public float volume=1f;
        //public AudioClip audioStart;
        public AudioClip audioDraw;
        public AudioClip audioMove;
        public AudioClip audioDeck;
        //public bool playStart = true;
        //public bool playEnd = true;
        public Transform destination = null;
        

        void Awake()
        {
            if(destination ==  null)
                destination = this.transform;

            audioSource = this.GetComponent<AudioSource>();
            
        }

        public void SetDestination (Transform _destinaton) {
            destination = _destinaton;
            StartCoroutine(MoveObjectToPosition(transform, destination, duration));
        }


        IEnumerator MoveObjectToPosition(Transform trans, Transform parent, float duration)
        {
            Vector3 start_position = trans.position;
            float elapsed = 0.0f;
            if(destination.name.Contains("Hand"))
            {
                audioSource.PlayOneShot(audioDraw);
            }
            if(destination.name.Contains("Deck") | destination.name.Contains("Sepultura"))
            {
                audioSource.PlayOneShot(audioDeck);
            }
            while (elapsed < duration)
            {
                trans.position = Vector3.Lerp(start_position, parent.position, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            trans.position = parent.position;
            trans.parent = parent;
            trans.localScale = new Vector3(1f,1,1);
            trans.localRotation = Quaternion.identity;
            if(destination.name.Contains("zone"))
                {
                    trans.localScale = new Vector3(0.5f,0.5f,1);
                    audioSource.PlayOneShot(audioMove);
                }
            else if(destination.name.Contains("Sepultura"))
                {
                    trans.localScale = new Vector3(1.5f,1.5f,1);
                    audioSource.PlayOneShot(audioMove);
                }
        }
    }
 }
