using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenuSounds : MonoBehaviour
{
    //holder for the sound
    [SerializeField]
    AudioClip selectSound;

    //holder for the audio source
    AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        //find the audiosource
        sound = GetComponent<AudioSource>();
    }

    public void PlaySelectSound()
    {
        //play the sound
        sound.PlayOneShot(selectSound);
    }
}
