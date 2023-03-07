using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hivemind : MonoBehaviour, IPointerClickHandler
{
    public SpriteRenderer spriteRenderer;
    
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

    void Start()
    {
        unlockRedButton.SetCostText(unlockRedCost.ToString("N0"));
        unlockBlueButton.SetCostText(unlockBlueCost.ToString("N0"));
        goldenSporeButton.SetCostText(goldenSporeCost.ToString("N0"));
        unlockRedButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        unlockBlueButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenSporeButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenMultiButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        goldenChanceButton.SetIcon(Resources.Load<Sprite>("Sprites/SkillPoint"));
        UpdateGoldenChanceText();
        UpdateGoldenMultiText();
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
            SaveSystem.instance.Save();
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
            SaveSystem.instance.Save();
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
            SaveSystem.instance.Save();
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
            SaveSystem.instance.Save();
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
            SaveSystem.instance.Save();
            UpdateGoldenChanceText();
        }
    }

    void FixedUpdate()
    {
        if (GameMaster.instance.ModeMaster.currentMode!=ModeMaster.Gamemode.Hivemind)return;
        
        float size = 0.1f+ Mathf.Clamp01(SaveSystem.instance.GetSaveFile().hivemindPointsTotal / 1000f) * 5;
        spriteRenderer.transform.localScale = new Vector3(size, size,size);
        
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

        if (goldenChanceButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().goldenChanceMultiplier >= goldenChanceMax)|| !SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenChanceButton.gameObject.SetActive(false);
        }
        else
        {
            goldenChanceButton.gameObject.SetActive(true);
            goldenChanceButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenChanceCost);
        }

        if (goldenMultiButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().goldenMultiplier >= goldenMultiMax)|| !SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenMultiButton.gameObject.SetActive(false);
        }
        else
        {
            goldenMultiButton.gameObject.SetActive(true);
            goldenMultiButton.ToggleButton(SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenMultiCost);
        }
    }

    public void ShakeStuff()
    {
        transform.DOComplete();
        transform.DOShakeScale(0.5f, 0.5f, 15, 50);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShakeStuff();
    }
    
    public void ToggleShowUpgrades()
    {
        hivemindPanel.DOLocalMoveX(upgradeToggle.isOn ? 0 : 900, 0.5f).SetEase(Ease.OutBounce);
    }
}
