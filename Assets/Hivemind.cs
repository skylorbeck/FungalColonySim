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
    
    [Header("Cauldron Unlock")]
    public UpgradeContainer unlockCauldronButton;
    public uint unlockCauldronCost = 10;
    
    

    void Start()
    {
        unlockRedButton.SetCostText(unlockRedCost.ToString("N0"));
        unlockBlueButton.SetCostText(unlockBlueCost.ToString("N0"));
        goldenSporeButton.SetCostText(goldenSporeCost.ToString("N0"));
        potentShroomsButton.SetCostText(potentShroomsCost.ToString("N0"));
        enchantSpoonButton.SetCostText(enchantSpoonCost.ToString("N0"));
        unlockCauldronButton.SetCostText(unlockCauldronCost.ToString("N0"));
        UpdateGoldenChanceText();
        UpdateGoldenMultiText();
        
        Sprite skillpoint = Resources.Load<Sprite>("Sprites/SkillPoint");
        
        unlockRedButton.SetIcon(skillpoint);
        unlockBlueButton.SetIcon(skillpoint);
        goldenSporeButton.SetIcon(skillpoint);
        goldenMultiButton.SetIcon(skillpoint);
        goldenChanceButton.SetIcon(skillpoint);
        potentShroomsButton.SetIcon(skillpoint);
        enchantSpoonButton.SetIcon(skillpoint);
        unlockCauldronButton.SetIcon(skillpoint);
        
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
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.redUnlocked = true;
            SaveSystem.SaveS();
            GameMaster.instance.ModeMaster.UpdateDots();
            unlockRedButton.ToggleButton(false);
            unlockRedButton.gameObject.SetActive(false);
            unlockBlueButton.ToggleButton(true);
        }
    }
    
    public void UnlockBlue()
    {
        if (SaveSystem.instance.SpendHivemindPoints(unlockBlueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.blueUnlocked = true;
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
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked = true;
            SaveSystem.SaveS();
            goldenSporeButton.ToggleButton(false);
            goldenSporeButton.gameObject.SetActive(false);
        }
    }
    
    public void UpdateGoldenMultiText()
    {
        goldenMultiCost = (uint)(Mathf.Pow(goldenMultiRatio, SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenMultiplier));
        goldenMultiButton.SetCostText(goldenMultiCost.ToString("N0"));
    }
    
    public void GoldenMulti()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenMultiCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenMultiplier++;
            SaveSystem.SaveS();
            UpdateGoldenMultiText();
        }
    }
    
    public void UpdateGoldenChanceText()
    {
        goldenChanceCost = (uint)(Mathf.Pow(goldenChanceRatio, SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenChanceMultiplier));
        goldenChanceButton.SetCostText(goldenChanceCost.ToString("N0"));
    }
    
    public void GoldenChance()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenChanceCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenChanceMultiplier++;
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
    
    public void UnlockCauldron()
    {
        if (SaveSystem.instance.SpendHivemindPoints(unlockCauldronCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().cauldronSave.isUnlocked = true;
            SaveSystem.instance.GetSaveFile().marketSave.isUnlocked = true;
            SaveSystem.SaveS();
            unlockCauldronButton.gameObject.SetActive(false);
            GameMaster.instance.ModeMaster.UpdateDots();
        }
    }

    void FixedUpdate()
    {
        if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind)return;
        
        TickUpgradeButtons();
    }

    private void TickUpgradeButtons()
    {
        if (unlockRedButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().farmSave.upgrades.redUnlocked)
        {
            unlockRedButton.gameObject.SetActive(false);
        }
        else
        {
            unlockRedButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= unlockRedCost);
        }

        if (unlockBlueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().farmSave.upgrades.blueUnlocked)
        {
            unlockBlueButton.gameObject.SetActive(false);
        }
        else if (!SaveSystem.instance.GetSaveFile().farmSave.upgrades.redUnlocked)
        {
            unlockBlueButton.gameObject.SetActive(false);
        }
        else
        {
            unlockBlueButton.gameObject.SetActive(true);
            unlockBlueButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= unlockBlueCost);
        }


        if (goldenSporeButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked)
        {
            goldenSporeButton.gameObject.SetActive(false);
        }
        else
        {
            goldenSporeButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= goldenSporeCost);
        }

        if (goldenChanceButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenChanceMultiplier >= goldenChanceMax)
        {
            goldenChanceButton.gameObject.SetActive(false);
        } 
        else if (!SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked)
        {
            goldenChanceButton.gameObject.SetActive(false);
        }
        else if (SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked &&
                 SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenChanceMultiplier < goldenChanceMax)
        {
            goldenChanceButton.gameObject.SetActive(true);
            goldenChanceButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= goldenChanceCost);
        }

        if (goldenMultiButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenMultiplier >= goldenMultiMax)
        {
            goldenMultiButton.gameObject.SetActive(false);
        }
        else if (!SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked)
        {
            goldenMultiButton.gameObject.SetActive(false);
        }
        else if (SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenSporeUnlocked && SaveSystem.instance.GetSaveFile().farmSave.upgrades.goldenMultiplier < goldenMultiMax)
        {
            goldenMultiButton.gameObject.SetActive(true);
            goldenMultiButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= goldenMultiCost);
        }

        if (potentShroomsButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.potentShrooms))
        {
            potentShroomsButton.gameObject.SetActive(false);
        }
        else if (!SaveSystem.instance.GetSaveFile().cauldronSave.isUnlocked)
        {
            potentShroomsButton.gameObject.SetActive(false);
        }
        else
        {
            potentShroomsButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= potentShroomsCost);
        }

        if (enchantSpoonButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().cauldronSave.upgrades.spoonEnchanted))
        {
            enchantSpoonButton.gameObject.SetActive(false);
        }
        else if (!SaveSystem.instance.GetSaveFile().cauldronSave.isUnlocked)
        {
            enchantSpoonButton.gameObject.SetActive(false);
        }
        else
        {
            enchantSpoonButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= enchantSpoonCost);
        }

        if (unlockCauldronButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().cauldronSave.isUnlocked))
        {
            unlockCauldronButton.gameObject.SetActive(false);
        }
        else
        {
            unlockCauldronButton.ToggleButton(SaveSystem.instance.GetSaveFile().stats.skillPoints >= unlockCauldronCost);
        }
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
