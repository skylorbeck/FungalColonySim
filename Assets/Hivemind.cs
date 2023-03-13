using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hivemind : MonoBehaviour
{
    public PlinkoMachine plinkoMachine;
    public CollectionShelf collectionShelf;
    public HiveMindMode mode = HiveMindMode.Plinko;
    
    public RectTransform hivemindPanel;
    public Toggle upgradeToggle;
    
    [Header("UnlockRed")]
    public UpgradeContainer unlockRedButton;
    public uint unlockRedCost = 3;
    
    [Header("UnlockBlue")]
    public UpgradeContainer unlockBlueButton;
    public uint unlockBlueCost = 5;

    [Header("GoldenSpore")]
    public UpgradeContainer goldenSporeButton;
    public uint goldenSporeCost = 5;
    
    [Header("GoldenMulti")]
    public UpgradeContainer goldenMultiButton;
    public uint goldenMultiCost = 5;
    public uint goldenMultiRatio = 2;
    public uint goldenMultiMax = 5;
    
    [Header("GoldenChance")]
    public UpgradeContainer goldenChanceButton;
    public uint goldenChanceCost = 5;
    public uint goldenChanceRatio = 2;
    public uint goldenChanceMax = 25;
    
    [Header("Potent Shrooms")]
    public UpgradeContainer potentShroomsButton;
    public uint potentShroomsCost = 15;
    
    [Header("Enchant Spoon")]
    public UpgradeContainer enchantSpoonButton;
    public uint enchantSpoonCost = 15;

    void Start()
    {
        unlockRedButton.SetCostText(unlockRedCost.ToString("N0"));
        unlockBlueButton.SetCostText(unlockBlueCost.ToString("N0"));
        goldenSporeButton.SetCostText(goldenSporeCost.ToString("N0"));
        potentShroomsButton.SetCostText(potentShroomsCost.ToString("N0"));
        enchantSpoonButton.SetCostText(enchantSpoonCost.ToString("N0"));
        unlockRedButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        unlockBlueButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenSporeButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenMultiButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenChanceButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        potentShroomsButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        enchantSpoonButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        UpdateGoldenChanceText();
        UpdateGoldenMultiText();
        plinkoMachine.transform.localPosition = new Vector3(0, 0, 0);
        collectionShelf.transform.localPosition = new Vector3(-5, collectionShelf.GetY(), 0);
    }

    void Update()
    {
        
    }
    
    public void UnlockRed()
    {
        if (SaveSystem.instance.SpendHivemindPoints(unlockRedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().redUnlocked = true;
            SaveSystem.SaveS();
            GameMaster.instance.ModeMaster.UpdateDots();
            unlockRedButton.ToggleButton(false);
            unlockRedButton.gameObject.SetActive(false);
        }
    }
    
    public void UnlockBlue()
    {
        if (SaveSystem.instance.SpendHivemindPoints(unlockBlueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().blueUnlocked = true;
            SaveSystem.SaveS();
            GameMaster.instance.ModeMaster.UpdateDots();
            unlockBlueButton.ToggleButton(false);
            unlockBlueButton.gameObject.SetActive(false);
        }
    }

    public void GoldenSpore()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenSporeCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().goldenSporeUnlocked = true;
            SaveSystem.SaveS();
            goldenSporeButton.ToggleButton(false);
            goldenSporeButton.gameObject.SetActive(false);
        }
    }
    
    public void UpdateGoldenMultiText()
    {
        goldenMultiCost = (uint)(Mathf.Pow(goldenMultiRatio, SaveSystem.instance.GetSaveFile().goldenMultiplier));
        goldenMultiButton.SetCostText(goldenMultiCost.ToString("N0"));
    }
    
    public void GoldenMulti()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenMultiCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().goldenMultiplier++;
            SaveSystem.SaveS();
            UpdateGoldenMultiText();
        }
    }
    
    public void UpdateGoldenChanceText()
    {
        goldenChanceCost = (uint)(Mathf.Pow(goldenChanceRatio, SaveSystem.instance.GetSaveFile().goldenChanceMultiplier));
        goldenChanceButton.SetCostText(goldenChanceCost.ToString("N0"));
    }
    
    public void GoldenChance()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenChanceCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().goldenChanceMultiplier++;
            SaveSystem.SaveS();
            UpdateGoldenChanceText();
        }
    }
    
    public void EnchantSpoon()
    {
        if (SaveSystem.instance.SpendHivemindPoints(enchantSpoonCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.spoonEnchanted = true;
            SaveSystem.SaveS();
            enchantSpoonButton.ToggleButton(false);
            enchantSpoonButton.gameObject.SetActive(false);
        }
    }
    
    public void PotentShrooms()
    {
        if (SaveSystem.instance.SpendHivemindPoints(potentShroomsCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.potentShrooms = true;
            SaveSystem.SaveS();
            potentShroomsButton.ToggleButton(false);
            potentShroomsButton.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind)return;
        
        TickUpgradeButtons();
    }

    private void TickUpgradeButtons()
    {
        if (unlockRedButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().redUnlocked)
        {
            unlockRedButton.gameObject.SetActive(false);
        }
        else
        {
            unlockRedButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= unlockRedCost);
        }

        if (unlockBlueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().blueUnlocked)
        {
            unlockBlueButton.gameObject.SetActive(false);
        }
        else
        {
            unlockBlueButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= unlockBlueCost);
        }


        if (goldenSporeButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenSporeButton.gameObject.SetActive(false);
        }
        else
        {
            goldenSporeButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenSporeCost);
        }

        if (goldenChanceButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().goldenChanceMultiplier >= goldenChanceMax)
        {
            goldenChanceButton.gameObject.SetActive(false);
        }
        else if (SaveSystem.instance.GetSaveFile().goldenSporeUnlocked && SaveSystem.instance.GetSaveFile().goldenChanceMultiplier < goldenChanceMax)
        {
            goldenChanceButton.gameObject.SetActive(true);
            goldenChanceButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenChanceCost);
        }

        if (goldenMultiButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().goldenMultiplier >= goldenMultiMax)
        {
            goldenMultiButton.gameObject.SetActive(false);
        }
        else if (SaveSystem.instance.GetSaveFile().goldenSporeUnlocked && SaveSystem.instance.GetSaveFile().goldenMultiplier < goldenMultiMax)
        {
            goldenMultiButton.gameObject.SetActive(true);
            goldenMultiButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenMultiCost);
        }

        if (potentShroomsButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.potentShrooms))
        {
            potentShroomsButton.gameObject.SetActive(false);
        }
        else
        {
            potentShroomsButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= potentShroomsCost);
        }

        if (enchantSpoonButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.spoonEnchanted))
        {
            enchantSpoonButton.gameObject.SetActive(false);
        }
        else
        {
            enchantSpoonButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= enchantSpoonCost);
        }
    }

    public void ToggleShowUpgrades()
    {
        hivemindPanel.DOLocalMoveX(upgradeToggle.isOn ? 0 : 900, 0.5f).SetEase(Ease.OutBounce);
    }
    
    public enum HiveMindMode
    {
        Plinko,
        Collection,
    }
    public void ToggleHiveMindMode()
    {
        SFXMaster.instance.PlayMenuClick();
        if (mode == HiveMindMode.Plinko)
        {
            mode = HiveMindMode.Collection;
            plinkoMachine.transform.DOMoveY(-200, 0.5f).SetEase(Ease.InOutCubic);
            collectionShelf.transform.DOMoveY(0, 0.5f).SetEase(Ease.InOutCubic);
            // plinkoButton.ToggleButton(false); //TODO replace this with plinko UI toggle
            // collectionButton.ToggleButton(true); //TODO replace this with collection UI toggle
        }
        else
        {
            mode = HiveMindMode.Plinko;
            plinkoMachine.transform.DOMoveY(0, 0.5f).SetEase(Ease.InOutCubic);
            collectionShelf.transform.DOMoveY(collectionShelf.GetY(), 0.5f).SetEase(Ease.InOutCubic);
            // plinkoButton.ToggleButton(true);
            // collectionButton.ToggleButton(false);
        }
    }
}
