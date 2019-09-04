using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XWingAnimation : MonoBehaviour {

    private Animator anim;

    private AudioSource audio;

    bool sFoilsOpen = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (sFoilsOpen)
            {
                anim.SetTrigger("close");
                sFoilsOpen = false;
            }
            else
            {
                anim.SetTrigger("open");
                sFoilsOpen = true;
            }

            PlayAudio();
        }
    }

    public void PlayAudio()
    {
        audio.Play();
    }
}
