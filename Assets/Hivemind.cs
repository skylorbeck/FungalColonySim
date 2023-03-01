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
    public Button unlockRedButton;
    public TextMeshProUGUI unlockRedText;
    public uint unlockRedCost = 3;
    
    [Header("UnlockBlue")]
    public Button unlockBlueButton;
    public TextMeshProUGUI unlockBlueText;
    public uint unlockBlueCost = 5;
    
    [Header("BrownValue")]
    public Button brownValueButton;
    public TextMeshProUGUI brownValueText;
    public uint brownValueCost = 5;
    public uint brownValueRatio = 2;
    public uint brownValueMax = 100;
    public float brownValueMultiplierGain = 0.05f;
    
    [Header("RedValue")]
    public Button redValueButton;
    public TextMeshProUGUI redValueText;
    public uint redValueCost = 5;
    public uint redValueRatio = 2;
    public uint redValueMax = 100;
    public float redValueMultiplierGain = 0.05f;
    
    [Header("BlueValue")]
    public Button blueValueButton;
    public TextMeshProUGUI blueValueText;
    public uint blueValueCost = 5;
    public uint blueValueRatio = 2;
    public uint blueValueMax = 100;
    public float blueValueMultiplierGain = 0.05f;
    
    [Header("GoldenSpore")]
    public Button goldenSporeButton;
    public TextMeshProUGUI goldenSporeText;
    public uint goldenSporeCost = 5;
    
    [Header("GoldenMulti")]
    public Button goldenMultiButton;
    public TextMeshProUGUI goldenMultiText;
    public uint goldenMultiCost = 5;
    public uint goldenMultiRatio = 2;
    public uint goldenMultiMax = 5;
    
    [Header("GoldenChance")]
    public Button goldenChanceButton;
    public TextMeshProUGUI goldenChanceText;
    public uint goldenChanceCost = 5;
    public uint goldenChanceRatio = 2;
    public uint goldenChanceMax = 25;

    void Start()
    {
        unlockRedText.text = unlockRedCost + "x";
        unlockBlueText.text = unlockBlueCost + "x";
        UpdateBrownValueText();
        UpdateRedValueText();
        UpdateBlueValueText();
        goldenSporeText.text = goldenSporeCost + "x";
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
            unlockRedButton.interactable = false;
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
            unlockBlueButton.interactable = false;
            unlockBlueButton.gameObject.SetActive(false);
        }
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
        brownValueText.text = brownValueCost + "x";
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
        redValueText.text = redValueCost + "x";
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
        blueValueText.text = blueValueCost + "x";
    }

    public void GoldenSpore()
    {
        if (SaveSystem.instance.SpendHivemindPoints(goldenSporeCost))
        {
            SFXMaster.instance.PlayMenuClick();
            SaveSystem.instance.GetSaveFile().goldenSporeUnlocked = true;
            SaveSystem.instance.Save();
            goldenSporeButton.interactable = false;
            goldenSporeButton.gameObject.SetActive(false);
        }
    }
    
    public void UpdateGoldenMultiText()
    {
        goldenMultiCost = (uint)(Mathf.Pow(goldenMultiRatio, SaveSystem.instance.GetSaveFile().goldenMultiplier));
        goldenMultiText.text = goldenMultiCost + "x";
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
        goldenChanceText.text = goldenChanceCost + "x";
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
        float size = 0.1f+ Mathf.Clamp01(SaveSystem.instance.GetSaveFile().hivemindPointsTotal / 1000f) * 5;
        spriteRenderer.transform.localScale = new Vector3(size, size,size);
        
        if (unlockRedButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().redUnlocked)
        {
            unlockRedButton.gameObject.SetActive(false);
        }
        else
        {
            unlockRedButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= unlockRedCost;
        }
        
        if (unlockBlueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().blueUnlocked)
        {
            unlockBlueButton.gameObject.SetActive(false);
        } 
        else
        {
            unlockBlueButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= unlockBlueCost;
        }

        if (brownValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().brownMultiplier >= brownValueMax)
        {
            brownValueButton.gameObject.SetActive(false);
        }
        else
        {
            brownValueButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= brownValueCost;
        }
        
        if (redValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().redMultiplier >= redValueMax)
        {
            redValueButton.gameObject.SetActive(false);
        }
        else
        {
            redValueButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= redValueCost && SaveSystem.instance.GetSaveFile().redUnlocked;
        }
        
        if (blueValueButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().blueMultiplier >= blueValueMax)
        {
            blueValueButton.gameObject.SetActive(false);
        }
        else
        {
            blueValueButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= blueValueCost && SaveSystem.instance.GetSaveFile().blueUnlocked;
        }
        
        if (goldenSporeButton.isActiveAndEnabled && SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenSporeButton.gameObject.SetActive(false);
        }
        else
        {
            goldenSporeButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenSporeCost;
        }

        if (goldenChanceButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().goldenChanceMultiplier >= goldenChanceMax)|| !SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenChanceButton.gameObject.SetActive(false);
        }
        else
        {
            goldenChanceButton.gameObject.SetActive(true);
            goldenChanceButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenChanceCost;
        }

        if (goldenMultiButton.isActiveAndEnabled && (SaveSystem.instance.GetSaveFile().goldenMultiplier >= goldenMultiMax)|| !SaveSystem.instance.GetSaveFile().goldenSporeUnlocked)
        {
            goldenMultiButton.gameObject.SetActive(false);
        }
        else
        {
            goldenMultiButton.gameObject.SetActive(true);
            goldenMultiButton.interactable = SaveSystem.instance.GetSaveFile().hivemindPoints >= goldenMultiCost;
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
        hivemindPanel.DOLocalMoveX(upgradeToggle.isOn ? 700 : 1500, 0.5f).SetEase(Ease.OutBounce);
    }
}
