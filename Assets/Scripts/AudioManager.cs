using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{


    public float maxVolume;

    [Header("Music")]
    public float MusicVolume = 1;
    public float MusicFadeTime = 0.5f;
    public AudioClip[] audioClips;
    AudioSource audioSource;
    int currentMusicIndex = 0;
    LTDescr musicTween;

    [Header("SFX")]
    public AudioMixer sfxMixer;
    
   

    // Start is called before the first frame update
    void Start()
    {
        
        audioSource = GetComponent<AudioSource>();
        PlayMenuMusic();

      //  maxVolume = PlayerPrefs.GetFloat("MusicMaxVolume", 1);
    }

    void PlayMenuMusic()
    {
        audioSource.clip = audioClips[0];
        audioSource.time = 0;
        audioSource.Play();
        musicTween = LeanTween.value(0, MusicVolume, MusicFadeTime).setOnUpdate(updateMusicVolume);

    }

    public void PlayRandomMusic()
    {
        StartCoroutine(PlayMusic_Coroutine(audioClips[Random.Range(1, audioClips.Length)], MusicVolume));
    }

    public void PlayMusic(AudioClip m, float vol, bool fromStart = true)
    {
        StartCoroutine(PlayMusic_Coroutine(m, vol, fromStart));
    }

    IEnumerator PlayMusic_Coroutine(AudioClip m, float vol, bool fromStart = true)
    {

       
        audioSource.clip = m;
     
        if (fromStart)
        {
            audioSource.time = 0;
        }
        else
        {
            audioSource.time = Random.Range(0, m.length-10);
        }

        audioSource.Play();

        musicTween = LeanTween.value(0, vol, MusicFadeTime).setOnUpdate(updateMusicVolume);
        yield return new WaitForSeconds(MusicFadeTime);

        yield return new WaitForSeconds(m.length-3);
       // yield return new WaitForSeconds(10);
        float v = vol;

        musicTween = LeanTween.value(v, 0, MusicFadeTime).setOnUpdate(updateMusicVolume);
        yield return new WaitForSeconds(MusicFadeTime);

        currentMusicIndex = Random.Range(1, audioClips.Length);
       

        StartCoroutine(PlayMusic_Coroutine(audioClips[currentMusicIndex], vol));



       

    }

    void updateMusicVolume(float v)
    {
        MusicVolume = Mathf.Min(v, maxVolume);
        audioSource.volume = MusicVolume;

       
    }

    public void UpdateMusicVolume(float v)
    {
        LeanTween.cancel(musicTween.id);
        MusicVolume = Mathf.Min(v, maxVolume);
        audioSource.volume = MusicVolume;

        PlayerPrefs.SetFloat("MusicVolume", v);
    }

    public void SetMusicVolume(float v)
    {
        musicTween = LeanTween.value(MusicVolume, v, MusicFadeTime).setOnUpdate(updateMusicVolume);
        //salvar volume
    }

    public void SetSFXVolume(float v)
    {
        sfxMixer.SetFloat("SfxVolume", Mathf.Log(v) * 20);
        //salvar sfx

        PlayerPrefs.SetFloat("SfxVolume", v);
    }

   
}
