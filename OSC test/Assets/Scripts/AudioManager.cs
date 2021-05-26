using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;     //sets the sound clip

            s.source.volume = s.volume;    //sets volume, pitch, loop
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.playOnStartup) s.source.Play();  //plays sound if the sound is to be played on startup
        }
    }
    public void playSound(string key)  //plays sound with key
    {
        Sound s = Array.Find(sounds, sound => sound.sName == key);
        if (!s.source.isPlaying) s.source.Play();    //only plays if sound isnt already playing, stops jittering audio
    }

    public void playSoundOneShot(string key)  //plays sound with key
    {
        Sound s = Array.Find(sounds, sound => sound.sName == key);
        if (!s.source.isPlaying) s.source.PlayOneShot(s.source.clip);    //only plays if sound isnt already playing, stops jittering audio
    }

    public void stopSound(string key)  //stops the sound with key
    {
        Sound s = Array.Find(sounds, sound => sound.sName == key);
        s.source.Stop();
    }

    public void pauseSound(string key)
    {
        Sound s = Array.Find(sounds, sound => sound.sName == key);
        s.source.Pause();
    }
}
