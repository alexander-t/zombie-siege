using UnityEngine;

namespace General
{
    public class SoundRandomizer : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] sounds;
        [SerializeField] float samplingInterval = 5;
        [SerializeField] float probabilityToPlay = 0.3f;

        private float timeElapsed;

        void Start()
        {
            timeElapsed = 0;
        }


        void Update()
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= samplingInterval)
            {
                if (Random.Range(0f, 1f) >= probabilityToPlay && !audioSource.isPlaying)
                {
                    audioSource.clip = sounds[Random.Range(0, sounds.Length)];
                    audioSource.Play();
                }
                timeElapsed = 0;
            }
        }
    }
}