using System.Collections;
using TMPro;
using UnityEngine;

public class Marketplace : MonoBehaviour
{
    public int ticksToRefreshDefault = 15000;
    public int tickRange = 5000;
    public MarketPreview buyPreview;
    public MarketPreview sellPreview;

    public Merchant merchant;

    public TextMeshProUGUI timerText;

    [Header("BrownValue")] public UpgradeContainer brownValueButton;

    public uint brownValueCost = 5;
    public uint brownValueRatio = 3;
    public uint brownValueMax = 20;
    public float brownValueMultiplierGain = 0.5f;

    [Header("RedValue")] public UpgradeContainer redValueButton;

    public uint redValueCost = 5;
    public uint redValueRatio = 3;
    public uint redValueMax = 20;
    public float redValueMultiplierGain = 0.5f;

    [Header("BlueValue")] public UpgradeContainer blueValueButton;

    public uint blueValueCost = 5;
    public uint blueValueRatio = 3;
    public uint blueValueMax = 20;
    public float blueValueMultiplierGain = 0.5f;

    [Header("SporeValue")] public UpgradeContainer sporeValueButton;

    public uint sporeValueCost = 1000;
    public uint sporeValueRatio = 10;
    public uint sporeValueMax = 20;

    [Header("BetterCauldron")] public UpgradeContainer betterCauldronButton;

    public uint betterCauldronCost = 100000;

    [Header("AutoWood")] public UpgradeContainer autoWoodButton;

    public uint autoWoodCost = 5000;

    [Header("PercentButtons")] public UpgradeContainer percentButtonsUpgradeContainer;

    public uint percentButtonsCost = 5000;

    [Header("EventAmount")] public UpgradeContainer evenAmountButton;

    public uint eventAmountCost = 5000;

    [Header("BallGenerationSpeed")] public UpgradeContainer ballGenerationSpeedButton;

    public uint ballGenerationSpeedCost = 5000;
    public uint ballGenerationSpeedBaseCost = 5000;
    public uint ballGenerationSpeedMax = 10;

    [Header("BallGenerationAmount")] public UpgradeContainer ballGenerationAmountButton;

    public uint ballGenerationAmountBaseCost = 5000;
    public uint ballGenerationAmountCost = 5000;
    public float ballGenerationAmountCostMultiplier = 2.25f;
    public float ballGenerationAmountMax = 10;

    [Header("BallSoftCap")] public UpgradeContainer ballSoftCapButton;

    public uint ballSoftCapCost = 50000;

    [Header("GoldenBall")] public UpgradeContainer goldenBallButton;

    public uint goldenBallCost = 50000;

    [Header("GoldenPeg")] public UpgradeContainer goldenPegButton;

    public uint goldenPegCost = 25000;

    [Header("AutoFire")] public UpgradeContainer autoFireButton;

    public uint autoFireCost = 100000;

    [Header("CauldronClickPower")] public UpgradeContainer cauldronClickPowerButton;

    public uint cauldronClickPowerBaseCost = 100000;
    public uint cauldronClickPowerCost = 100000;
    public float cauldronClickPowerCostMultiplier = 2.25f;
    public float cauldronClickPowerMax = 9;

