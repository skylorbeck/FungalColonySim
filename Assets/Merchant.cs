using DG.Tweening;
using TMPro;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public TextMeshProUGUI speech;

    public string[] sayings = new[]
    {
        "Hello, I'm a merchant.",
        "I sell stuff.",
        "Come back anytime.",
        "What do you want?",
        "What're ya buyin'?",
        "What do you need?",
        "What do you want?",
        "What're ya sellin'?",
        "What do you have?",
        "Payment first, secrets later.",
        "Currency speaks, silence denies.",
        "Trade your riches, or yourself.",
        "Seeking treasures, are we?"
    };

    public string[] thankYou = new[]
    {
        "Hehe, thanks.",
        "Thanks for that!",
        "Thanks!",
        "I'll use it to buy more stuff!",
        "Ah, a rare gem indeed.",
        "Such a fine choice...",
        "Who am I to stop you?",
        "Buy a memory, take a dream.",
        "A wise choice, my friend.",
        "I'll take it."
    };

    public string[] noItem = new[]
    {
        "...",
        "...You didn't give me anything...",
        "...I'm not a charity.",
        "...Nothing...?",
        "*grumble*",
        "Some doors should remain closed...",
        "No gold, no goods, my friend.",
        "No coins, no dreams.",
    };

    public string[] noMoney = new[]
    {
        "...",
        "...You don't have enough money...",
        "...I'm not a charity.",
        "...No money...?",
        "*grumble*",
        "A simple trade won't suffice...",
        "I'm afraid your purse is light...",
        "Treasure you seek, coins you lack...",
    };


    void Start()
    {
        GameMaster.instance.ModeMaster.OnModeChange += RandomSaying;
    }

    void FixedUpdate()
    {
        speech.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 2) * 5);
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