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
    [Header("Auto Harvest")]
    [SerializeField] private Button autoHarvestButton;
    [SerializeField] private TextMeshProUGUI autoHarvestText;
    [SerializeField] private uint autoHarvestCost = 5;
    [SerializeField] private bool autoHarvestVisible = false;
    [Header("Auto Harvest Speed")]
    [SerializeField] private Button autoHarvestSpeedButton;
    [SerializeField] private TextMeshProUGUI autoHarvestSpeedText;
    [SerializeField] private uint autoHarvestSpeedCost = 50;
    [SerializeField] private uint autoHarvestSpeedRatio = 2;
    [SerializeField] private bool autoHarvestSpeedVisible = false;

    [Header("Growth Speed")]
    [SerializeField] private Button growthSpeedButton;
    [SerializeField] private TextMeshProUGUI growthSpeedText;
    [SerializeField] private uint growthSpeedCost = 5;
    [SerializeField] private uint growthSpeedRatio= 3;
    [SerializeField] private bool growthSpeedVisible = false;
    
    [Header("Enrich")]
    [SerializeField] private TextMeshProUGUI enrichCostText;
    [SerializeField] private Button enrichButton;
    
    [Header("Meta")]
    [SerializeField] private Toggle showUpgrades;
    [SerializeField] private RectTransform upgradePanel;

    public MushroomBlock.MushroomType mushroomType;
    void Start()
    {
        UpdateButtons();
        
    }

    public void Awake()
    {
    }

    public void ToggleShowUpgrades()
    {
        // SFXMaster.instance.PlayMenuClick();
        upgradePanel.DOLocalMoveX(showUpgrades.isOn ? 700 : 1500, 0.5f).SetEase(Ease.OutBounce);
    }
    
    public void PurchaseAutoHarvest()
    {
        if (ScoreMaster.instance.SpendMushrooms(mushroomType,autoHarvestCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.autoHarvest[(int)mushroomType] = true;
            autoHarvestButton.interactable = false;
            autoHarvestButton.gameObject.SetActive(false);
        }
    }
    
    public void PurchaseAutoHarvestSpeed()
    {
        autoHarvestSpeedCost=(uint) Mathf.Max(50+Mathf.Pow(autoHarvestSpeedRatio, GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType]),autoHarvestSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(mushroomType,autoHarvestSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType]++;
            UpdateAutoHarvestSpeedButton();
        }
    }
    
    public void PurchaseGrowthSpeed()
    {
        growthSpeedCost =(uint) Mathf.Max(Mathf.Pow(growthSpeedRatio, GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType]),growthSpeedCost);
        if (ScoreMaster.instance.SpendMushrooms(mushroomType,growthSpeedCost))
        {
            SFXMaster.instance.PlayMenuClick();
            GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType]++;
            UpdateGrowthSpeedButton();
        }
    }
    
    public void UpdateButtons()
    {
        UpdateAutoHarvestButton();
        UpdateGrowthSpeedButton();
        UpdateAutoHarvestSpeedButton();
        UpdateEnrichButton();
    }

    private void UpdateEnrichButton()
    {
        uint cost = (uint)Mathf.Pow(GameMaster.instance.SaveSystem.mushroomBlockCount[(int)mushroomType], 2);
        enrichCostText.text = cost + "x";
        enrichButton.interactable = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= cost;
    }

    private void UpdateAutoHarvestButton()
    {
        autoHarvestButton.interactable = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= autoHarvestCost;
        autoHarvestText.text = autoHarvestCost + "x";
    }

    private void UpdateGrowthSpeedButton()
    {
        growthSpeedCost = (uint)Mathf.Max(Mathf.Pow(growthSpeedRatio, GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType]), growthSpeedCost);
        growthSpeedButton.interactable = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= growthSpeedCost;
        growthSpeedText.text = growthSpeedCost + "x";
        if (GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType]>=20)
        {
            growthSpeedButton.gameObject.SetActive(false);
        }
    }

    public void UpdateAutoHarvestSpeedButton()
    {
        autoHarvestSpeedCost = (uint)Mathf.Max(Mathf.Pow(autoHarvestSpeedRatio, GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType]), autoHarvestSpeedCost);
        autoHarvestSpeedButton.interactable = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= autoHarvestSpeedCost;
        autoHarvestSpeedText.text = autoHarvestSpeedCost + "x";
        if (GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType] >= 20)
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
            autoHarvestVisible = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= autoHarvestCost;
            autoHarvestButton.gameObject.SetActive(autoHarvestVisible && !GameMaster.instance.SaveSystem.autoHarvest[(int)mushroomType]);
        }

        if (!growthSpeedVisible && GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType] < 20)
        {
            growthSpeedVisible = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= growthSpeedCost || GameMaster.instance.SaveSystem.growthSpeedBonus[(int)mushroomType] is > 0 and < 20;
            growthSpeedButton.gameObject.SetActive(growthSpeedVisible);
        }
        
        if (!autoHarvestSpeedVisible && GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType]<20)
        {
            autoHarvestSpeedVisible = GameMaster.instance.SaveSystem.mushrooms[(int)mushroomType] >= autoHarvestSpeedCost || GameMaster.instance.SaveSystem.autoHarvestSpeed[(int)mushroomType] is > 0 and < 20;
            autoHarvestSpeedButton.gameObject.SetActive(autoHarvestSpeedVisible);
        }
    }
    
    public void Reset()
    {
        GameMaster.instance.SaveSystem.autoHarvest = new bool[3];
        GameMaster.instance.SaveSystem.growthSpeedBonus = new uint[3];
        GameMaster.instance.SaveSystem.autoHarvestSpeed = new uint[3];
        autoHarvestVisible = false;
        growthSpeedVisible = false;
        autoHarvestSpeedVisible = false;
        autoHarvestCost = 5;
        growthSpeedCost = 5;
        autoHarvestSpeedCost = 50;
        UpdateButtons();
    }
}
