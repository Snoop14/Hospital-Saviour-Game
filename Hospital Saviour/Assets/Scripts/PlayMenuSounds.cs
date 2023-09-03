using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenuSounds : MonoBehaviour
{
    public AudioClip selectSound;

    AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    public void PlaySelectSound()
    {
        sound.PlayOneShot(selectSound);
    }
}
