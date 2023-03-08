using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXMaster : MonoBehaviour
{
    public static SFXMaster instance;
    public AudioSource audioSource;
    public AudioClip mushPops;
    public AudioClip blockReplace;
    public AudioClip blockDestroy;
    public AudioClip blockPlace;
    public AudioClip menuClick;

    public Toggle muteToggle;
    
    //TODO too many sound effects killing the game. Maybe not an issue? Leaving this for the future

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
        
        muteToggle.isOn = PlayerPrefs.GetInt("SFXMute",1) == 1;
        ToggleSFX();
        audioSource.maxDistance = 10000;
    }

    public void Randomize()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = 0.5f;
    }
    
    public void PlayOneShot(AudioClip clip)
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(clip);
    }
    
    public void PlayMushPop()
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(mushPops);
    }

    public void PlayBlockReplace()
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(blockReplace);
    }

    public void PlayBlockDestroy()
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(blockDestroy);
    }

    public void PlayBlockPlace()
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(blockPlace);
    }

    public void PlayMenuClick()
    {
        if (!muteToggle.isOn)
        {
            return;
        }
        Randomize();
        audioSource.PlayOneShot(menuClick);
    }

    public void ToggleSFX()
    {
        if (muteToggle.isOn)
        {
            audioSource.Stop();
            PlayerPrefs.SetInt("SFXMute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SFXMute", 0);
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}