    public uint ticksToRefresh
    {
        get => SaveSystem.save.marketSave.shopTicks;
        set => SaveSystem.save.marketSave.shopTicks = value;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveSystem.instance.loaded);
        GameMaster.instance.ModeMaster.OnModeChange += sellPreview.Clear;
        Refresh();
        Sprite coinSprite = Resources.Load<Sprite>("Sprites/Coin");
        brownValueButton.SetIcon(coinSprite);
        redValueButton.SetIcon(coinSprite);
        blueValueButton.SetIcon(coinSprite);
        sporeValueButton.SetIcon(coinSprite);
        betterCauldronButton.SetIcon(coinSprite);
        autoWoodButton.SetIcon(coinSprite);
        percentButtonsUpgradeContainer.SetIcon(coinSprite);
        evenAmountButton.SetIcon(coinSprite);
        ballGenerationSpeedButton.SetIcon(coinSprite);
        ballGenerationAmountButton.SetIcon(coinSprite);
        ballSoftCapButton.SetIcon(coinSprite);
        goldenBallButton.SetIcon(coinSprite);
        goldenPegButton.SetIcon(coinSprite);
        autoFireButton.SetIcon(coinSprite);
        cauldronClickPowerButton.SetIcon(coinSprite);
        UpdateBrownValueText();
        UpdateRedValueText();
        UpdateBlueValueText();
        UpdateSporeValueText();
        betterCauldronButton.SetCostText(betterCauldronCost.ToString("N0"));
        autoWoodButton.SetCostText(autoWoodCost.ToString("N0"));
        percentButtonsUpgradeContainer.SetCostText(percentButtonsCost.ToString("N0"));
        evenAmountButton.SetCostText(eventAmountCost.ToString("N0"));
        UpdateBallGenerationSpeedText();
        UpdateBallGenerationAmountText();
        ballSoftCapButton.SetCostText(ballSoftCapCost.ToString("N0"));
        goldenBallButton.SetCostText(goldenBallCost.ToString("N0"));
        goldenPegButton.SetCostText(goldenPegCost.ToString("N0"));
        autoFireButton.SetCostText(autoFireCost.ToString("N0"));
        UpdateCauldronClickPowerText();
    }


    void FixedUpdate()
    {
        TickTimer();
        if (!GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Marketplace)) return;
        TickUpgrades();
    }

    private void UpdateCauldronClickPowerText()
    {
        cauldronClickPowerCost = (uint)(cauldronClickPowerBaseCost * cauldronClickPowerCostMultiplier *
                                        (SaveSystem.save.cauldronSave.upgrades.clickPower +
                                         1));
        cauldronClickPowerButton.SetCostText(cauldronClickPowerCost.ToString("N0"));
    }

    private void TickUpgrades()
    {
        if (brownValueButton.isActiveAndEnabled &&
            SaveSystem.save.farmSave.upgrades.brownMultiplier >= brownValueMax)
        {
            brownValueButton.gameObject.SetActive(false);
        }
        else
        {
            brownValueButton.ToggleButton(SaveSystem.save.marketSave.coins >= brownValueCost);
        }

        if (redValueButton.isActiveAndEnabled &&
            SaveSystem.save.farmSave.upgrades.redMultiplier >= redValueMax)
        {
            redValueButton.gameObject.SetActive(false);
        }
        else
        {
            redValueButton.ToggleButton(SaveSystem.save.marketSave.coins >= redValueCost &&
                                        SaveSystem.save.farmSave.upgrades.redUnlocked);
        }

        if (blueValueButton.isActiveAndEnabled &&
            SaveSystem.save.farmSave.upgrades.blueMultiplier >= blueValueMax)
        {
            blueValueButton.gameObject.SetActive(false);
        }
        else
        {
            blueValueButton.ToggleButton(SaveSystem.save.marketSave.coins >= blueValueCost &&
                                         SaveSystem.save.farmSave.upgrades.blueUnlocked);
        }

        if (sporeValueButton.isActiveAndEnabled &&
            SaveSystem.save.farmSave.upgrades.sporeMultiplier >= sporeValueMax)
        {
            sporeValueButton.gameObject.SetActive(false);
        }
        else
        {
            sporeValueButton.ToggleButton(SaveSystem.save.marketSave.coins >= sporeValueCost);
        }

        if (betterCauldronButton.isActiveAndEnabled &&
            SaveSystem.save.cauldronSave.upgrades.betterCauldron)
        {
            betterCauldronButton.gameObject.SetActive(false);
        }
        else
        {
            betterCauldronButton.ToggleButton(SaveSystem.save.marketSave.coins >= betterCauldronCost);
        }

        if (autoWoodButton.isActiveAndEnabled && SaveSystem.save.cauldronSave.upgrades.autoWood)
        {
            autoWoodButton.gameObject.SetActive(false);
        }
        else
        {
            autoWoodButton.ToggleButton(SaveSystem.save.marketSave.coins >= autoWoodCost);
        }

        if (percentButtonsUpgradeContainer.isActiveAndEnabled &&
            SaveSystem.save.cauldronSave.upgrades.percentButtons)
        {
            percentButtonsUpgradeContainer.gameObject.SetActive(false);
        }
        else
        {
            percentButtonsUpgradeContainer.ToggleButton(SaveSystem.save.marketSave.coins >=
                                                        percentButtonsCost);
        }

        if (evenAmountButton.isActiveAndEnabled && SaveSystem.save.cauldronSave.upgrades.evenAmount)
        {
            evenAmountButton.gameObject.SetActive(false);
        }
        else
        {
            evenAmountButton.ToggleButton(SaveSystem.save.marketSave.coins >= eventAmountCost);
        }

        if (ballGenerationSpeedButton.isActiveAndEnabled &&
            SaveSystem.save.plinkoSave.ballRegenSpeed >= ballGenerationSpeedMax - 0.01f)
        {
            ballGenerationSpeedButton.gameObject.SetActive(false);
        }
        else
        {
            ballGenerationSpeedButton.ToggleButton(SaveSystem.save.marketSave.coins >=
                                                   ballGenerationSpeedCost);
        }

        if (ballGenerationAmountButton.isActiveAndEnabled &&
            SaveSystem.save.plinkoSave.ballRegenAmount >= ballGenerationAmountMax)
        {
            ballGenerationAmountButton.gameObject.SetActive(false);
        }
        else
        {
            ballGenerationAmountButton.ToggleButton(SaveSystem.save.marketSave.coins >=
                                                    ballGenerationAmountCost);
        }

        if (ballSoftCapButton.isActiveAndEnabled && SaveSystem.save.plinkoSave.ballSoftCap >= 100)
        {
            ballSoftCapButton.gameObject.SetActive(false);
        }
        else
        {
            ballSoftCapButton.ToggleButton(SaveSystem.save.marketSave.coins >= ballSoftCapCost);
        }

        if (goldenBallButton.isActiveAndEnabled && SaveSystem.save.plinkoSave.goldenBallsUnlocked)
        {
            goldenBallButton.gameObject.SetActive(false);
        }
        else
        {
            goldenBallButton.ToggleButton(SaveSystem.save.marketSave.coins >= goldenBallCost);
        }

        if (goldenPegButton.isActiveAndEnabled && SaveSystem.save.plinkoSave.goldenPegsUnlocked)
        {
            goldenPegButton.gameObject.SetActive(false);
        }
        else
        {
            goldenPegButton.ToggleButton(SaveSystem.save.marketSave.coins >= goldenPegCost);
        }

        if (autoFireButton.isActiveAndEnabled && SaveSystem.save.plinkoSave.autofireUnlocked)
        {
            autoFireButton.gameObject.SetActive(false);
        }
        else
        {
            autoFireButton.ToggleButton(SaveSystem.save.marketSave.coins >= autoFireCost);
        }

        if (cauldronClickPowerButton.isActiveAndEnabled &&
            SaveSystem.save.cauldronSave.upgrades.clickPower >= cauldronClickPowerMax - 0.01f)
        {
            cauldronClickPowerButton.gameObject.SetActive(false);
        }
        else
        {
            cauldronClickPowerButton.ToggleButton(SaveSystem.save.marketSave.coins >=
                                                  cauldronClickPowerCost);
        }
    }

    private void TickTimer()
    {
        ticksToRefresh--;
        if (ticksToRefresh <= 1)
        {
            Refresh();
        }

        if (!GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Marketplace))
        {
            return;
        }

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
        if (SaveSystem.instance.SpendCoins(brownValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.brownMultiplier++;
            SaveSystem.SaveS();
            UpdateBrownValueText();
        }
    }

    private void UpdateBrownValueText()
    {
        brownValueCost = (uint)(Mathf.Pow(brownValueRatio,
            SaveSystem.save.farmSave.upgrades.brownMultiplier));
        brownValueButton.SetCostText(brownValueCost.ToString("N0"));
    }

    public void RedValue()
    {
        if (SaveSystem.instance.SpendCoins(redValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.redMultiplier++;
            SaveSystem.SaveS();
            UpdateRedValueText();
        }
    }

    private void UpdateRedValueText()
    {
        redValueCost =
            (uint)(Mathf.Pow(redValueRatio, SaveSystem.save.farmSave.upgrades.redMultiplier));
        redValueButton.SetCostText(redValueCost.ToString("N0"));
    }

    public void BlueValue()
    {
        if (SaveSystem.instance.SpendCoins(blueValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.blueMultiplier++;
            SaveSystem.SaveS();
            UpdateBlueValueText();
        }
    }

    private void UpdateBlueValueText()
    {
        blueValueCost = (uint)(Mathf.Pow(blueValueRatio,
            SaveSystem.save.farmSave.upgrades.blueMultiplier));
        blueValueButton.SetCostText(blueValueCost.ToString("N0"));
    }

    public void SporeValue()
    {
        if (SaveSystem.instance.SpendCoins(sporeValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.farmSave.upgrades.sporeMultiplier++;
            SaveSystem.SaveS();
            UpdateSporeValueText();
        }
    }

    private void UpdateSporeValueText()
    {
        sporeValueCost = (uint)(Mathf.Pow(sporeValueRatio,
            SaveSystem.save.farmSave.upgrades.sporeMultiplier));
        sporeValueButton.SetCostText(sporeValueCost.ToString("N0"));
    }

    public void BetterCauldron()
    {
        if (SaveSystem.instance.SpendCoins(betterCauldronCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.cauldronSave.upgrades.betterCauldron = true;
            SaveSystem.SaveS();
            betterCauldronButton.gameObject.SetActive(false);
        }
    }

    public void AutoWood()
    {
        if (SaveSystem.instance.SpendCoins(autoWoodCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.cauldronSave.upgrades.autoWood = true;
            GameMaster.instance.Cauldron.fuelButton.gameObject.SetActive(false);
            GameMaster.instance.Cauldron.AddFuel();
            SaveSystem.SaveS();
            autoWoodButton.gameObject.SetActive(false);
        }
    }

    public void PercentButtons()
    {
        if (SaveSystem.instance.SpendCoins(percentButtonsCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.cauldronSave.upgrades.percentButtons = true;
            GameMaster.instance.Cauldron.percentButtons.gameObject.SetActive(true);
            SaveSystem.SaveS();
            percentButtonsUpgradeContainer.gameObject.SetActive(false);
        }
    }

    public void EvenAmount()
    {
        if (SaveSystem.instance.SpendCoins(eventAmountCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.cauldronSave.upgrades.evenAmount = true;
            GameMaster.instance.Cauldron.evenPotionButtons.gameObject.SetActive(true);
            SaveSystem.SaveS();
            evenAmountButton.gameObject.SetActive(false);
        }
    }

    public void BallGenerationSpeed()
    {
        if (SaveSystem.instance.SpendCoins(ballGenerationSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.ballRegenSpeed *= 2;
            SaveSystem.SaveS();
            UpdateBallGenerationSpeedText();
        }
    }

    private void UpdateBallGenerationSpeedText()
    {
        ballGenerationSpeedCost =
            (uint)(ballGenerationSpeedBaseCost * SaveSystem.save.plinkoSave.ballRegenSpeed);
        ballGenerationSpeedButton.SetCostText(ballGenerationSpeedCost.ToString("N0"));
    }

    public void BallGenerationAmount()
    {
        if (SaveSystem.instance.SpendCoins(ballGenerationAmountCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.ballRegenAmount++;
            SaveSystem.SaveS();
            UpdateBallGenerationAmountText();
        }
    }

    private void UpdateBallGenerationAmountText()
    {
        ballGenerationAmountCost = (uint)(ballGenerationAmountBaseCost * ballGenerationAmountCostMultiplier *
                                          SaveSystem.save.plinkoSave.ballRegenAmount);
        ballGenerationAmountButton.SetCostText(ballGenerationAmountCost.ToString("N0"));
    }

    public void BallSoftCap()
    {
        if (SaveSystem.instance.SpendCoins(ballSoftCapCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.ballSoftCap = 100;
            SaveSystem.SaveS();
            ballSoftCapButton.gameObject.SetActive(false);
            GameMaster.instance.Hivemind.plinkoMachine.UpdateSoftCap();
        }
    }

    public void GoldenBalls()
    {
        if (SaveSystem.instance.SpendCoins(goldenBallCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.goldenBallsUnlocked = true;
            SaveSystem.SaveS();
            goldenBallButton.gameObject.SetActive(false);
        }
    }

    public void GoldenPegs()
    {
        if (SaveSystem.instance.SpendCoins(goldenPegCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.goldenPegsUnlocked = true;
            SaveSystem.SaveS();
            goldenPegButton.gameObject.SetActive(false);
        }
    }

    public void AutoFire()
    {
        if (SaveSystem.instance.SpendCoins(autoFireCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.plinkoSave.autofireUnlocked = true;
            SaveSystem.SaveS();
            autoFireButton.gameObject.SetActive(false);
        }
    }

    public void CauldronClickPower()
    {
        if (SaveSystem.instance.SpendCoins(cauldronClickPowerCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.save.cauldronSave.upgrades.clickPower += 1;
            SaveSystem.SaveS();
            UpdateCauldronClickPowerText();
        }
    }
}