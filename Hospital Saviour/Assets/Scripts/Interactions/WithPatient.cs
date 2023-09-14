using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithPatient : MonoBehaviour
{

    //sound for swhen interacting with patient
    [SerializeField]
    AudioClip TalkWithPatient;

    //The audiosource that plays the sound
    AudioSource sound;

    private void OnEnable()
    {
        //add the event to the player
        gameObject.GetComponent<Player>().OnInteractWithPatient += PlayInteractionSound;
    }

    private void OnDisable()
    {
        //remove event from player
        gameObject.GetComponent<Player>().OnInteractWithPatient -= PlayInteractionSound;
    }

    // Start is called before the first frame update
    void Start()
    {
        //find the audiosource in the scene
        sound = GetComponent<AudioSource>();
    }

    void PlayInteractionSound()
    {
        //play the sound once when the function is triggered
        sound.PlayOneShot(TalkWithPatient);
    }
}
