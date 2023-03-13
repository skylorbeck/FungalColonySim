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
    [SerializeField] private Image image;

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
                text.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[0].ToString("N0");
                break;
            case Currency.RedMushroom:
                text.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[1].ToString("N0");
                break;
            case Currency.BlueMushroom:
                text.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[2].ToString("N0");
                break;
            case Currency.Spore:
                text.text = SaveSystem.instance.GetSaveFile().stats.spores.ToString("N0");
                break;
            case Currency.SkillPoint:
                text.text = SaveSystem.instance.GetSaveFile().stats.skillPoints.ToString("N0");
                break;
            case Currency.BrownPotion:
                text.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0].ToString("N0");
                break;
            case Currency.RedPotion:
                text.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1].ToString("N0");
                break;
            case Currency.BluePotion:
                text.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2].ToString("N0");
                break;
            case Currency.Coin:
                text.text = SaveSystem.instance.GetSaveFile().marketSave.coins.ToString("N0");
                break;
            case Currency.PlinkoBall:
                text.text = SaveSystem.instance.GetSaveFile().plinkoSave.balls.ToString("N0");
                break;
            case Currency.Collectible:
                text.text = SaveSystem.instance.GetSaveFile().collectionItems.Count.ToString("N0");
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
                image.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Brown);
                break;
            case Currency.RedMushroom:
                image.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Red);
                break;
            case Currency.BlueMushroom:
                image.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Blue);
                break;
            case Currency.Spore:
                image.sprite = Resources.Load<Sprite>("Sprites/Spore");
                break;
            case Currency.SkillPoint:
                image.sprite = Resources.Load<Sprite>("Sprites/SkillPoint");
                break;
            case Currency.BrownPotion:
                image.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Brown);
                break;
            case Currency.RedPotion:
                image.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Red);
                break;
            case Currency.BluePotion:
                image.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Blue);
                break;
            case Currency.Coin:
                image.sprite = Resources.Load<Sprite>("Sprites/Coin");
                break;
            case Currency.PlinkoBall:
                image.sprite = Resources.Load<Sprite>("Sprites/PlinkoBall");
                break;
            case Currency.Collectible:
                image.sprite = Resources.Load<Sprite>("Sprites/Collectible");
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
        SkillPoint,
        BrownPotion,
        RedPotion,
        BluePotion,
        Coin,
        PlinkoBall,
        Collectible,
    }
}