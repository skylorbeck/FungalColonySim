using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicMaster : MonoBehaviour
{
    public static MusicMaster instance;
    public AudioSource audioSource;
    public Slider volumeSlider;

    public List<AudioClip> musicClips = new List<AudioClip>();

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

        float volume = PlayerPrefs.GetFloat("MusicVolume", .25f);
        try
        {
            volumeSlider.SetValueWithoutNotify(volume);
        }
        catch
        {
            // ignored
        }

        SetVolume(volume);
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!audioSource.isPlaying)
        {
            PlaySong();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void PlaySong()
    {
        audioSource.clip = musicClips[UnityEngine.Random.Range(0, musicClips.Count)];
        audioSource.Play();
    }
}