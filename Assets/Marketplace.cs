using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Marketplace : MonoBehaviour
{
    public uint ticksToRefresh
    {
        get => SaveSystem.instance.GetSaveFile().shopTicks;
        set => SaveSystem.instance.GetSaveFile().shopTicks = value;
    }

    public int ticksToRefreshDefault = 15000;
    public int tickRange = 5000;
    public MarketPreview buyPreview;
    public MarketPreview sellPreview;
    
    public Merchant merchant;
    
    public TextMeshProUGUI timerText;
    
    [Header("BrownValue")]
    public UpgradeContainer brownValueButton;
    public uint brownValueCost = 5;
    public uint brownValueRatio = 2;
    public uint brownValueMax = 20;
    public float brownValueMultiplierGain = 0.5f;
    
    [Header("RedValue")]
    public UpgradeContainer redValueButton;
    public uint redValueCost = 5;
    public uint redValueRatio = 2;
    public uint redValueMax = 20;
    public float redValueMultiplierGain = 0.5f;
    
    [Header("BlueValue")]
    public UpgradeContainer blueValueButton;
    public uint blueValueCost = 5;
    public uint blueValueRatio = 2;
    public uint blueValueMax = 20;
    public float blueValueMultiplierGain = 0.5f;
    
    void Start()
    {
        Refresh();
        brownValueButton.SetIcon(Resources.Load<Sprite>("Sprites/Coin"));
        redValueButton.SetIcon(Resources.Load<Sprite>("Sprites/Coin"));
        blueValueButton.SetIcon(Resources.Load<Sprite>("Sprites/Coin"));
    }

    

    void FixedUpdate()
    {
        TickTimer();
        if (GameMaster.instance.ModeMaster.currentMode != ModeMaster.Gamemode.Marketplace) return;
        TickUpgrades();
    }

    private void TickUpgrades()
    {
        if (brownValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().brownMultiplier >= brownValueMax)
        {
            brownValueButton.gameObject.SetActive(false);
        }
        else
        {
            brownValueButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= brownValueCost);
        }

        if (redValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().redMultiplier >= redValueMax)
        {
            redValueButton.gameObject.SetActive(false);
        }
        else
        {
            redValueButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= redValueCost &&
                                        SaveSystem.instance.GetSaveFile().redUnlocked);
        }

        if (blueValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().blueMultiplier >= blueValueMax)
        {
            blueValueButton.gameObject.SetActive(false);
        }
        else
        {
            blueValueButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= blueValueCost &&
                                         SaveSystem.instance.GetSaveFile().blueUnlocked);
        }
    }

    private void TickTimer()
    {
        ticksToRefresh--;
        if (ticksToRefresh <= 1)
        {
            Refresh();
        }

        if (GameMaster.instance.ModeMaster.currentMode != ModeMaster.Gamemode.Marketplace) return;
        buyPreview.CheckButton();
        sellPreview.CheckButton();
        //format tickstoRefresh to minutes and seconds. 50 ticks = 1 second
        uint minutes = ticksToRefresh / 3000;
        uint seconds = (ticksToRefresh % 3000) / 50;
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void Refresh()
    {
        merchant.RandomSaying();
        ticksToRefresh = (uint)(ticksToRefreshDefault + Random.Range(-tickRange, tickRange));
        do
        {
            buyPreview.Refresh();
            sellPreview.Refresh();
        } while (buyPreview.currency == sellPreview.currency);
        sellPreview.ValidateAmount();
    }
    
      
    public void BrownValue()
    {
        if (SaveSystem.instance.SpendHivemindPoints(brownValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().brownMultiplier++;
            SaveSystem.instance.Save();
            UpdateBrownValueText();
        }
    }

    private void UpdateBrownValueText()
    {
        brownValueCost = (uint)(Mathf.Pow(brownValueRatio, SaveSystem.instance.GetSaveFile().brownMultiplier));
        brownValueButton.SetCostText(brownValueCost.ToString("N0"));
    }

    public void RedValue()
    {
        if (SaveSystem.instance.SpendHivemindPoints(redValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().redMultiplier++;
            SaveSystem.instance.Save();
            UpdateRedValueText();
        }
    }

    private void UpdateRedValueText()
    {
        redValueCost = (uint)(Mathf.Pow(redValueRatio, SaveSystem.instance.GetSaveFile().redMultiplier));
        redValueButton.SetCostText(redValueCost.ToString("N0"));
    }

    public void BlueValue()
    {
        if (SaveSystem.instance.SpendHivemindPoints(blueValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().blueMultiplier++;
            SaveSystem.instance.Save();
            UpdateBlueValueText();
        }
    }

    private void UpdateBlueValueText()
    {
        blueValueCost = (uint)(Mathf.Pow(blueValueRatio, SaveSystem.instance.GetSaveFile().blueMultiplier));
        blueValueButton.SetCostText(blueValueCost.ToString("N0"));
    }
    
    
}
