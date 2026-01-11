using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    [SerializeField] AudioClip Intro;
    [SerializeField] AudioClip Loop;
    [SerializeField] AudioSource Source;

    void Start(){
        Source.clip = Intro;
        Source.loop = false;
        Source.Play();
    }

    void Update(){
        if(!Source.isPlaying){
            Source.clip = Loop;
            Source.loop = true;
            Source.Play();
            Destroy(this);
        }
    }
}
