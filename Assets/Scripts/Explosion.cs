using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AudioClip soundExplosion;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(soundExplosion);
        Destroy(gameObject, 1.1f);
    }
}
