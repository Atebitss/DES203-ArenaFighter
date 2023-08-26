using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    //public static AudioManager instance;


    void Awake()
    {

        //if (instance == null)
        //instance = this;
        //else
        // {
        //   Destroy(gameObject);
        //   return;
        //  }


        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void PlayWithPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        s.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        s.source.Play();

    }


    public void PlayOnce(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        if (!s.source.isPlaying) s.source.Play();

    }



    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.PlayOneShot(s.clip);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
    }


    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }


    public void VolumeChange(float inc)
    {
        //add volume change here
    }

    public void VolumeSet(float set)
    {
        //add volume change here
    }


    public void VolumeFadeIn()
    {
        //add volume change here
    }



    public void VolumeFadeOut(string soundName)
    {
        //Debug.Log("Fading " + soundName);
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        //Debug.Log(s.name + " volume: " + s.source.volume);
        float volume = s.source.volume;
        if (volume >= 0.1f) { StartCoroutine(Lower(volume, s)); }
    }

    IEnumerator Lower(float volume, Sound s)
    {
        volume -= 0.1f;
        s.source.volume = volume;
        //Debug.Log(s.name + " volume: " + s.source.volume);
        yield return new WaitForSeconds(0.1f);
        if (volume >= 0.1f) { StartCoroutine(Lower(volume, s)); }
    }
}
