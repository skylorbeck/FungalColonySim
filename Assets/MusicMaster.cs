using UnityEngine;
using UnityEngine.UI;

public class MusicMaster : MonoBehaviour
{
    public static MusicMaster instance;
    public AudioSource audioSource;
    public Slider volumeSlider;

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

        volumeSlider.value = (PlayerPrefs.GetFloat("MusicVolume", .25f));
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}