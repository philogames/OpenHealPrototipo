using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager_OpenHeal : MonoBehaviour
{
    public float maxVolume = 0.3f;
    public Toggle ToggleSound;
    [Header("Music")]
    public float MusicVolume = 1;
    public float MusicFadeTime = 0.5f;
    public AudioClip Music;
    AudioSource audioSource;
    int currentMusicIndex = 0;
    LTDescr musicTween;
    [Header("Sfx")]
    
    public AudioMixer sfxMixer;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
       
        if(PlayerPrefs.HasKey("Sound"))
        {
            ToggleSound.isOn = PlayerPrefs.GetInt("Sound") == 1;
        }
        else
        {
            ToggleSound.isOn = false;
        }
       
    }

    public void ToggleSoundValue(bool v)
    {
        if (v)//se estiver on, desliga
        {
            StopMusic();
            DisableSFX();
        }
        else
        {
            if(audioSource.clip != null)
                PlayMusic();
            EnableSFX();
        }
        

        PlayerPrefs.SetInt("Sound", v ? 1 : 0);
    }


    void EnableSFX()
    {
        sfxMixer.SetFloat("UiVolume", 0);
    }

    void DisableSFX()
    {
        sfxMixer.SetFloat("UiVolume", -80);
    }
    public  void PlayMusic()
    {
        audioSource.clip = Music;
        audioSource.time = 0;
        audioSource.Play();
        musicTween = LeanTween.value(0, maxVolume, MusicFadeTime).setOnUpdate(updateMusicVolume);

    }

    public void StopMusic()
    {
       LeanTween.value(maxVolume, 0, MusicFadeTime).setOnUpdate(updateMusicVolume);
    
    }

    void updateMusicVolume(float v)
    {
        MusicVolume = Mathf.Min(v, maxVolume);
        audioSource.volume = MusicVolume;


    }
}
