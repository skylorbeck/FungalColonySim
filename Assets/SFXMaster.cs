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
    public AudioClip plinkoHit;
    public List<AudioClip> cauldronPop;

    public Slider volumeSlider;
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

        volumeSlider.value = (PlayerPrefs.GetFloat("SFXVolume", 0.5f));
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
    }

    public void RandomizeMushPop()
    {
        mushroomPopSource.pitch = Random.Range(0.9f, 1.1f);
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (volumeSlider.value.Equals(0)) return;

        Randomize();
        audioSource.PlayOneShot(clip);
    }

    public void PlayMushPop()
    {
        if (!canPop) return;
        if (volumeSlider.value.Equals(0)) return;

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
        PlayOneShot(menuClick);
    }

    public void PlayCauldronPop()
    {
        PlayOneShot(cauldronPop[Random.Range(0, cauldronPop.Count)]);
    }

    public void PlayWood()
    {
        PlayOneShot(wood);
    }

    public void PlayPlinkoHit()
    {
        PlayOneShot(plinkoHit);
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        mushroomPopSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}