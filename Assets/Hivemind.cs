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

    void Start()
    {
        unlockRedText.text = unlockRedCost + "x";
        unlockBlueText.text = unlockBlueCost + "x";
        UpdateBrownValueText();
        UpdateRedValueText();
        UpdateBlueValueText();
        goldenSporeText.text = goldenSporeCost + "x";
    }

    void Update()
    {
        
    }
    
    public void UnlockRed()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(unlockRedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.redUnlocked = true;
            GameMaster.instance.SaveSystem.Save();
            GameMaster.instance.ModeMaster.UpdateDots();
            unlockRedButton.interactable = false;
            unlockRedButton.gameObject.SetActive(false);
        }
    }
    
    public void UnlockBlue()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(unlockBlueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.blueUnlocked = true;
            GameMaster.instance.SaveSystem.Save();
            GameMaster.instance.ModeMaster.UpdateDots();
            unlockBlueButton.interactable = false;
            unlockBlueButton.gameObject.SetActive(false);
        }
    }
    
    public void BrownValue()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(brownValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.brownMultiplier++;
            GameMaster.instance.SaveSystem.Save();
            UpdateBrownValueText();
        }
    }

    private void UpdateBrownValueText()
    {
        brownValueCost = (uint)(Mathf.Pow(brownValueRatio, GameMaster.instance.SaveSystem.brownMultiplier));
        brownValueText.text = brownValueCost + "x";
    }

    public void RedValue()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(redValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.redMultiplier++;
            GameMaster.instance.SaveSystem.Save();
            UpdateRedValueText();
        }
    }

    private void UpdateRedValueText()
    {
        redValueCost = (uint)(Mathf.Pow(redValueRatio, GameMaster.instance.SaveSystem.redMultiplier));
        redValueText.text = redValueCost + "x";
    }

    public void BlueValue()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(blueValueCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.blueMultiplier++;
            GameMaster.instance.SaveSystem.Save();
            UpdateBlueValueText();
        }
    }

    private void UpdateBlueValueText()
    {
        blueValueCost = (uint)(Mathf.Pow(blueValueRatio, GameMaster.instance.SaveSystem.blueMultiplier));
        blueValueText.text = blueValueCost + "x";
    }

    public void GoldenSpore()
    {
        if (GameMaster.instance.SaveSystem.SpendHivemindPoints(goldenSporeCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.goldenSporeUnlocked = true;
            GameMaster.instance.SaveSystem.Save();
            goldenSporeButton.interactable = false;
            goldenSporeButton.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        float size = 0.1f+ Mathf.Clamp01(GameMaster.instance.SaveSystem.hivemindPointsTotal / 1000f) * 5;
        spriteRenderer.transform.localScale = new Vector3(size, size,size);
        
        if (unlockRedButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.redUnlocked)
        {
            unlockRedButton.gameObject.SetActive(false);
        }
        else
        {
            unlockRedButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= unlockRedCost;
        }
        
        if (unlockBlueButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.blueUnlocked)
        {
            unlockBlueButton.gameObject.SetActive(false);
        } 
        else
        {
            unlockBlueButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= unlockBlueCost;
        }

        if (brownValueButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.brownMultiplier >= brownValueMax)
        {
            brownValueButton.gameObject.SetActive(false);
        }
        else
        {
            brownValueButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= brownValueCost;
        }
        
        if (redValueButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.redMultiplier >= redValueMax)
        {
            redValueButton.gameObject.SetActive(false);
        }
        else
        {
            redValueButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= redValueCost && GameMaster.instance.SaveSystem.redUnlocked;
        }
        
        if (blueValueButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.blueMultiplier >= blueValueMax)
        {
            blueValueButton.gameObject.SetActive(false);
        }
        else
        {
            blueValueButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= blueValueCost && GameMaster.instance.SaveSystem.blueUnlocked;
        }
        
        if (goldenSporeButton.isActiveAndEnabled && GameMaster.instance.SaveSystem.goldenSporeUnlocked)
        {
            goldenSporeButton.gameObject.SetActive(false);
        }
        else
        {
            goldenSporeButton.interactable = GameMaster.instance.SaveSystem.hivemindPoints >= goldenSporeCost;
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
