using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolSlider : MonoBehaviour
{
    private Slider musicSlider;

    void Start()
    {
        musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();

        PlayerPrefs.SetFloat("Music vol", musicSlider.value);
        musicSlider.onValueChanged.AddListener(delegate { musicValueUpdate(); });
    }

    public void musicValueUpdate()
    {
        PlayerPrefs.SetFloat("Music vol", musicSlider.value);
    }
}
