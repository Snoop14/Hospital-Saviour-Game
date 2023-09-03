using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithPillMachine : MonoBehaviour
{
    [SerializeField]
    AudioClip CollectPill;

    AudioSource sound;

    private void OnEnable()
    {
        gameObject.GetComponent<Player>().OnInteractWithPillMachine += PlayInteractionSound;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Player>().OnInteractWithPillMachine -= PlayInteractionSound;

    }

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    void PlayInteractionSound()
    {
        sound.PlayOneShot(CollectPill);
    }
}
