using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    void Update()
    {
        music.volume = PlayerPrefs.GetFloat("Music vol");
    }
}
