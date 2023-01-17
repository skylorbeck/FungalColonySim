using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UpgradeMaster : MonoBehaviour
{
    public static UpgradeMaster instance;
    [Header("Auto Harvest")]
    [SerializeField] private Button autoHarvestButton;
    [SerializeField] private TextMeshProUGUI autoHarvestText;
    [SerializeField] private uint autoHarvestCost = 5;
    [SerializeField] private bool autoHarvestVisible = false;
    [Header("Auto Harvest Speed")]
    [SerializeField] private Button autoHarvestSpeedButton;
    [SerializeField] private TextMeshProUGUI autoHarvestSpeedText;
    [SerializeField] private uint autoHarvestSpeedCost = 50;
    [SerializeField] private uint autoHarvestSpeedRatio = 3;
    [SerializeField] private bool autoHarvestSpeedVisible = false;

    [Header("Growth Speed")]
    [SerializeField] private Button growthSpeedButton;
    [SerializeField] private TextMeshProUGUI growthSpeedText;
    [SerializeField] private uint growthSpeedCost = 5;
    [SerializeField] private uint growthSpeedRatio= 3;
    [SerializeField] private bool growthSpeedVisible = false;

    
    [Header("Meta")]
    [SerializeField] private Toggle showUpgrades;
    [SerializeField] private RectTransform upgradePanel;

    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateButtons();
        
    }
    
    public void ToggleShowUpgrades()
    {
        SFXMaster.instance.PlayMenuClick();

        if (showUpgrades.isOn)
        {
            upgradePanel.DOLocalMoveX(700, 0.5f);
        } else
        {
            upgradePanel.DOLocalMoveX(1500, 0.5f);
        }
    }
    
    public void PurchaseAutoHarvest()
    {
        if (ScoreMaster.instance.SpendMushrooms(autoHarvestCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.autoHarvest = true;
            autoHarvestButton.interactable = false;
            autoHarvestButton.gameObject.SetActive(false);
        }
    }
    
   
    
    public void PurchaseAutoHarvestSpeed()
    {
        autoHarvestSpeedCost=(uint) Mathf.Max(50+Mathf.Pow(autoHarvestSpeedRatio, GameMaster.instance.SaveSystem.autoHarvestSpeed),autoHarvestSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(autoHarvestSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.autoHarvestSpeed++;
            UpdateAutoHarvestSpeedButton();
        }
    }
    
    public void PurchaseGrowthSpeed()
    {
        growthSpeedCost =(uint) Mathf.Max(Mathf.Pow(growthSpeedRatio, GameMaster.instance.SaveSystem.growthSpeedBonus),growthSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(growthSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.growthSpeedBonus++;
            UpdateGrowthSpeedButton();
        }
    }
    
    public void UpdateButtons()
    {
        UpdateAutoHarvestButton();
        UpdateGrowthSpeedButton();
        UpdateAutoHarvestSpeedButton();
    }

    private void UpdateAutoHarvestButton()
    {
        autoHarvestButton.interactable = GameMaster.instance.SaveSystem.BrownMushrooms >= autoHarvestCost;
        autoHarvestText.text = autoHarvestCost + "x";
    }

    private void UpdateGrowthSpeedButton()
    {
        growthSpeedCost = (uint)Mathf.Max(Mathf.Pow(growthSpeedRatio, GameMaster.instance.SaveSystem.growthSpeedBonus), growthSpeedCost);
        growthSpeedButton.interactable = GameMaster.instance.SaveSystem.BrownMushrooms >= growthSpeedCost;
        growthSpeedText.text = growthSpeedCost + "x";
        if (GameMaster.instance.SaveSystem.growthSpeedBonus>=20)
        {
            growthSpeedButton.gameObject.SetActive(false);
        }
    }

    public void UpdateAutoHarvestSpeedButton()
    {
        autoHarvestSpeedCost = (uint)Mathf.Max(Mathf.Pow(autoHarvestSpeedRatio, GameMaster.instance.SaveSystem.autoHarvestSpeed), autoHarvestSpeedCost);
        autoHarvestSpeedButton.interactable = GameMaster.instance.SaveSystem.BrownMushrooms >= autoHarvestSpeedCost;
        autoHarvestSpeedText.text = autoHarvestSpeedCost + "x";
        if (GameMaster.instance.SaveSystem.autoHarvestSpeed >= 20)
        {
            autoHarvestSpeedButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!autoHarvestVisible)
        {
            autoHarvestVisible = GameMaster.instance.SaveSystem.BrownMushrooms >= autoHarvestCost;
            autoHarvestButton.gameObject.SetActive(autoHarvestVisible && !GameMaster.instance.SaveSystem.autoHarvest);
        }

        if (!growthSpeedVisible && GameMaster.instance.SaveSystem.growthSpeedBonus < 20)
        {
            growthSpeedVisible = GameMaster.instance.SaveSystem.BrownMushrooms >= growthSpeedCost || GameMaster.instance.SaveSystem.growthSpeedBonus is > 0 and < 20;
            growthSpeedButton.gameObject.SetActive(growthSpeedVisible);
        }
        
        if (!autoHarvestSpeedVisible && GameMaster.instance.SaveSystem.autoHarvestSpeed<20)
        {
            autoHarvestSpeedVisible = GameMaster.instance.SaveSystem.BrownMushrooms >= autoHarvestSpeedCost || GameMaster.instance.SaveSystem.autoHarvestSpeed is > 0 and < 20;
            autoHarvestSpeedButton.gameObject.SetActive(autoHarvestSpeedVisible);
        }
    }
    
    public void Reset()
    {
        GameMaster.instance.SaveSystem.autoHarvest = false;
        GameMaster.instance.SaveSystem.growthSpeedBonus = 0;
        GameMaster.instance.SaveSystem.autoHarvestSpeed = 0;
        autoHarvestVisible = false;
        growthSpeedVisible = false;
        autoHarvestSpeedVisible = false;
        autoHarvestCost = 5;
        growthSpeedCost = 5;
        autoHarvestSpeedCost = 50;
        UpdateButtons();
    }
}
