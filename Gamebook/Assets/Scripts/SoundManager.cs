using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider soundSlider;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public void SetSoundVolume(float volume)
    {
        bgmSource.volume = volume; 
        
        sfxSource.volume = volume;
    }


}
