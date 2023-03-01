using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CurrencyVisualizer : MonoBehaviour
{
    public Currency currency;
    [SerializeField] private TextMeshProUGUI text;
    [FormerlySerializedAs("spriteRenderer")] [SerializeField] private Image image;

    void Start()
    {
        SetCurrencyAndUpdateVisuals(currency);
    }
    
    
    public void SetCurrencyAndUpdateVisuals(Currency currency)
    {
        SetCurrency(currency);
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        SetCurrency(currency);
    }

    public void SetCurrency(Currency currency)
    {
        this.currency = currency;
        switch (currency)
        {
            case Currency.BrownMushroom:
                text.text = SaveSystem.instance.GetSaveFile().mushrooms[0].ToString();
                break;
            case Currency.RedMushroom:
                text.text = SaveSystem.instance.GetSaveFile().mushrooms[1].ToString();
                break;
            case Currency.BlueMushroom:
                text.text = SaveSystem.instance.GetSaveFile().mushrooms[2].ToString();
                break;
            case Currency.Spore:
                text.text = SaveSystem.instance.GetSaveFile().sporeCount.ToString();
                break;
            case Currency.SkillPoint:
                text.text = SaveSystem.instance.GetSaveFile().hivemindPoints.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
        }
    }

    private void UpdateVisuals()
    {
        switch (currency)
        {
            case Currency.BrownMushroom:
                image.sprite = Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + MushroomBlock.MushroomType.Brown);
                break;
            case Currency.RedMushroom:
                image.sprite = Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + MushroomBlock.MushroomType.Red);
                break;
            case Currency.BlueMushroom:
                image.sprite = Resources.Load<Sprite>("Sprites/Blocks/Mushroom/" + MushroomBlock.MushroomType.Blue);
                break;
            case Currency.Spore:
                image.sprite = Resources.Load<Sprite>("Sprites/Spore");
                break;
            case Currency.SkillPoint:
                image.sprite = Resources.Load<Sprite>("Sprites/SkillPoint");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
        }
    }

    public enum Currency
{
    BrownMushroom,
    RedMushroom,
    BlueMushroom,
    Spore,
    SkillPoint
}
}
