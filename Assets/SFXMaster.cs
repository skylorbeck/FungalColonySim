using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXMaster : MonoBehaviour
{
    public static SFXMaster instance;
    public AudioSource audioSource;
    public AudioSource mushroomPopSource;

    public List<AudioClip> mushPops;
    public float timeSinceLastPop = 0;
    public float timeBetweenPops = 0.1f;

    public List<AudioClip> blockDestroy;
    public List<AudioClip> blockPlace;
    public AudioClip menuClick;
    public AudioClip wood;
    public List<AudioClip> cauldronPop;

    public Toggle muteToggle;
    public bool canPop => timeSinceLastPop > timeBetweenPops;


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

        muteToggle.isOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;
        ToggleSFX();
        audioSource.maxDistance = 10000;
        mushroomPopSource.maxDistance = 10000;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (timeSinceLastPop < timeBetweenPops)
        {
            timeSinceLastPop += Time.fixedDeltaTime;
        }
    }

    public void Randomize()
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = 0.5f;
    }

    public void RandomizeMushPop()
    {
        mushroomPopSource.pitch = Random.Range(0.9f, 1.1f);
        mushroomPopSource.volume = 0.5f;
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
        if (!canPop) return;
        if (!muteToggle.isOn) return;

        timeSinceLastPop = 0;
        RandomizeMushPop();
        mushroomPopSource.PlayOneShot(mushPops[Random.Range(0, mushPops.Count)]);
    }


    public void PlayBlockDestroy()
    {
        PlayOneShot(blockDestroy[Random.Range(0, blockDestroy.Count)]);
    }

    public void PlayBlockPlace()
    {
        PlayOneShot(blockPlace[Random.Range(0, blockPlace.Count)]);
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

    public void PlayCauldronPop()
    {
        PlayOneShot(cauldronPop[Random.Range(0, cauldronPop.Count)]);
    }

    public void PlayWood()
    {
        PlayOneShot(wood);
    }

    public void ToggleSFX()
    {
        if (muteToggle.isOn)
        {
            audioSource.Stop();
            mushroomPopSource.Stop();
            PlayerPrefs.SetInt("SFXMute", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SFXMute", 0);
        }
    }
}