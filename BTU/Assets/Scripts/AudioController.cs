using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip lurci, lacrimosa;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if(!audioSource.isPlaying && GameObject.Find("CanvasContainer_Final").GetComponent<CanvasContainer>().isComplete)
        {
            audioSource.clip = lacrimosa;
            audioSource.Play();
        }
        else if(!audioSource.isPlaying)
        {
            audioSource.clip = lurci;
            audioSource.Play();
        }
    }
}
