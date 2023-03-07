using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public TextMeshProUGUI speech;

    public string[] sayings = new[]
    {
        "Hello, I'm a merchant.",
        "I sell stuff.",
        "I'm not very good at this.",
        "Come back anytime.",
        "What do you want?",
        "What're ya buyin'?",
        "What do you need?",
        "What do you want?",
        "What're ya sellin'?",
        "What do you have?",
    };
    public string[] thankYou = new[]
    {
        "Hehe, thanks.",
        "Thanks for that!",
        "Thanks!",
        "I'll use it to buy more stuff!",
    };
    public int currentSaying = 0;
    
    void Start()
    {
        RandomSaying();
    }

    public void NextSaying()
    {
        currentSaying++;
        if (currentSaying >= sayings.Length)
        {
            currentSaying = 0;
        }
        speech.text = sayings[currentSaying];
    }
    
    public void RandomSaying()
    {
        currentSaying = Random.Range(0, sayings.Length);
        speech.text = sayings[currentSaying];
    }
    
    public void RandomThankYou()
    {
        speech.text = thankYou[Random.Range(0, thankYou.Length)];
    }
    
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        speech.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time*2) * 5);
    }
}
