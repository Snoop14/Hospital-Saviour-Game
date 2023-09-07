﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WithPatient : MonoBehaviour
{

    [SerializeField]
    AudioClip TalkWithPatient;

    AudioSource sound;

    private void OnEnable()
    {
       gameObject.GetComponent<Player>().OnInteractWithPatient += PlayInteractionSound;
    }

    private void OnDisable()
    {
        gameObject.GetComponent<Player>().OnInteractWithPatient -= PlayInteractionSound;
    }

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    void PlayInteractionSound()
    {
        
        sound.PlayOneShot(TalkWithPatient);
    }
}