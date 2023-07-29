using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyVisualizer : MonoBehaviour
{
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

    public Currency currency;
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] private Image image;

    void Start()
    {
        SetCurrencyAndUpdateVisuals(currency);
    }

    private void FixedUpdate()
    {
        SetCurrency(currency);
    }


    public void SetCurrencyAndUpdateVisuals(Currency currency)
    {
        SetCurrency(currency);
        UpdateVisuals();
    }

    public virtual void SetCurrency(Currency currency)
    {
        this.currency = currency;
        switch (currency)
        {
            case Currency.BrownMushroom:
                text.text = SaveSystem.save.stats.mushrooms[0].ToString("N0");
                break;
            case Currency.RedMushroom:
                text.text = SaveSystem.save.stats.mushrooms[1].ToString("N0");
                break;
            case Currency.BlueMushroom:
                text.text = SaveSystem.save.stats.mushrooms[2].ToString("N0");
                break;
            case Currency.Spore:
                text.text = SaveSystem.save.stats.spores.ToString("N0");
                break;
            case Currency.SkillPoint:
                text.text = SaveSystem.save.stats.skillPoints.ToString("N0");
                break;
            case Currency.BrownPotion:
                text.text = SaveSystem.save.marketSave.potionsCount[0].ToString("N0");
                break;
            case Currency.RedPotion:
                text.text = SaveSystem.save.marketSave.potionsCount[1].ToString("N0");
                break;
            case Currency.BluePotion:
                text.text = SaveSystem.save.marketSave.potionsCount[2].ToString("N0");
                break;
            case Currency.Coin:
                text.text = SaveSystem.save.marketSave.coins.ToString("N0");
                break;
            case Currency.PlinkoBall:
                text.text = SaveSystem.save.plinkoSave.balls.ToString("N0");
                break;
            case Currency.Collectible:
                text.text = SaveSystem.save.collectionItems.Count.ToString("N0");
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
}