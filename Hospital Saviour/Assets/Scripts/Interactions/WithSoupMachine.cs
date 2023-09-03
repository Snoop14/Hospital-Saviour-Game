using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithSoupMachine : MonoBehaviour
{
    [SerializeField]
    AudioClip CollectSoup;

    AudioSource sound;

    private void OnEnable()
    {
        gameObject.GetComponent<Player>().OnInteractWithSoupMachine += PlayInteractionSound;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Player>().OnInteractWithSoupMachine -= PlayInteractionSound;

    }

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    void PlayInteractionSound()
    {
        sound.PlayOneShot(CollectSoup);
    }
}
