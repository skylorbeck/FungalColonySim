using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MarketPreview : MonoBehaviour
{
    public uint price = 10; 
    public SpriteRenderer itemSprite;
    public SpriteRenderer shadowSprite;
    public SpriteRenderer rockSprite;
    public TextMeshPro itemName;
    public TextMeshProUGUI itemPrice;
    public Button buySellButton;
    public CurrencyVisualizer.Currency currency;
    public Mode mode = Mode.Sell;
    public TMP_InputField inputField;
    
    public bool soldOut = false;
    
    public float speed = 1f;
    public float timeOffset = 0f;
    public float maxAngle = 15f;
    public float maxHeight = .1f;
    public float shadowSize = 1f;
    
    public void Start()
    {
        SetCurrency(currency);
        UpdatePrice();
    }

    public void FixedUpdate()
    {
        Transform itemSpriteTransform = itemSprite.transform;
        itemSpriteTransform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(timeOffset+Time.fixedTime * speed) * maxAngle);
        float y = Mathf.Cos(timeOffset+Time.fixedTime * speed) * maxHeight;
        itemSpriteTransform.localPosition = new Vector3(0, y, itemSpriteTransform.localPosition.z);
        shadowSprite.transform.localScale = new Vector3(shadowSize - y, shadowSize*0.5f - y*0.5f, shadowSize - y);
        itemName.transform.rotation = itemSpriteTransform.rotation;
        // itemPrice.transform.rotation = itemSpriteTransform.rotation;
    }

    public void SetCurrency(CurrencyVisualizer.Currency currency)
    {
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Brown);
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Red);
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                itemSprite.sprite = MushroomBlock.GetMushroomSprite(MushroomBlock.MushroomType.Blue);
                break;
            case CurrencyVisualizer.Currency.Spore:
                itemSprite.sprite = Resources.Load<Sprite>("Sprites/Spore");
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                itemSprite.sprite = Resources.Load<Sprite>("Sprites/SkillPoint");
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Brown);
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Red);
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                itemSprite.sprite = Cauldron.GetPotionSprite(MushroomBlock.MushroomType.Blue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
        }

        this.currency = currency;
        itemName.text = currency.ToString();
    }

    public void UpdatePrice()
    {
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
                price = (uint)Random.Range(SaveSystem.instance.GetSaveFile().sporeCountTotal*50,SaveSystem.instance.GetSaveFile().sporeCountTotal*100);
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                price = (uint)Random.Range(SaveSystem.instance.GetSaveFile().hivemindPointsTotal*500,SaveSystem.instance.GetSaveFile().hivemindPointsTotal* 1000);
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
            default:
                throw new ArgumentOutOfRangeException();
        }
        SetPrice(price);
        CheckButton();

    }
    
    public void SetPrice(uint price)
    {
        this.price = price;
        itemPrice.text = price.ToString("N0");
        CheckButton();
    }

    public void CheckButton()
    {
        if (mode == Mode.Buy)
            buySellButton.interactable = SaveSystem.instance.GetSaveFile().coins >= price && !soldOut;
        else
            buySellButton.interactable = !soldOut && currency switch
            {
                CurrencyVisualizer.Currency.BrownMushroom => SaveSystem.instance.GetSaveFile().mushrooms[0] > 0,
                CurrencyVisualizer.Currency.RedMushroom => SaveSystem.instance.GetSaveFile().mushrooms[1] > 0,
                CurrencyVisualizer.Currency.BlueMushroom => SaveSystem.instance.GetSaveFile().mushrooms[2] > 0,
                CurrencyVisualizer.Currency.Spore => SaveSystem.instance.GetSaveFile().sporeCount > 0,
                CurrencyVisualizer.Currency.SkillPoint => SaveSystem.instance.GetSaveFile().hivemindPoints > 0,
                CurrencyVisualizer.Currency.BrownPotion => SaveSystem.instance.GetSaveFile().potionsCount[0] > 0,
                CurrencyVisualizer.Currency.RedPotion => SaveSystem.instance.GetSaveFile().potionsCount[1] > 0,
                CurrencyVisualizer.Currency.BluePotion => SaveSystem.instance.GetSaveFile().potionsCount[2] > 0,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    public void Sell()
    {
        if (soldOut) return;
        uint amount = int.TryParse(inputField.text.Replace(",", ""), out var a) ? (uint)a : 1;
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[0]-=amount;
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[1]-=amount;
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[2]-=amount;
                break;
            case CurrencyVisualizer.Currency.Spore:
                SaveSystem.instance.GetSaveFile().sporeCount-=amount;
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                SaveSystem.instance.GetSaveFile().hivemindPoints-=amount;
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                SaveSystem.instance.GetSaveFile().potionsCount[0]-=amount;
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                SaveSystem.instance.GetSaveFile().potionsCount[1]-=amount;
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                SaveSystem.instance.GetSaveFile().potionsCount[2]-=amount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        SaveSystem.instance.GetSaveFile().coins += price * amount;
        CheckButton();
    }

    public void Buy()
    {
        if (soldOut) return;
        if (SaveSystem.instance.GetSaveFile().coins < price) return;
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[0]++;
                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[1]++;
                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                SaveSystem.instance.GetSaveFile().mushrooms[2]++;
                break;
            case CurrencyVisualizer.Currency.Spore:
                SaveSystem.instance.GetSaveFile().sporeCount++;
                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                SaveSystem.instance.GetSaveFile().hivemindPoints++;
                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                SaveSystem.instance.GetSaveFile().potionsCount[0]++;
                break;
            case CurrencyVisualizer.Currency.RedPotion:
                SaveSystem.instance.GetSaveFile().potionsCount[1]++;
                break;
            case CurrencyVisualizer.Currency.BluePotion:
                SaveSystem.instance.GetSaveFile().potionsCount[2]++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        SaveSystem.instance.GetSaveFile().coins -= price;
        soldOut = true;
        itemSprite.color = Color.gray;
        CheckButton();
    }

    public void Refresh()
    {
        CurrencyVisualizer.Currency[] currencies = mode switch
        {
            Mode.Buy => new[]
            {
                CurrencyVisualizer.Currency.Spore, CurrencyVisualizer.Currency.SkillPoint,
                CurrencyVisualizer.Currency.BrownPotion, CurrencyVisualizer.Currency.RedPotion,
                CurrencyVisualizer.Currency.BluePotion
            },
            Mode.Sell => new[]
            {
                CurrencyVisualizer.Currency.BrownMushroom, CurrencyVisualizer.Currency.RedMushroom,
                CurrencyVisualizer.Currency.BlueMushroom, CurrencyVisualizer.Currency.BrownPotion,
                CurrencyVisualizer.Currency.RedPotion, CurrencyVisualizer.Currency.BluePotion
            },
            _ => throw new ArgumentOutOfRangeException()
        };
        
        SetCurrency(currencies[Random.Range(0, currencies.Length)]);

        UpdatePrice();
        CheckButton();
    }

    public enum Mode
    {
        Buy,
        Sell
    }
    
    public void ValidateAmount()
    {
        int amount = int.TryParse(inputField.text.Replace(",", ""), out var a) ? a : 1;
        if (amount < 1)
        {
            inputField.text = "1";
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
        if (amount * price > SaveSystem.instance.GetSaveFile().coins)
        {
            inputField.text = (SaveSystem.instance.GetSaveFile().coins / price).ToString("N0");
        }
    }

    private void ValidateSell(int amount)
    {
        switch (currency)
        {
            case CurrencyVisualizer.Currency.BrownMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().mushrooms[0])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().mushrooms[0].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.RedMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().mushrooms[1])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().mushrooms[1].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BlueMushroom:
                if (amount > SaveSystem.instance.GetSaveFile().mushrooms[2])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().mushrooms[2].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.Spore:
                if (amount > SaveSystem.instance.GetSaveFile().sporeCount)
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().sporeCount.ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.SkillPoint:
                if (amount > SaveSystem.instance.GetSaveFile().hivemindPoints)
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().hivemindPoints.ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BrownPotion:
                if (amount > SaveSystem.instance.GetSaveFile().potionsCount[0])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().potionsCount[0].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.RedPotion:
                if (amount > SaveSystem.instance.GetSaveFile().potionsCount[1])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().potionsCount[1].ToString("N0");
                }

                break;
            case CurrencyVisualizer.Currency.BluePotion:
                if (amount > SaveSystem.instance.GetSaveFile().potionsCount[2])
                {
                    inputField.text = SaveSystem.instance.GetSaveFile().potionsCount[2].ToString("N0");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
