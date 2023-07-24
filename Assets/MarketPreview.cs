using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MarketPreview : MonoBehaviour
{
    public enum Mode
    {
        Buy,
        Sell
    }

    public SpriteRenderer itemSprite;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer rockSprite;
    public TextMeshPro itemName;
    public TextMeshProUGUI itemPrice;
    public Button buySellButton;

    public Mode mode = Mode.Sell;
    public TMP_InputField inputField;
    public TextMeshProUGUI goldChangedText;

    public float speed = 1f;
    public float timeOffset = 0f;
    public float maxAngle = 15f;
    public float maxHeight = .1f;
    public float shadowSize = 1f;

    public uint price => mode == Mode.Sell
        ? SaveSystem.instance.GetSaveFile().marketSave.sellPrice
        : SaveSystem.instance.GetSaveFile().marketSave.buyPrice;

    public CurrencyVisualizer.Currency currency => mode == Mode.Sell
        ? SaveSystem.instance.GetSaveFile().marketSave.sellItem
        : SaveSystem.instance.GetSaveFile().marketSave.buyItem;

    public static CollectionItemSaveData item
    {
        get => SaveSystem.instance.GetSaveFile().marketSave.sellItemData;
        set => SaveSystem.instance.GetSaveFile().marketSave.sellItemData = value;
    }

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveSystem.instance != null);
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        SetCurrency(currency);
        SetPrice(price);
        // UpdatePrice();
    }

    public void FixedUpdate()
    {
        Transform itemSpriteTransform = itemSprite.transform;
        itemSpriteTransform.rotation =
            Quaternion.Euler(0, 0, Mathf.Sin(timeOffset + Time.fixedTime * speed) * maxAngle);
        float y = Mathf.Cos(timeOffset + Time.fixedTime * speed) * maxHeight;
        itemSpriteTransform.localPosition = new Vector3(0, y, itemSpriteTransform.localPosition.z);
        shadowSprite.transform.localScale = new Vector3(shadowSize - y, shadowSize * 0.5f - y * 0.5f, shadowSize - y);
        itemName.transform.rotation = itemSpriteTransform.rotation;
        // itemPrice.transform.rotation = itemSpriteTransform.rotation;
    }

    public void SetCurrency(CurrencyVisualizer.Currency newCurrency)
    {
        switch (newCurrency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Brown);
                itemName.text = "Brown Mushroom";
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Red);
                itemName.text = "Red Mushroom";
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Blue);
                itemName.text = "Blue Mushroom";
                break;
            case CurrencyVisualizer.Currency.Spore:
                itemSprite.sprite = Resources.Load<Sprite>("Sprites/Spore");
                itemName.text = "Spore";
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                itemSprite.sprite = Resources.Load<Sprite>("Sprites/SkillPoint");
                itemName.text = "Skill Point";
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Brown);
                itemName.text = "Brown Potion";
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Red);
                itemName.text = "Red Potion";
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Blue);
                itemName.text = "Blue Potion";
                break;
            case CurrencyVisualizer.Currency.Collectible:
                item = SaveSystem.instance.GetSaveFile()
                    .collectionItems[Random.Range(0, SaveSystem.instance.GetSaveFile().collectionItems.Count)];
                itemSprite.sprite = Resources.Load<Sprite>("Sprites/Collection/" + item.spriteName);
                itemName.text = item.name;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newCurrency), newCurrency, null);
        }

        switch (mode)
        {
            case Mode.Buy:
                SaveSystem.instance.GetSaveFile().marketSave.buyItem = newCurrency;
                break;
            case Mode.Sell:
                SaveSystem.instance.GetSaveFile().marketSave.sellItem = newCurrency;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdatePrice()
    {
        uint price = 99999;
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                price = (uint)Random.Range(1, 2);
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                price = (uint)Random.Range(2, 5);
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                price = (uint)Random.Range(3, 10);
                break;
            case CurrencyVisualizer.Currency.Spore:
                price = (uint)Random.Range(SaveSystem.instance.GetSaveFile().statsTotal.skillPoints * 50,
                    SaveSystem.instance.GetSaveFile().statsTotal.skillPoints * 100);
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                price = (uint)Random.Range(SaveSystem.instance.GetSaveFile().statsTotal.skillPoints * 500,
                    SaveSystem.instance.GetSaveFile().statsTotal.skillPoints * 1000);
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                price = (uint)Random.Range(100, 300);
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                price = (uint)Random.Range(200, 750);
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                price = (uint)Random.Range(300, 1500);
                break;
            case CurrencyVisualizer.Currency.Collectible:
                price = (uint)Random.Range(100, 5000);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        SetPrice(price);
        CheckButton();
        ValidateAmount();
    }

    public void SetPrice(uint price)
    {
        switch (mode)
        {
            case Mode.Buy:
                SaveSystem.instance.GetSaveFile().marketSave.buyPrice = price;
                break;
            case Mode.Sell:
                SaveSystem.instance.GetSaveFile().marketSave.sellPrice = price;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        itemPrice.text = price.ToString("N0");
        CheckButton();
    }

    public void CheckButton()
    {
        bool soldOut = mode == Mode.Sell
            ? SaveSystem.instance.GetSaveFile().marketSave.sellSoldOut
            : SaveSystem.instance.GetSaveFile().marketSave.buySoldOut;
        itemSprite.color = soldOut ? new Color(.3f, .3f, .3f, 1) : Color.white;
        if (mode == Mode.Buy)
            buySellButton.interactable = /*SaveSystem.instance.GetSaveFile().marketSave.coins >= price &&*/ !soldOut;
        else
            buySellButton.interactable = !soldOut && currency switch
            {
                CurrencyVisualizer.Currency.BrownMushroom => SaveSystem.instance.GetSaveFile().stats.mushrooms[0] > 0,
                CurrencyVisualizer.Currency.RedMushroom => SaveSystem.instance.GetSaveFile().stats.mushrooms[1] > 0,
                CurrencyVisualizer.Currency.BlueMushroom => SaveSystem.instance.GetSaveFile().stats.mushrooms[2] > 0,
                CurrencyVisualizer.Currency.Spore => SaveSystem.instance.GetSaveFile().stats.spores > 0,
                CurrencyVisualizer.Currency.SkillPoint => SaveSystem.instance.GetSaveFile().stats.skillPoints > 0,
                CurrencyVisualizer.Currency.BrownPotion =>
                    SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0] > 0,
                CurrencyVisualizer.Currency.RedPotion => SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1] >
                                                         0,
                CurrencyVisualizer.Currency.BluePotion => SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2] >
                                                          0,
                CurrencyVisualizer.Currency.Collectible => SaveSystem.instance.GetSaveFile().collectionItems.Count > 0,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    public void Sell()
    {
        if (SaveSystem.instance.GetSaveFile().marketSave.sellSoldOut) return;
        ValidateAmount();
        uint amount = int.TryParse(inputField.text.Replace(",", ""), out var a) ? (uint)a : 1;
        if (amount == 0)
        {
            GameMaster.instance.Marketplace.merchant.RandomNoItem();
            return;
        }

        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[0] -= amount;
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[1] -= amount;
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[2] -= amount;
                break;
            case CurrencyVisualizer.Currency.Spore:
                SaveSystem.instance.GetSaveFile().stats.spores -= amount;
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                SaveSystem.instance.GetSaveFile().stats.skillPoints -= amount;
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0] -= amount;
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1] -= amount;
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2] -= amount;
                break;
            case CurrencyVisualizer.Currency.Collectible:
                SaveSystem.instance.GetSaveFile().marketSave.sellSoldOut = true;
                SaveSystem.instance.GetSaveFile().collectionItems.Remove(item);
                GameMaster.instance.Hivemind.collectionShelf.RemoveItem(item);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        uint total = price * amount;
        total += (uint)Mathf.FloorToInt(total * SaveSystem.instance.GetSaveFile().GetCollectionMultiplier() * 0.01f);
        SaveSystem.instance.GetSaveFile().marketSave.coins += total;
        inputField.text = "0";
        goldChangedText.text = "+" + total.ToString("N0");
        goldChangedText.DOKill();
        goldChangedText.DOFade(1, 0.1f).OnComplete(() => goldChangedText.DOFade(0, 0.75f).SetDelay(.5f));
        CheckButton();
        GameMaster.instance.Marketplace.merchant.RandomThankYou();
    }

    public void Buy()
    {
        if (SaveSystem.instance.GetSaveFile().marketSave.buySoldOut) return;
        if (SaveSystem.instance.GetSaveFile().marketSave.coins < price)
        {
            GameMaster.instance.Marketplace.merchant.RandomNoMoney();
            return;
        }


        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[0]++;
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[1]++;
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                SaveSystem.instance.GetSaveFile().stats.mushrooms[2]++;
                break;
            case CurrencyVisualizer.Currency.Spore:
                SaveSystem.instance.GetSaveFile().stats.spores++;
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                SaveSystem.instance.GetSaveFile().stats.skillPoints++;
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0]++;
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1]++;
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2]++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        SaveSystem.instance.GetSaveFile().marketSave.coins -= price;
        goldChangedText.text = "-" + (price).ToString("N0");
        goldChangedText.DOKill();
        goldChangedText.DOFade(1, 0.1f).OnComplete(() => goldChangedText.DOFade(0, 0.75f).SetDelay(.5f));
        SaveSystem.instance.GetSaveFile().marketSave.buySoldOut = true;
        CheckButton();
        GameMaster.instance.Marketplace.merchant.RandomThankYou();
    }

    public void Refresh()
    {
        if (mode == Mode.Sell)
        {
            SaveSystem.instance.GetSaveFile().marketSave.sellSoldOut = false;
        }
        else if (mode == Mode.Buy)
        {
            SaveSystem.instance.GetSaveFile().marketSave.buySoldOut = false;
        }

        List<CurrencyVisualizer.Currency> currencies = mode switch
        {
            Mode.Buy => new List<CurrencyVisualizer.Currency>()
            {
                CurrencyVisualizer.Currency.Spore,
                CurrencyVisualizer.Currency.SkillPoint,
                CurrencyVisualizer.Currency.BrownPotion,
                CurrencyVisualizer.Currency.RedPotion,
                CurrencyVisualizer.Currency.BluePotion
            },
            Mode.Sell => new List<CurrencyVisualizer.Currency>()
            {
                CurrencyVisualizer.Currency.BrownPotion,
                CurrencyVisualizer.Currency.RedPotion,
                CurrencyVisualizer.Currency.BluePotion,
                CurrencyVisualizer.Currency.Collectible
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        if (SaveSystem.instance.GetSaveFile().GetCollectionMultiplier() == 0)
        {
            currencies.Remove(CurrencyVisualizer.Currency.Collectible);
        }

        SetCurrency(currencies[Random.Range(0, currencies.Count)]);

        UpdatePrice();
    }

    public void Clear()
    {
        inputField.text = "0";
        ValidateAmount();
    }

    public void ValidateAmount()
    {
        if (mode == Mode.Buy) return; //TODO remove this when you can buy more than 1 item

        int amount = int.TryParse(inputField.text.Replace(",", ""), out var a) ? a : 0;
        if (amount < 0)
        {
            inputField.text = "0";
        }

        if (mode == Mode.Buy)
        {
            ValidateBuy(amount);
        }
        else
        {
            ValidateSell(amount);
        }
    }

    private void ValidateBuy(int amount)
    {
        if (amount * price > SaveSystem.instance.GetSaveFile().marketSave.coins)
        {
            inputField.text = (SaveSystem.instance.GetSaveFile().marketSave.coins / price).ToString("N0");
        }
    }

    private void ValidateSell(int amount)
    {
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().stats.mushrooms[0])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[0].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().stats.mushrooms[1])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[1].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().stats.mushrooms[2])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().stats.mushrooms[2].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.Spore:
                if (amount > SaveSystem.instance.GetSaveFile().stats.spores)
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().stats.spores.ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                if (amount > SaveSystem.instance.GetSaveFile().stats.skillPoints)
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().stats.skillPoints.ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                if (amount > SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[0].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.RedPotion:
                if (amount > SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[1].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BluePotion:
                if (amount > SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().marketSave.potionsCount[2].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.Collectible:
                inputField.text = "1";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}