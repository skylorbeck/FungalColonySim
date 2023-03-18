using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

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

    public string[] noItem = new[]
    {
        "...",
        "...You didn't give me anything...",
        "...I'm not a charity.",
        "...Nothing...?",
        "*grumble*",
    };

    public string[] noMoney = new[]
    {
        "...",
        "...You don't have enough money...",
        "...I'm not a charity.",
        "...No money...?",
        "*grumble*",
    };


    void Start()
    {
        GameMaster.instance.ModeMaster.OnModeChange += RandomSaying;
    }

 
    public void RandomSaying()
    {
        UpdateText(sayings[Random.Range(0, sayings.Length)]);
    }

    public void RandomThankYou()
    {
        UpdateText(thankYou[Random.Range(0, thankYou.Length)]);
    }
    
    public void RandomNoItem()
    {
        UpdateText(noItem[Random.Range(0, noItem.Length)]);
    }

    void FixedUpdate()
    {
        speech.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 2) * 5);
    }

    public void RandomNoMoney()
    {
        UpdateText(noMoney[Random.Range(0, noMoney.Length)]);
    }

    public void UpdateText(string text)
    {
        speech.text = text;
        this.transform.DOComplete();
        this.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 1, 0.5f);
    }
}