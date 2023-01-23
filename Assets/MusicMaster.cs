using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicMaster : MonoBehaviour
{
    public static MusicMaster instance;
public AudioSource audioSource;
public Toggle muteToggle;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        muteToggle.isOn = PlayerPrefs.GetInt("MusicMute",1) == 1;
    }
    
    public void ToggleMusic()
    {
        if (muteToggle.isOn)
        {
            PlayerPrefs.SetInt("MusicMute", 1);
            audioSource.mute = true;
        }
        else
        {
            PlayerPrefs.SetInt("MusicMute", 0);
            audioSource.mute = false;
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}
